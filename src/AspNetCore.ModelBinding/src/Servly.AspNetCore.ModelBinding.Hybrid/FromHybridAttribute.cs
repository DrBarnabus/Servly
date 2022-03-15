using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Servly.AspNetCore.ModelBinding.Hybrid;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class FromHybridAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource? BindingSource => HybridBindingSource.Hybrid;
}
