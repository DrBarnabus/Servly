using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Servly.Core;

public interface IChainedServlyBuilder<TBuilder> : IServlyBuilder
    where TBuilder : IChainedServlyBuilder<TBuilder>, IServlyBuilder
{
    IServlyBuilder BaseBuilder { get; }

    IServiceCollection IServlyBuilder.Services => BaseBuilder.Services;

    IConfiguration IServlyBuilder.Configuration => BaseBuilder.Configuration;

    bool IServlyBuilder.TryRegisterModule(string moduleName) => BaseBuilder.TryRegisterModule(moduleName);

    bool IServlyBuilder.IsModuleRegistered(string moduleName) => BaseBuilder.IsModuleRegistered(moduleName);

    void IServlyBuilder.AddBuildAction(Action<IServiceCollection> buildAction) =>
        BaseBuilder.AddBuildAction(buildAction);

    void IServlyBuilder.AddInitializer<TInitializer>() => BaseBuilder.AddInitializer<TInitializer>();

    void IServlyBuilder.AddOptions<TOptions>(string sectionKey, string? instanceName, Func<TOptions, bool>? validate) =>
        BaseBuilder.AddOptions(sectionKey, instanceName, validate);

    TOptions IServlyBuilder.GetOptions<TOptions>(string? instanceName) => BaseBuilder.GetOptions<TOptions>(instanceName);

    TService IServlyBuilder.GetService<TService>() => BaseBuilder.GetService<TService>();

    void IServlyBuilder.Build() => BaseBuilder.Build();
}
