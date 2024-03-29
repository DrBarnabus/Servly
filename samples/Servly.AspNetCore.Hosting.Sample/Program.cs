using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Servly.Extensions;
using Servly.Hosting;

var hostBuilder = ServlyHost.CreateDefaultBuilder(args)
    .ConfigureInternalHost(internalBuilder => internalBuilder
        .ConfigureLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        })
    )
    .ConfigureServices((_, services) =>
    {
        services
            .AddRouting();
    })
    .ConfigureWebHost(builder => builder
        .Configure(app => app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", ctx => ctx.Response.WriteAsync("Hello World!"));
            })
        )
    );

await hostBuilder.Build().RunAsync();
