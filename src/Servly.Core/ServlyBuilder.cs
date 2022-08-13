using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Servly.Core;

[ExcludeFromCodeCoverage]
public static class ServlyBuilder
{
    public static IServlyBuilder Create(IServiceCollection services, IConfiguration configuration)
    {
        return new Implementations.ServlyBuilder(services, configuration);
    }
}
