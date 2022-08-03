using StackExchange.Redis;

namespace Servly.Persistence.Redis;

public class RedisProviderOptions
{
    public string Configuration { get; set; } = "localhost";

    public ConfigurationOptions? ConfigurationOptions { get; set; }

    public string? InstanceName { get; set; }
}
