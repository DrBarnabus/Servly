using Microsoft.Extensions.DependencyInjection;
using Servly.Authentication;
using Servly.Core;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ServlyBuilderExtensions
{
    private const string AuthenticationContextModuleName = "Authentication.Context";

    public static IServlyBuilder AddAuthenticationContext<TState>(this IServlyBuilder builder)
        where TState : class, IAuthenticationContextState, new()
    {
        if (builder.TryRegisterModule($"{AuthenticationContextModuleName}-{typeof(TState)}"))
            return builder;

        builder.Services
            .AddScoped<IAuthenticationContext<TState>>(_ => new AuthenticationContext<TState>(new TState()));

        return builder;
    }
}
