using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Servly.AspNetCore.Idempotency.Attributes;

namespace Servly.AspNetCore.Idempotency.Middleware;

public class IdempotencyMiddleware : IMiddleware
{
    private const string ProblemJsonContentType = "application/problem+json";

    private readonly ILogger<IdempotencyMiddleware> _logger;
    private readonly IIdempotencyPersistenceProvider _persistence;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    private string _keyPrefix;
    private string _idempotencyKey;
    private string _prefixedIdempotencyKey;

    public IdempotencyMiddleware(ILogger<IdempotencyMiddleware> logger, IIdempotencyPersistenceProvider persistence, ProblemDetailsFactory problemDetailsFactory)
    {
        _logger = logger;
        _persistence = persistence;
        _problemDetailsFactory = problemDetailsFactory;

        _keyPrefix = string.Empty;
        _idempotencyKey = string.Empty;
        _prefixedIdempotencyKey = string.Empty;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!ExtractIdempotencySettingsFromEndpoint(context)
            || !CanApplyIdempotency(context)
            || !TryGetIdempotencyKey(context.Request, out string? idempotencyKey))
        {
            await next(context);
            return;
        }

        _idempotencyKey = idempotencyKey;
        _prefixedIdempotencyKey = $"{_keyPrefix}-{_idempotencyKey}";

        context.Request.EnableBuffering();

        using var _ = _logger.BeginScope(new Dictionary<string, string> {{"IdempotencyKey", _idempotencyKey}});

        if (await ApplyPreExecution(context))
            return; // Request has been handled by Pre-Execution step

        var originalResponseStream = context.Response.Body;
        await using var responseStream = new MemoryStream();

        try
        {
            context.Response.Body = responseStream;

            await next(context);

            await ApplyPostExecution(context, responseStream);
        }
        finally
        {
            responseStream.Position = 0;
            await responseStream.CopyToAsync(originalResponseStream);
            context.Response.Body = originalResponseStream;
        }
    }

    private async ValueTask<bool> ApplyPreExecution(HttpContext context)
    {
        _logger.LogTrace("Processing pre-execution for idempotency for Idempotency Key {IdempotencyKey}", _prefixedIdempotencyKey);

        var idempotencyData = await _persistence.ReadIdempotencyData(_prefixedIdempotencyKey, context.RequestAborted);
        if (idempotencyData is null)
        {
            _logger.LogTrace("Adding request into in-flight cache");
            var inFlightData = await GetInFlightIdempotencyData(context);
            await _persistence.WriteIdempotencyData(_prefixedIdempotencyKey, inFlightData, context.RequestAborted);

            return false;
        }

        string currentRequestHash = await CalculateRequestHash(context);
        if (currentRequestHash != idempotencyData.RequestHash)
        {
            _logger.LogWarning("A request with a different hash attempted to utilize the same idempotency key of {IdempotencyKey}", _idempotencyKey);

            const int statusCode = StatusCodes.Status400BadRequest;
            const string title = "Idempotency Key Conflict.";
            const string detail = "A request with a different signature was already processed with the same idempotency key.";
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(context, statusCode, title, detail: detail);
            problemDetails.Extensions.Add("IdempotencyKey", _idempotencyKey);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, null, ProblemJsonContentType, context.RequestAborted);
            return true;
        }

        if (idempotencyData.InFlight)
        {
            _logger.LogDebug("Request is currently in-flight, respond with conflict");

            const int statusCode = StatusCodes.Status409Conflict;
            const string title = "Idempotent request is already in flight.";
            const string detail = "The original request is still 'in-flight' and is currently being processed.";
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(context, statusCode, title, detail: detail);
            problemDetails.Extensions.Add("IdempotencyKey", _idempotencyKey);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, null, ProblemJsonContentType, context.RequestAborted);
            return true;
        }

        var responseData = idempotencyData.Response;
        if (responseData is null)
            throw new Exception("IdempotencyData Response Data is null");

        _logger.LogDebug("Response found in idempotency storage for {IdempotencyKey}, returning previous response.", _idempotencyKey);

        context.Response.StatusCode = responseData.StatusCode;
        context.Response.ContentType = responseData.ContentType;

        foreach ((string key, var value) in responseData.Headers)
        {
            if (!context.Response.Headers.ContainsKey(key))
                context.Response.Headers.Add(key, value.ToArray());
        }

        context.Response.Headers.Add("Idempotency-Key", $"{_idempotencyKey}, replay=1");

        byte[] responseBodyBytes = Convert.FromBase64String(responseData.ResponseBody);
        await context.Response.BodyWriter.WriteAsync(responseBodyBytes);

        return true;
    }

    private async ValueTask ApplyPostExecution(HttpContext context, MemoryStream responseStream)
    {
        _logger.LogTrace("Processing post-execution for idempotency, request has StatusCode {StatusCode} and ContentType {ContentType}",
            context.Response.StatusCode, context.Response.ContentType);

        var idempotencyData = await GetIdempotencyData(context, responseStream);
        await _persistence.WriteIdempotencyData(_prefixedIdempotencyKey, idempotencyData, context.RequestAborted);

        context.Response.Headers.Add("Idempotency-Key", _idempotencyKey);

        _logger.LogDebug("Result of request has been cached under Idempotency Key {IdempotencyKey}", _idempotencyKey);
    }

    private bool ExtractIdempotencySettingsFromEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var idempotencyAttribute = endpoint?.Metadata.GetMetadata<IdempotentAttribute>();
        if (idempotencyAttribute is null)
            return false;

        _keyPrefix = idempotencyAttribute.KeyPrefix;
        return true;
    }

    private static async Task<IdempotencyData> GetInFlightIdempotencyData(HttpContext context)
    {
        string requestHash = await CalculateRequestHash(context);
        return new IdempotencyData(requestHash);
    }

    private static async Task<IdempotencyData> GetIdempotencyData(HttpContext context, MemoryStream responseStream)
    {
        string requestHash = await CalculateRequestHash(context);
        var responseData = GetResponseData(context, responseStream);
        return new IdempotencyData(requestHash, false, responseData);
    }

    private static IdempotencyData.ResponseData GetResponseData(HttpContext context, MemoryStream responseStream)
    {
        var response = context.Response;

        int statusCode = response.StatusCode;
        string contentType = response.ContentType;
        var headers = response.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToList());

        responseStream.Position = 0;
        string responseBody = Convert.ToBase64String(responseStream.ToArray());

        var responseData = new IdempotencyData.ResponseData(statusCode, contentType, headers, responseBody);
        return responseData;
    }

    private static async Task<string> CalculateRequestHash(HttpContext context)
    {
        var request = context.Request;

        string method = request.Method;
        string pathAndQuery = request.GetEncodedPathAndQuery();

        using var hashAlgorithm = SHA256.Create();

        string bodyHash = string.Empty;
        if (request.ContentLength.HasValue && request.Body.CanRead && request.Body.CanSeek)
            bodyHash = Convert.ToHexString(await hashAlgorithm.ComputeHashAsync(request.Body, context.RequestAborted));

        byte[] requestHashBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes($"{method}-{pathAndQuery}-{bodyHash}"));
        return Convert.ToHexString(requestHashBytes);
    }

    private static bool TryGetIdempotencyKey(HttpRequest request, [NotNullWhen(true)] out string? idempotencyKey)
    {
        idempotencyKey = null;

        if (!request.Headers.TryGetValue("Idempotency-Key", out var keys))
            return false;

        if (keys.Count > 1)
            return false;

        if (keys.Count == 0 || string.IsNullOrEmpty(keys[0]))
            return false;

        idempotencyKey = keys[0];
        return true;
    }

    private bool CanApplyIdempotency(HttpContext context)
    {
        string method = context.Request.Method;
        if (method != HttpMethods.Post && method != HttpMethods.Patch)
        {
            _logger.LogWarning("Idempotency skipped method {HttpMethod} is not supported", method);
            return false;
        }

        if (context.Request.HasFormContentType)
        {
            _logger.LogWarning("Idempotency skipped, content type is {ContentType} which is currently not supported.", context.Request.ContentType);
            return false;
        }

        return true;
    }
}
