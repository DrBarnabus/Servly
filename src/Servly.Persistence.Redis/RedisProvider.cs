using StackExchange.Redis;

namespace Servly.Persistence.Redis;

[Obsolete($"{nameof(RedisProvider)} is deprecated, use {nameof(IRedisProviderResolver)} instead.")]
public sealed class RedisProvider : IRedisProvider
{
    public RedisProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        ConnectionMultiplexer = connectionMultiplexer;
    }

    public IConnectionMultiplexer ConnectionMultiplexer { get; }
}
