namespace Servly.AspNetCore.Idempotency.Providers;

public class IdempotencyData
{
    public string RequestHash { get; }
    public bool InFlight { get; }
    public ResponseData? Response { get; }

    public IdempotencyData(string requestHash, bool inFlight = true, ResponseData? response = null)
    {
        RequestHash = requestHash;
        InFlight = inFlight;
        Response = response;
    }

    public class ResponseData
    {
        public int StatusCode { get; }
        public string ContentType { get; }
        public Dictionary<string, List<string>> Headers { get; }
        public string ResponseBody { get; }

        public ResponseData(int statusCode, string contentType, Dictionary<string, List<string>> headers, string responseBody)
        {
            StatusCode = statusCode;
            ContentType = contentType;
            Headers = headers;
            ResponseBody = responseBody;
        }
    }
}
