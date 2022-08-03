using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Servly.Core.Exceptions;

namespace Servly.Core.Implementations;

internal class ServlyBuilder : IServlyBuilder
{
    private int _built;

    internal List<Action<IServiceCollection>> BuildActions { get; }
    internal ConcurrentDictionary<string, bool> RegisteredModules { get; }

    public ServlyBuilder(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;

        BuildActions = new List<Action<IServiceCollection>>();
        RegisteredModules = new ConcurrentDictionary<string, bool>();
    }

    public IServiceCollection Services { get; }
    public IConfiguration Configuration { get; }

    public bool TryRegisterModule(string moduleName)
    {
        return RegisteredModules.TryAdd(moduleName, true);
    }

    public bool IsModuleRegistered(string moduleName)
    {
        return RegisteredModules.TryGetValue(moduleName, out bool value) && value;
    }

    public void AddBuildAction(Action<IServiceCollection> buildAction)
    {
        Guard.Assert(buildAction is not null, $"BuildAction cannot be null");
        BuildActions.Add(buildAction);
    }

    public void AddInitializer<TInitializer>()
        where TInitializer : class, IInitializer
    {
        AddBuildAction(services => services.AddSingleton<IInitializer, TInitializer>());
    }

    public void AddOptions<TOptions>(string sectionKey, string? instanceName = null,
        Func<TOptions, bool>? validate = null)
        where TOptions : class, new()
    {
        Guard.Assert(!string.IsNullOrEmpty(sectionKey), $"ConfigurationSection cannot be null or empty");

        var optionsBuilder = Services.AddOptions<TOptions>(instanceName ?? string.Empty)
            .Bind(Configuration.GetSection(sectionKey));

        if (validate is not null)
            optionsBuilder.Validate(validate, $"{typeof(TOptions).Name} options from Configuration Section '{sectionKey}' has failed validation");
    }

    public TOptions GetOptions<TOptions>(string? instanceName = null)
        where TOptions : class, new()
    {
        var options = GetService<IOptionsSnapshot<TOptions>>();
        return instanceName is null ? options.Value : options.Get(instanceName);
    }

    public TService GetService<TService>()
        where TService : notnull
    {
        using var serviceProvider = Services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<TService>();
    }

    public void Build()
    {
        if (Interlocked.Exchange(ref _built, 1) == 1)
            throw new AlreadyBuiltException();

        foreach (var buildAction in BuildActions)
            buildAction.Invoke(Services);

        Services.AddSingleton<IStartupInitializer, StartupInitializer>();
    }
}
