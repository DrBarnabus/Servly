using Microsoft.AspNetCore.Builder;
using Servly.Idempotency.AspNetCore.Middleware;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<IdempotencyMiddleware>();
    }
}
