using Microsoft.Extensions.DependencyInjection;
using Servly.Core.Exceptions;
using Servly.Persistence.Redis;
using Servly.Persistence.Redis.Implementations;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class RedisProviderBuilderExtensions
{
    public static IRedisProviderBuilder UseAsDistributedCache(this IRedisProviderBuilder builder)
    {
        if (builder is not RedisProviderBuilder redisProviderBuilder)
            throw new ChainedServlyBuilderTypeException(typeof(RedisProviderBuilder), builder.GetType());

        var redisProviderOptions = builder.GetOptions<RedisProviderOptions>(redisProviderBuilder.InstanceName);

        builder.Services
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisProviderOptions.Configuration;
                options.InstanceName = redisProviderOptions.InstanceName;
            });

        redisProviderBuilder.IsDistributedCache = true;
        return redisProviderBuilder;
    }
}
