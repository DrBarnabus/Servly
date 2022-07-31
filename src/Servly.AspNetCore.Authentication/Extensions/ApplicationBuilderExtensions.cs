using Microsoft.AspNetCore.Builder;
using Servly.AspNetCore.Authentication.Middleware;
using Servly.Authentication;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAuthenticationContextMiddleware<TState>(this IApplicationBuilder app)
        where TState : class, IAuthenticationContextState, new()
    {
        return app
            .UseMiddleware<AuthenticationContextMiddleware<TState>>();
    }
}
