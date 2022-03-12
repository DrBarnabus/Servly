﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Servly.Authentication.AspNetCore.Middleware;
using Servly.Authentication.Extensions;
using Servly.Core;

namespace Servly.Authentication.AspNetCore.Extensions;

public static class ServlyBuilderExtensions
{
    private const string AuthenticationContextMiddlewareModuleName = "Authentication.ContextMiddleware";

    public static IServlyBuilder AddAuthenticationContextMiddleware<TState>(this IServlyBuilder builder, Action<TState, HttpContext> setupContext)
        where TState : class, IAuthenticationContextState, new()
    {
        if (builder.TryRegisterModule($"{AuthenticationContextMiddlewareModuleName}-{typeof(TState)}"))
            return builder;

        // Add Authentication Context if not already registered
        builder.AddAuthenticationContext<TState>();

        builder.Services
            .AddScoped(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<AuthenticationContextMiddleware<TState>>>();
                return new AuthenticationContextMiddleware<TState>(logger, setupContext);
            });

        return builder;
    }
}
