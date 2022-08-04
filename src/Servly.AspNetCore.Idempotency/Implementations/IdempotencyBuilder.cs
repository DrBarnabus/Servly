using Servly.Core;
using Servly.Core.Implementations;

namespace Servly.AspNetCore.Idempotency.Implementations;

internal sealed class IdempotencyBuilder : ChainedServlyBuilder<IIdempotencyBuilder>, IIdempotencyBuilder
{
    public bool HasPersistenceProvider { get; internal set; }

    public IdempotencyBuilder(IServlyBuilder baseBuilder)
        : base(baseBuilder)
    {
    }
}
