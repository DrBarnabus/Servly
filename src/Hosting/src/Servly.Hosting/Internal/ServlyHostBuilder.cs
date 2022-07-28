using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Servly.Hosting.Internal;

public class ServlyHostBuilder : IServlyHostBuilder
{
    private readonly List<Action<IConfigurationBuilder>> _configureHostConfigurationActions = new();
    private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigurationActions = new();
    private readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new();
    private readonly List<IConfigureContainerAdapter> _configureContainerActions = new();

    private bool _hostBuilt;
    private HostBuilderContext? _hostBuilderContext;
    private HostingEnvironment? _hostingEnvironment;
    private IConfiguration? _hostConfiguration;
    private IConfiguration? _appConfiguration;

    private IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

    /// <inheritdoc />
    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    /// <inheritdoc />
    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _configureHostConfigurationActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _configureAppConfigurationActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _configureServicesActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException(nameof(factory)));
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostBuilderContext!,
            factory ?? throw new ArgumentNullException(nameof(factory)));
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainerActions.Add(new ConfigureContainerAdapter<TContainerBuilder>(configureDelegate
            ?? throw new ArgumentNullException(nameof(configureDelegate))));
        return this;
    }

    /// <inheritdoc />
    public IHost Build()
    {
        if (_hostBuilt)
            throw new InvalidOperationException("Build can only be called once.");

        _hostBuilt = true;

        BuildHostConfiguration();
        CreateHostingEnvironment();
        CreateHostBuilderContext();
        BuildAppConfiguration();
        CreateServiceProvider();

        var appServices = CreateServiceProvider();
        return appServices.GetRequiredService<IHost>();
    }

    private void BuildHostConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection();

        foreach (var buildAction in _configureHostConfigurationActions)
            buildAction(configurationBuilder);

        _hostConfiguration = configurationBuilder.Build();
    }

    private void CreateHostingEnvironment()
    {
        _hostingEnvironment = new HostingEnvironment()
        {
            ApplicationName = _hostConfiguration![HostDefaults.ApplicationKey],
            EnvironmentName = _hostConfiguration![HostDefaults.EnvironmentKey] ?? Environments.Production,
            ContentRootPath = ResolveContentRootPath(_hostConfiguration![HostDefaults.ContentRootKey], AppContext.BaseDirectory),
        };

        if (string.IsNullOrEmpty(_hostingEnvironment.ApplicationName))
            _hostingEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;

        _hostingEnvironment.ContentRootFileProvider = new PhysicalFileProvider(_hostingEnvironment.ContentRootPath);
    }

    private static string ResolveContentRootPath(string contentRootPath, string basePath)
    {
        return string.IsNullOrEmpty(contentRootPath) ? basePath :
            Path.IsPathRooted(contentRootPath) ? contentRootPath :
            Path.Combine(Path.GetFullPath(basePath), contentRootPath);
    }

    private void CreateHostBuilderContext()
    {
        _hostBuilderContext = new HostBuilderContext(Properties)
        {
            HostingEnvironment = _hostingEnvironment,
            Configuration = _hostConfiguration
        };
    }

    private void BuildAppConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(_hostingEnvironment!.ContentRootPath)
            .AddConfiguration(_hostConfiguration, shouldDisposeConfiguration: true);

        foreach (var buildAction in _configureAppConfigurationActions)
            buildAction(_hostBuilderContext!, configBuilder);

        _appConfiguration = configBuilder.Build();
        _hostBuilderContext!.Configuration = _appConfiguration;
    }

    private IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
#pragma warning disable CS0618 // Type or member is obsolete
        services.AddSingleton<IHostingEnvironment>(_hostingEnvironment!);
#pragma warning restore CS0618 // Type or member is obsolete
        services.AddSingleton<IHostEnvironment>(_hostingEnvironment!);
        services.AddSingleton(_hostBuilderContext!);
        services.AddSingleton(_ => _appConfiguration!);
#pragma warning disable CS0618 // Type or member is obsolete
        services.AddSingleton(s => (IApplicationLifetime)s.GetService<IHostApplicationLifetime>()!);
#pragma warning restore CS0618 // Type or member is obsolete
        services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
        services.AddSingleton<IHostLifetime, ConsoleLifetime>();
        services.AddSingleton<IHost, ServlyHost>();
        services.AddOptions();
        services.AddLogging();

        foreach (var configureServicesAction in _configureServicesActions)
            configureServicesAction(_hostBuilderContext!, services);

        object containerBuilder = _serviceProviderFactory.CreateBuilder(services);
        foreach (var containerAction in _configureContainerActions)
            containerAction.ConfigureContainer(_hostBuilderContext!, containerBuilder);

        var appServices = _serviceProviderFactory.CreateServiceProvider(containerBuilder);
        if (appServices is null)
            throw new InvalidOperationException($"The IServiceProviderFactory returned a null IServiceProvider.");

        _ = appServices.GetService<IConfiguration>();

        return appServices;
    }
}
