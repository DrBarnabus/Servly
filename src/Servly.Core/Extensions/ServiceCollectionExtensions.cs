using Figgle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Servly.Core;
using Servly.Core.StartupInformation;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ServiceCollectionExtensions
{
    private const string MainConfigurationSection = "Servly";

    public static IServiceCollection AddServly(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IServlyBuilder>? configureServly = null)
    {
        var servlyBuilder = ServlyBuilder.Create(services, configuration);
        servlyBuilder.AddSystemClock();

        servlyBuilder.AddOptions<ServlyOptions>(MainConfigurationSection, validate: options => !string.IsNullOrEmpty(options.ServiceName));

        // Default Startup Information
        servlyBuilder.Services.AddSingleton<IStartupInformation, RuntimeStartupInformation>();
        servlyBuilder.Services.AddSingleton<IStartupInformation, ApplicationStartupInformation>();

        configureServly?.Invoke(servlyBuilder);

        DisplayStartupInformation(servlyBuilder);

        servlyBuilder.Build();
        return services;
    }

    private static void DisplayStartupInformation(IServlyBuilder servlyBuilder)
    {
        var servlyOptions = servlyBuilder.GetOptions<ServlyOptions>();

        string fullWidthSeparatorString = new('=', Console.BufferWidth);

        if (servlyOptions.DisplayStartupBanner && !string.IsNullOrEmpty(servlyOptions.ServiceName))
        {
            Console.WriteLine(fullWidthSeparatorString);
            Console.WriteLine(FiggleFonts.Banner3.Render(servlyOptions.ServiceName));
            Console.Write(fullWidthSeparatorString);
        }

        if (!servlyOptions.DisplayStartupInformation)
            return;

        if (!servlyOptions.DisplayStartupBanner)
            Console.Write(fullWidthSeparatorString);

        var startupInformation = servlyBuilder.GetService<IEnumerable<IStartupInformation>>().ToList();

        foreach (var startupInfo in startupInformation)
        {
            Console.WriteLine(startupInfo.SectionTitle);
            foreach ((string key, string value) in startupInfo.Values.OrderBy(v => v.Key))
                Console.WriteLine($"    {key}: {value}");
        }

        Console.Write(fullWidthSeparatorString);
    }
}
