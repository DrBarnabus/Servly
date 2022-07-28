using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Servly.Hosting.Internal;

internal class ServiceFactoryAdapter<TContainerBuilder> : IServiceFactoryAdapter
    where TContainerBuilder : notnull
{
    private IServiceProviderFactory<TContainerBuilder>? _serviceProviderFactory;
    private readonly Func<HostBuilderContext>? _contextResolver;
    private readonly Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>>? _factoryResolver;

    public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
    {
        _serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
    }

    public ServiceFactoryAdapter(Func<HostBuilderContext> contextResolver, Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
    {
        _contextResolver = contextResolver ?? throw new ArgumentNullException(nameof(contextResolver));
        _factoryResolver = factoryResolver ?? throw new ArgumentNullException(nameof(factoryResolver));
    }

    public object CreateBuilder(IServiceCollection services)
    {
        if (_serviceProviderFactory is not null)
            return _serviceProviderFactory.CreateBuilder(services);

        _serviceProviderFactory = _factoryResolver!(_contextResolver!());

        if (_serviceProviderFactory is null)
            throw new InvalidOperationException("The resolver returned a null IServiceProviderFactory");

        return _serviceProviderFactory.CreateBuilder(services);
    }

    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        if (_serviceProviderFactory is null)
            throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");

        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
