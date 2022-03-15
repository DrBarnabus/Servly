using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Servly.Authentication;
using Servly.Core;

namespace Servly.AspNetCore.Authentication.Middleware;

public class AuthenticationContextMiddleware<TState> : IMiddleware
    where TState : class, IAuthenticationContextState, new()
{
    private readonly ILogger<AuthenticationContextMiddleware<TState>> _logger;
    private readonly Action<TState, HttpContext> _setupContext;

    public AuthenticationContextMiddleware(ILogger<AuthenticationContextMiddleware<TState>> logger, Action<TState, HttpContext> setupContext)
    {
        Guard.Assert(logger is not null, $"Logger cannot be null");
        Guard.Assert(setupContext is not null, $"SetupContext cannot be null");

        _logger = logger;
        _setupContext = setupContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            _logger.LogTrace("Request is not authenticated, unable to set authentication context");
            await next(context);
            return;
        }

        _logger.LogTrace("Request is authenticated, initializing authentication context from Identity in HttpContext");

        var authenticationContext = context.RequestServices.GetRequiredService<IAuthenticationContext<TState>>();
        using var _ = authenticationContext.SetContext(state =>
        {
            state.IsAuthenticated = true;
            _setupContext(state, context);
        });

        await next(context);
    }
}
