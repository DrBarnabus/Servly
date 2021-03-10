// Copyright (c) 2021 DrBarnabus

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Servly.Core.Internal;
using System.Threading.Tasks;

namespace Servly.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseServly(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var startupInitializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
            Task.Run(() => startupInitializer.InitializeAsync()).GetAwaiter().GetResult();

            return app;
        }
    }
}
