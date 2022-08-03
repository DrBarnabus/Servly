using StackExchange.Redis;

namespace Servly.Persistence.Redis;

[Obsolete($"{nameof(RedisProvider)} is deprecated, use {nameof(IRedisProviderResolver)} instead.")]
public interface IRedisProvider
{
    public IConnectionMultiplexer ConnectionMultiplexer { get; }
}
