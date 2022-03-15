using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Servly.Core.Implementations;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

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
