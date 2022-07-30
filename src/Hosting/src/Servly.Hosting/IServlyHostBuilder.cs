using Microsoft.Extensions.Hosting;
using Servly.Core;

namespace Servly.Hosting;

public interface IServlyHostBuilder : IHostBuilder
{
    /// <summary>
    ///     Configures the Servly Modules that are enabled. This method can only be called once.
    /// </summary>
    /// <param name="configureServly">The action for configuring the modules using <see cref="IServlyBuilder" />.</param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder" /> for chaining.</returns>
    IServlyHostBuilder ConfigureModules(Action<IServlyBuilder> configureServly);

    /// <summary>
    ///     Configures the Servly Modules that are enabled. This method can only be called once.
    /// </summary>
    /// <param name="configureServly">
    ///     The action for configuring the modules using <see cref="IServlyBuilder" />
    ///     including the <see cref="HostBuilderContext" /> for making environment specific decisions.
    /// </param>
    /// <returns>The same instance of the <see cref="IServlyHostBuilder" /> for chaining.</returns>
    IServlyHostBuilder ConfigureModules(Action<HostBuilderContext, IServlyBuilder> configureServly);
}
