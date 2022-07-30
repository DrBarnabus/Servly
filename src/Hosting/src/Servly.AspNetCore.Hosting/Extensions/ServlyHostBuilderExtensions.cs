using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Servly.Hosting;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

/// <summary>
///     Extension methods for configuring the <see cref="IServlyHostBuilder"/>.
/// </summary>
public static class ServlyHostBuilderExtensions
{
    /// <summary>
    ///     Configures a <see cref="IServlyHostBuilder"/> with defaults for hosting an AspNetCore app. It can then be
    ///     be used to configure the applications endpoints and other AspNetCore specific features.
    /// </summary>
    /// <param name="builder">The <see cref="IServlyHostBuilder"/> instance to configure.</param>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IServlyHostBuilder"/>.</param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder"/> for chaining.</returns>
    public static IServlyHostBuilder ConfigureWebHost(this IServlyHostBuilder builder, Action<IWebHostBuilder> configureDelegate)
    {
        return builder
            .ConfigureInternalHost(internalBuilder => internalBuilder
                .ConfigureWebHostDefaults(configureDelegate));
    }
}
