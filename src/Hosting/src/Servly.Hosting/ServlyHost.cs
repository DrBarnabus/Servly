using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Servly.Hosting.Extensions;
using Servly.Hosting.Internal;

namespace Servly.Hosting;

public static class ServlyHost
{
    public static IServlyHostBuilder CreateDefaultBuilder(string[]? args = null)
    {
        var builder = new ServlyHostBuilder();

        return builder
            .ConfigureInternalHost(hostBuilder => hostBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables("DOTNET_");
                    if (args is not null)
                        config.AddCommandLine(args);
                })
                .ConfigureLogging(logging => logging.ClearProviders())
                .UseDefaultServiceProvider((context, options) =>
                {
                    bool isLocal = context.HostingEnvironment.IsLocal();
                    options.ValidateScopes = isLocal;
                    options.ValidateOnBuild = isLocal;
                })
            )
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                    .AddJsonFile("appsettings.override.json", true, true);

                if (env.IsLocal() && !string.IsNullOrEmpty(env.ApplicationName))
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    config.AddUserSecrets(appAssembly, true);
                }

                config.AddEnvironmentVariables();

                if (args is not null)
                    config.AddCommandLine(args);
            });
    }
}
