namespace Servly.Persistence.Redis;

public class RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
    public string? InstanceName { get; set; }
    public int Database { get; set; }
}
