using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Servly.Core;
using Servly.Core.Exceptions;
using Servly.Persistence.Redis;
using Servly.Persistence.Redis.Implementations;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ServlyBuilderExtensions
{
    public static IRedisProviderBuilder AddRedisProvider(this IServlyBuilder builder, string instanceName = Constants.DefaultInstanceName)
    {
        if (!builder.TryRegisterModule(Constants.ModuleName))
        {
            builder.Services
                .AddSingleton<IRedisProviderResolver, RedisProviderResolver>();
        }

        string instanceModuleName = $"{Constants.ModuleName}:[{instanceName.ToLowerInvariant()}]";
        if (builder.TryRegisterModule(instanceModuleName))
            throw new ModuleAlreadyRegisteredException(instanceModuleName);

        string sectionKey = $"{Constants.ConfigurationSectionKey}:{instanceName}";
        builder.AddOptions<RedisProviderOptions>(sectionKey, instanceName);

        builder.Services
            .AddSingleton<IRedisProviderInstance>(sp =>
            {
                var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<RedisProviderOptions>>();
                return new RedisProviderInstance(optionsSnapshot, instanceName);
            });

        return new RedisProviderBuilder(builder, instanceName);
    }
}
