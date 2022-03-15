using Microsoft.Extensions.DependencyInjection;
using Servly.AspNetCore.ModelBinding.Hybrid;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddHybridModelBinder(this IMvcBuilder builder)
    {
        builder.Services.ConfigureOptions<HybridMvcConfigureOptions>();
        return builder;
    }
}
