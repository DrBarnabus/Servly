using StackExchange.Redis;

namespace Servly.Persistence.Redis;

public interface IRedisProviderResolver
{
    IDatabase Connect(string instanceName);

    Task<IDatabase> ConnectAsync(string instanceName, CancellationToken cancellationToken = default);
}
