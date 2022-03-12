using Microsoft.AspNetCore.Builder;
using Servly.Idempotency.AspNetCore.Middleware;

namespace Servly.Idempotency.AspNetCore.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<IdempotencyMiddleware>();
    }
}
