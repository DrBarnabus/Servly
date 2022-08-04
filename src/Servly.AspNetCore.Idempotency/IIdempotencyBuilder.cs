using Servly.Core;

namespace Servly.AspNetCore.Idempotency;

public interface IIdempotencyBuilder : IChainedServlyBuilder<IIdempotencyBuilder>
{
}
