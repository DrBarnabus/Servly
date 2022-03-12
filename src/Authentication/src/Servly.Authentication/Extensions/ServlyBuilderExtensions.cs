using Microsoft.Extensions.DependencyInjection;
using Servly.Core;

namespace Servly.Authentication.Extensions;

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
