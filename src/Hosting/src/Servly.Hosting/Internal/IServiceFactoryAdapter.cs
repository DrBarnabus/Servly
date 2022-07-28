using Microsoft.Extensions.DependencyInjection;

namespace Servly.Hosting.Internal;

internal interface IServiceFactoryAdapter
{
    object CreateBuilder(IServiceCollection services);

    IServiceProvider CreateServiceProvider(object containerBuilder);
}
