namespace Servly.Idempotency.AspNetCore;

public interface IIdempotencyPersistenceProvider
{
    /// <summary>
    ///     Returns the <see cref="IdempotencyData"/> object associated with the provided key.
    /// </summary>
    /// <param name="key">The storage key that should be read from.</param>
    /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the write operation.</param>
    /// <returns>The <see cref="IdempotencyData"/> object from the persistence provider or null if not found.</returns>
    ValueTask<IdempotencyData?> ReadIdempotencyData(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Saves the provided <see cref="IdempotencyData"/> into the persistence provider under the provided key.
    /// </summary>
    /// <param name="key">The storage key that should be written to.</param>
    /// <param name="idempotencyData">The <see cref="IdempotencyData"/> that should be written.</param>
    /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the write operation.</param>
    ValueTask WriteIdempotencyData(string key, IdempotencyData idempotencyData, CancellationToken cancellationToken = default);
}
