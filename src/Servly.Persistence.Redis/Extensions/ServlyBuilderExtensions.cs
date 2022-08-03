using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Servly.Core;
using Servly.Core.Exceptions;
using Servly.Persistence.Redis;
using Servly.Persistence.Redis.Implementations;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ServlyBuilderExtensions
{
    private const string RedisProviderModuleName = "Persistence.Redis";
    private const string ConfigurationSectionKey = "Servly:Redis";

    public static IRedisProviderBuilder AddRedisProvider(this IServlyBuilder builder, string instanceName = "Default")
    {
        if (!builder.TryRegisterModule(RedisProviderModuleName))
        {
            builder.Services
                .AddSingleton<IRedisProviderResolver, RedisProviderResolver>();
        }

        string instanceModuleName = $"{RedisProviderModuleName}:[{instanceName.ToLowerInvariant()}]";
        if (builder.TryRegisterModule(instanceModuleName))
            throw new ModuleAlreadyRegisteredException(instanceModuleName);

        string sectionKey = $"{ConfigurationSectionKey}:{instanceName}";
        builder.AddOptions<RedisProviderOptions>(sectionKey, instanceName);

        builder.Services
            .AddSingleton<IRedisProviderInstance>(sp =>
            {
                var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<RedisProviderOptions>>();
                return new RedisProviderInstance(optionsSnapshot, instanceName);
            });

        return new RedisProviderBuilder(builder, instanceName);
    }

    [Obsolete($"{nameof(AddRedis)} is deprecated, use {nameof(AddRedisProvider)} instead.")]
    public static IServlyBuilder AddRedis(this IServlyBuilder builder,
        string configurationSectionName = ConfigurationSectionKey, bool distributedCache = true)
    {
        if (builder.TryRegisterModule(RedisProviderModuleName))
            return builder;

        builder.AddOptions<RedisOptions>(configurationSectionName);
        var redisOptions = builder.GetOptions<RedisOptions>();

        builder.Services
            .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisOptions.ConnectionString))
            .AddSingleton<IRedisProvider, RedisProvider>();

        if (distributedCache)
            builder.Services.AddStackExchangeRedisCache(cacheOptions =>
            {
                cacheOptions.Configuration = redisOptions.ConnectionString;
                cacheOptions.InstanceName = redisOptions.InstanceName;
            });

        return builder;
    }
}
