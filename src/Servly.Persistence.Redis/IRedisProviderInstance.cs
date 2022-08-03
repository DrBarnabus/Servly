using StackExchange.Redis;

namespace Servly.Persistence.Redis;

public interface IRedisProviderInstance
{
    string InstanceName { get; }

    IDatabase Connect();

    Task<IDatabase> ConnectAsync(CancellationToken cancellationToken = default);
}
