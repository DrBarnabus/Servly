using Microsoft.Extensions.DependencyInjection;
using Servly.Core;
using StackExchange.Redis;

namespace Servly.Persistence.Redis.Extensions;

public static class ServlyBuilderExtensions
{
    private const string RedisProviderModuleName = "Persistence.Redis";
    private const string DefaultSectionName = "Servly:Redis";

    public static IServlyBuilder AddRedis(this IServlyBuilder builder, string configurationSectionName = DefaultSectionName, bool distributedCache = true)
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
