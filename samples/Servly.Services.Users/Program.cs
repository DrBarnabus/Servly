using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Servly.Core;
using Servly.Core.Extensions;
using Servly.Core.Options;
using System.Threading.Tasks;

namespace Servly.Services.Users
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IServlyHostBuilder CreateHostBuilder(string[] args)
        {
            return ServlyHost.CreateDefaultBuilder(args)
                .ConfigureServly(builder => builder.AddServiceId())
                .ConfigureServices(services => services.AddRouting())
                .Configure(app => app
                    .UseRouting()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", async context =>
                        {
                            var servlyOptions = context.RequestServices.GetRequiredService<IOptions<ServlyOptions>>();
                            await context.Response.WriteAsync($"Hello World from {servlyOptions.Value.ServiceName}!");
                        });
                    }));
        }
    }
}
