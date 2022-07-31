using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;

namespace Servly.AspNetCore.ModelBinding.Hybrid;

public class HybridMvcConfigureOptions : IConfigureOptions<MvcOptions>
{
    public void Configure(MvcOptions options)
    {
        var providers = options.ModelBinderProviders;

        var bodyProvider = providers.Single(p =>
            p.GetType() == typeof(BodyModelBinderProvider)) as BodyModelBinderProvider;
        var complexProvider = providers.Single(p =>
            p.GetType() == typeof(ComplexObjectModelBinderProvider)) as ComplexObjectModelBinderProvider;

        providers.Insert(0, new HybridModelBinderProvider(bodyProvider!, complexProvider!));
    }
}
