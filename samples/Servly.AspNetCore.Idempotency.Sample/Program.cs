using Servly.AspNetCore.Idempotency.Attributes;
using Servly.Extensions;
using Servly.Hosting;

var hostBuilder = ServlyHost.CreateDefaultBuilder(args)
    .ConfigureInternalHost(internalBuilder => internalBuilder
        .ConfigureLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        }))
    .ConfigureModules((_, builder) => builder
        .AddRedisProvider("Idempotency")
        .AddIdempotencyMiddleware()
            .UseRedisPersistence("Idempotency"))
    .ConfigureServices((_, services) => { services.AddRouting(); })
    .ConfigureWebHost(builder => builder
        .Configure(app => app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/", async ctx =>
                    {
                        var echoBody = await ctx.Request.ReadFromJsonAsync<List<string>>();
                        await ctx.Response.WriteAsJsonAsync(echoBody);
                    })
                    .WithMetadata(new IdempotentAttribute("ExamplePrefix"));
            })));

await hostBuilder.Build().RunAsync();
