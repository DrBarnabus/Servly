using Servly.Hosting;

var hostBuilder = ServlyHost.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddRouting();
    })
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
    });

await hostBuilder.Build().RunAsync();
