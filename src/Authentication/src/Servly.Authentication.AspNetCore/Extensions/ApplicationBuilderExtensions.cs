using Microsoft.AspNetCore.Builder;
using Servly.Authentication.AspNetCore.Middleware;

namespace Servly.Authentication.AspNetCore.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAuthenticationContextMiddleware<TState>(this IApplicationBuilder app)
        where TState : class, IAuthenticationContextState, new()
    {
        return app
            .UseMiddleware<AuthenticationContextMiddleware<TState>>();
    }
}
