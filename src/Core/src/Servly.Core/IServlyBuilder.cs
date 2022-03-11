using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Servly.Core;

public interface IServlyBuilder
{
    IServiceCollection Services { get; }

    IConfiguration Configuration { get; }

    bool TryRegisterModule(string moduleName);

    bool IsModuleRegistered(string moduleName);

    void AddBuildAction(Action<IServiceCollection> buildAction);

    void AddInitializer<TInitializer>()
        where TInitializer : class, IInitializer;

    void AddOptions<TOptions>(string configurationSection, Func<TOptions, bool>? validate = null)
        where TOptions : class, new();

    TOptions GetOptions<TOptions>()
        where TOptions : class, new();

    public TService GetService<TService>()
        where TService : notnull;

    void Build();
}
