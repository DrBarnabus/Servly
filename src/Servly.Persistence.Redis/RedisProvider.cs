using StackExchange.Redis;

namespace Servly.Persistence.Redis;

public sealed class RedisProvider : IRedisProvider
{
    public RedisProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        ConnectionMultiplexer = connectionMultiplexer;
    }

    public IConnectionMultiplexer ConnectionMultiplexer { get; }
}
