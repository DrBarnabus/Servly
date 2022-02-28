using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Servly.Idempotency.AspNetCore.Implementations;
using Servly.Idempotency.AspNetCore.Middleware;

namespace Servly.Idempotency.AspNetCore;

public static class Extensions
{
    public static IServiceCollection AddIdempotency(this IServiceCollection services)
    {
        return services
            .AddSingleton<IIdempotencyPersistenceProvider, RedisIdempotencyPersistenceProvider>()
            .AddScoped<IdempotencyMiddleware>();
    }

    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app)
    {
        return app
            .UseMiddleware<IdempotencyMiddleware>();
    }
}
