using System.Diagnostics.CodeAnalysis;

namespace Servly.Core.Implementations;

[ExcludeFromCodeCoverage]
public abstract class ChainedServlyBuilder<TBuilder> : IChainedServlyBuilder<TBuilder>
    where TBuilder : IChainedServlyBuilder<TBuilder>
{
    public IServlyBuilder BaseBuilder { get; }

    protected ChainedServlyBuilder(IServlyBuilder baseBuilder)
    {
        BaseBuilder = baseBuilder;
    }
}
