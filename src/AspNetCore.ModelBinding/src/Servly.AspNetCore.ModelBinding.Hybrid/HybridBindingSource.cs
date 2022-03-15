using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Servly.AspNetCore.ModelBinding.Hybrid;

public class HybridBindingSource : BindingSource
{
    public static readonly BindingSource Hybrid = new HybridBindingSource(nameof(Hybrid), nameof(Hybrid), true, true);

    public HybridBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest)
        : base(id, displayName, isGreedy, isFromRequest)
    {
    }

    public override bool CanAcceptDataFrom(BindingSource bindingSource)
    {
        return bindingSource == Body || bindingSource == this;
    }
}
