using Microsoft.Extensions.DependencyInjection;
using Servly.AspNetCore.Idempotency;
using Servly.AspNetCore.Idempotency.Implementations;
using Servly.AspNetCore.Idempotency.Providers;
using Servly.AspNetCore.Idempotency.Redis.Implementations;
using Servly.Core.Exceptions;
using Servly.Persistence.Redis;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class IdempotencyBuilderExtensions
{
    public static IIdempotencyBuilder UseRedisPersistence(this IIdempotencyBuilder builder, string instanceName = "Default")
    {
        if (builder is not IdempotencyBuilder redisProviderBuilder)
            throw new ChainedServlyBuilderTypeException(typeof(IdempotencyBuilder), builder.GetType());

        builder.Services
            .AddSingleton<IIdempotencyPersistenceProvider>(sp =>
            {
                var redisProviderResolver = sp.GetRequiredService<IRedisProviderResolver>();
                return new RedisIdempotencyPersistenceProvider(instanceName, redisProviderResolver);
            });

        redisProviderBuilder.HasPersistenceProvider = true;
        return builder;
    }
}
