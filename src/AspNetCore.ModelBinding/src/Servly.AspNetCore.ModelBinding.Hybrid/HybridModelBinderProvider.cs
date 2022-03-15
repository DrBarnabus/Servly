using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Servly.AspNetCore.ModelBinding.Hybrid;

public class HybridModelBinderProvider : IModelBinderProvider
{
    private readonly BodyModelBinderProvider _bodyModelBinderProvider;
    private readonly ComplexObjectModelBinderProvider _complexObjectModelBinderProvider;

    public HybridModelBinderProvider(BodyModelBinderProvider bodyModelBinderProvider, ComplexObjectModelBinderProvider complexObjectModelBinderProvider)
    {
        _bodyModelBinderProvider = bodyModelBinderProvider;
        _complexObjectModelBinderProvider = complexObjectModelBinderProvider;
    }

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var bodyModelBinder = _bodyModelBinderProvider.GetBinder(context)!;
        var complexObjectModelBinder = _complexObjectModelBinderProvider.GetBinder(context)!;

        var bindingSource = context.BindingInfo.BindingSource;

        if (bindingSource is not null && bindingSource.CanAcceptDataFrom(HybridBindingSource.Hybrid))
            return new HybridModelBinder(bodyModelBinder, complexObjectModelBinder);

        return null;
    }
}
