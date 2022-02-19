using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Servly.Authentication.AspNetCore.Middleware;

namespace Servly.Authentication.AspNetCore.Extensions;

public static class AuthenticationContextExtensions
{
    public static IServiceCollection AddAuthenticationContext<TState>(this IServiceCollection services, Action<TState, HttpContext> setupContext)
        where TState : class, IAuthenticationContextState, new()
    {
        return services
            .AddScoped<IAuthenticationContext<TState>>(_ => new AuthenticationContext<TState>(new TState()))
            .AddScoped(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<AuthenticationContextMiddleware<TState>>>();
                return new AuthenticationContextMiddleware<TState>(logger, setupContext);
            });
    }

    public static IApplicationBuilder UseAuthenticationContext<TState>(this IApplicationBuilder app)
        where TState : class, IAuthenticationContextState, new()
    {
        return app
            .UseMiddleware<AuthenticationContextMiddleware<TState>>();
    }
}
