using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servly.Core;

namespace Servly.Hosting;

/// <summary>
///     A program initialization abstraction.
/// </summary>
public interface IServlyHostBuilder
{
    /// <summary>
    ///     Sets up the configuration for the remainder of the build process and application. This can be called multiple times
    ///     and the results will be additive. The results will be available at <see cref="HostBuilderContext.Configuration" />
    ///     for subsequent operations, as well as in <see cref="IServlyHost.Services" />.
    /// </summary>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the <see cref="IConfigurationBuilder" /> that will be used
    ///     to construct the <see cref="IConfiguration" /> for the application.
    /// </param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder" /> for chaining.</returns>
    IServlyHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate);

    /// <summary>
    ///     Adds services to the container. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the <see cref="IServiceCollection" /> that will be used
    ///     to construct the <see cref="IServiceProvider" />.
    /// </param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder" /> for chaining.</returns>
    IServlyHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);

    /// <summary>
    ///     Configures the Servly Modules that are enabled. This method can only be called once.
    /// </summary>
    /// <param name="configureDelegate">
    ///     The delegate for configuring the modules using <see cref="IServlyBuilder" />
    ///     including the <see cref="HostBuilderContext" /> for making environment specific decisions.
    /// </param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder" /> for chaining.</returns>
    IServlyHostBuilder ConfigureModules(Action<HostBuilderContext, IServlyBuilder> configureDelegate);

    /// <summary>
    ///     Allows for configuration of the internal host inside the <see cref="IServlyHostBuilder"/>, this allows
    ///     for advanced configuration scenarios requiring direct access to <see cref="IHostBuilder"/> methods. This
    ///     can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the internal <see cref="IHostBuilder"/>.</param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder"/> for chaining.</returns>
    IServlyHostBuilder ConfigureInternalHost(Action<IHostBuilder> configureDelegate);

    /// <summary>
    ///     Run the given actions to initialize the host. This can only be called once.
    /// </summary>
    /// <returns>An initialized <see cref="IHost" />.</returns>
    IServlyHost Build();
}
