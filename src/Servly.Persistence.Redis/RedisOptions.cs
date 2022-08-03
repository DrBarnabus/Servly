namespace Servly.Persistence.Redis;

[Obsolete($"{nameof(RedisOptions)} is deprecated, use {nameof(RedisProviderOptions)} instead.")]
public class RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
    public string? InstanceName { get; set; }
    public int Database { get; set; }
}
