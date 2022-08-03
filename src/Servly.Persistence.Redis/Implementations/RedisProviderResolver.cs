using Servly.Core.Exceptions;
using StackExchange.Redis;

namespace Servly.Persistence.Redis.Implementations;

internal class RedisProviderResolver : IRedisProviderResolver
{
    private readonly IEnumerable<IRedisProviderInstance> _instances;

    public RedisProviderResolver(IEnumerable<IRedisProviderInstance> instances)
    {
        _instances = instances;
    }

    public IDatabase Connect(string instanceName)
    {
        var instance = ResolveInstance(instanceName);
        return instance.Connect();
    }

    public async Task<IDatabase> ConnectAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        var instance = ResolveInstance(instanceName);
        return await instance.ConnectAsync(cancellationToken).ConfigureAwait(false);
    }

    private IRedisProviderInstance ResolveInstance(string instanceName)
    {
        foreach (var instance in _instances)
        {
            if (instance.InstanceName.Equals(instanceName))
                return instance;
        }

        throw new ServlyException($"Unable to resolve instance named '{instanceName}'");
    }
}
