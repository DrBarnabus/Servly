using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Servly.Idempotency.AspNetCore.Attributes;
using Servly.Idempotency.AspNetCore.Extensions;
using Servly.Idempotency.AspNetCore.Middleware;
using Shouldly;
using Xunit;

namespace Servly.Idempotency.AspNetCore.FunctionalTests.Middleware;

public class IdempotencyMiddlewareTests
{
    [Fact]
    public async Task ShouldNotApplyWhenEndpointHasNoIdempotentMetadata()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", context => context.Response.WriteAsync("Response"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("n"));
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    public async Task ShouldNotApplyWhenTypeIsGet()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapGet("/", context => context.Response.WriteAsync("Response"))
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Get;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("n"));
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    public async Task ShouldNotApplyWhenTypeIsPut()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPut("/", context => context.Response.WriteAsync("Response"))
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Put;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("n"));
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    public async Task ShouldNotApplyWhenTypeIsDelete()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapDelete("/", context => context.Response.WriteAsync("Response"))
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Delete;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("n"));
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    public async Task ShouldNotApplyWhenIdempotencyKeyNotProvided()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", context => context.Response.WriteAsync("Response"))
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    public async Task ShouldNotApplyWhenIdempotencyKeyHasMultipleValues()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", context => context.Response.WriteAsync("Response"))
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", new StringValues(new[]{"ValueOne", "ValueTwo"}));
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    public async Task ShouldNotApplyWhenIdempotencyKeyIsEmpty()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", context => context.Response.WriteAsync("Response"))
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", "");
        });

        // Assert
        persistenceProvider.Data.ShouldBeEmpty();
        context.Response.Headers.ShouldNotContainKey("Idempotency-Key");
    }

    [Fact]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public async Task ShouldAddToInFlightCacheWhileProcessing()
    {
        // Setup
        var cts = new CancellationTokenSource();
        var cts2 = new CancellationTokenSource();

        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", async context =>
                {
                    cts.Cancel();
                    cts2.Token.WaitHandle.WaitOne();
                    await context.Response.WriteAsync("Response");
                })
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        string idempotencyKey = Guid.NewGuid().ToString("n");

        // Act
        var requestTask = server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Wait until request has started processing
        cts.Token.WaitHandle.WaitOne();

        // Assert
        persistenceProvider.Data.ShouldNotBeEmpty();
        persistenceProvider.Data.ShouldContainKey($"TestPrefix-{idempotencyKey}");

        var inFlightData = persistenceProvider.Data[$"TestPrefix-{idempotencyKey}"];
        inFlightData.RequestHash.ShouldBe("CF2EBBDE289B4C9730ADBE3DCCBF12342DE8976FCEDB847FD0EA9692C7A0C6C7");
        inFlightData.InFlight.ShouldBeTrue();

        cts2.Cancel();

        await requestTask;
    }

    [Fact]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public async Task ShouldAddToInFlightCacheWhileProcessingWhenBodyProvided()
    {
        // Setup
        string idempotencyKey = Guid.NewGuid().ToString("n");

        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", async context =>
                {
                    // Assert
                    persistenceProvider.Data.ShouldNotBeEmpty();
                    persistenceProvider.Data.ShouldContainKey($"TestPrefix-{idempotencyKey}");

                    var inFlightData = persistenceProvider.Data[$"TestPrefix-{idempotencyKey}"];
                    inFlightData.RequestHash.ShouldBe("04CEE54FFA745DFA782B79B6DCB2D7E4631CCB0C71DB7B9FA1370576A4A7BB05");
                    inFlightData.InFlight.ShouldBeTrue();

                    // Test Data
                    await context.Response.WriteAsync("Response");
                })
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);

            c.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{\"testData\": \"Some Test Json Data\"}"));
            c.Request.ContentLength = c.Request.Body.Length;
        });

        // Assert
        context.Response.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task ShouldCorrectlyPersistRequestDataWhenRequestIsValid()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", async context =>
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers.Add("TestHeader", "TestValue");
                    await context.Response.WriteAsync("Response");
                })
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        string idempotencyKey = Guid.NewGuid().ToString("n");

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Assert
        persistenceProvider.Data.ShouldContainKey($"TestPrefix-{idempotencyKey}");
        persistenceProvider.Data.ShouldNotBeEmpty();
        var persistedData = persistenceProvider.Data[$"TestPrefix-{idempotencyKey}"];
        persistedData.RequestHash.ShouldBe("CF2EBBDE289B4C9730ADBE3DCCBF12342DE8976FCEDB847FD0EA9692C7A0C6C7");
        persistedData.InFlight.ShouldBeFalse();
        persistedData.Response.ShouldNotBeNull();
        persistedData.Response.StatusCode.ShouldBe(200);
        persistedData.Response.ContentType.ShouldBe("text/plain");
        persistedData.Response.Headers.ShouldContainKey("TestHeader");
        persistedData.Response.Headers["TestHeader"][0].ShouldBe("TestValue");
        persistedData.Response.ResponseBody.ShouldBe("UmVzcG9uc2U=");

        context.Response.Headers.ShouldContainKey("Idempotency-Key");
        context.Response.Headers["Idempotency-Key"][0].ShouldBe(idempotencyKey);
    }

    [Fact]
    public async Task ShouldReturnA400ErrorWhenDifferentRequestSentWithSameKey()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", async context =>
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers.Add("TestHeader", "TestValue");
                    await context.Response.WriteAsync("Response");
                })
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        string idempotencyKey = Guid.NewGuid().ToString("n");

        // Initial Request
        var _ = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.QueryString = new QueryString("?with=QueryString");
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Assert
        context.Response.StatusCode.ShouldBe(400);
        context.Response.ContentType.ShouldBe("application/problem+json");

        using var streamReader = new StreamReader(context.Response.Body);
        string body = await streamReader.ReadToEndAsync();
        body.ShouldContain("Idempotency Key Conflict.");
        body.ShouldContain("A request with a different signature was already processed with the same idempotency key.");
        body.ShouldContain(idempotencyKey);
    }

    [Fact]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public async Task ShouldReturnA409ErrorWhenOriginalRequestIsStillInFlight()
    {
        // Setup
        var cts = new CancellationTokenSource();
        var cts2 = new CancellationTokenSource();

        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", async context =>
                {
                    if (!cts.IsCancellationRequested)
                    {
                        cts.Cancel();
                        cts2.Token.WaitHandle.WaitOne();
                    }

                    await context.Response.WriteAsync("Response");
                })
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        string idempotencyKey = Guid.NewGuid().ToString("n");

        // Act
        var requestTask = server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Wait until request has started processing
        cts.Token.WaitHandle.WaitOne();

        // Assert
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        context.Response.StatusCode.ShouldBe(409);
        context.Response.ContentType.ShouldBe("application/problem+json");

        using var streamReader = new StreamReader(context.Response.Body);
        string body = await streamReader.ReadToEndAsync();
        body.ShouldContain("Idempotent request is already in flight.");
        body.ShouldContain("The original request is still 'in-flight' and is currently being processed.");
        body.ShouldContain(idempotencyKey);

        cts2.Cancel();

        await requestTask;
    }

    [Fact]
    public async Task ShouldReturnCorrectlyWhenDuplicateRequestIsSent()
    {
        // Setup
        var persistenceProvider = new MockIdempotencyPersistenceProvider();
        using var host = await CreateServer(persistenceProvider, builder =>
        {
            builder.MapPost("/", async context =>
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers.Add("TestHeader", "TestValue");
                    await context.Response.WriteAsync("Response");
                })
                .WithMetadata(new IdempotentAttribute("TestPrefix"));
        });

        var server = host.GetTestServer();

        string idempotencyKey = Guid.NewGuid().ToString("n");

        // Send first Request
        var _ = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Act
        var context = await server.SendAsync(c =>
        {
            c.Request.Method = HttpMethods.Post;
            c.Request.Path = "/";
            c.Request.Headers.Add("Idempotency-Key", idempotencyKey);
        });

        // Assert
        context.Response.StatusCode.ShouldBe(200);
        context.Response.ContentType.ShouldBe("text/plain");
        context.Response.Headers.ShouldContainKeyAndValue("TestHeader", "TestValue");

        using var streamReader = new StreamReader(context.Response.Body);
        string body = await streamReader.ReadToEndAsync();
        body.ShouldContain("Response");

        context.Response.Headers.ShouldContainKeyAndValue("Idempotency-Key", $"{idempotencyKey}, replay=1");
    }

    private static Task<IHost> CreateServer(IIdempotencyPersistenceProvider persistenceProvider, Action<IEndpointRouteBuilder> configureEndpoints)
    {
        return new HostBuilder()
            .ConfigureWebHost(builder => builder
                .UseTestServer()
                .ConfigureServices(services => services
                    .AddScoped(_ => persistenceProvider)
                    .AddScoped<IdempotencyMiddleware>()
                    .AddControllers())
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseIdempotency();
                    app.UseEndpoints(configureEndpoints);
                }))
            .StartAsync();
    }
}
