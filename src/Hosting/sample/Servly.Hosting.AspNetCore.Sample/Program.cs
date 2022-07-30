using Servly.Hosting;

var hostBuilder = ServlyHost.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services
            .AddRouting();
    })
    .ConfigureInternalHost(internalBuilder => internalBuilder
        .ConfigureWebHostDefaults(builder =>
        {
            builder
                .Configure(app => app
                    .UseRouting()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", ctx => ctx.Response.WriteAsync("Hello World!"));
                    }));
        })
        .ConfigureLogging(builder =>
        {
            builder.AddConsole();
        })
    );

await hostBuilder.Build().RunAsync();
