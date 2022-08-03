using Servly.Core;
using Servly.Core.Implementations;

namespace Servly.Persistence.Redis.Implementations;

internal sealed class RedisProviderBuilder : ChainedServlyBuilder<IRedisProviderBuilder>, IRedisProviderBuilder
{
    public string InstanceName { get; }

    public bool IsDistributedCache { get; internal set; }

    public RedisProviderBuilder(IServlyBuilder baseBuilder, string instanceName)
        : base(baseBuilder)
    {
        InstanceName = instanceName;
    }
}
