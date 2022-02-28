using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Servly.Persistence.Redis;

public static class Extensions
{
    private const string SectionName = "Redis";

    public static IServiceCollection AddRedisProvider(this IServiceCollection services, IConfiguration configuration,
        string sectionName = SectionName, bool distributedCache = true)
    {
        var options = new RedisOptions();
        configuration.GetSection(sectionName).Bind(options);

        services
            .Configure<RedisOptions>(configuration.GetSection(sectionName))
            .AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(options.ConnectionString))
            .AddSingleton<IRedisProvider, RedisProvider>();

        if (distributedCache)
            services.AddStackExchangeRedisCache(cacheOptions =>
            {
                cacheOptions.Configuration = options.ConnectionString;
                cacheOptions.InstanceName = options.InstanceName;
            });

        return services;
    }
}
