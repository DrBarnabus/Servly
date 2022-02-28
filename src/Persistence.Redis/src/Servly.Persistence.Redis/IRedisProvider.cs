using StackExchange.Redis;

namespace Servly.Persistence.Redis;

public interface IRedisProvider
{
    public IConnectionMultiplexer ConnectionMultiplexer { get; }
}
