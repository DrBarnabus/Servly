namespace Servly.Idempotency.AspNetCore.FunctionalTests;

public class MockIdempotencyPersistenceProvider : IIdempotencyPersistenceProvider
{
    public Dictionary<string, IdempotencyData> Data { get; } = new();

    public ValueTask<IdempotencyData?> ReadIdempotencyData(string key, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(Data.TryGetValue(key, out var value) ? value : null);
    }

    public ValueTask WriteIdempotencyData(string key, IdempotencyData idempotencyData,
        CancellationToken cancellationToken = default)
    {
        if (!Data.TryAdd(key, idempotencyData))
            Data[key] = idempotencyData;

        return ValueTask.CompletedTask;
    }
}
