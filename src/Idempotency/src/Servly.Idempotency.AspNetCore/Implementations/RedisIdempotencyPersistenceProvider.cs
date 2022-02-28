using System.IO.Compression;
using System.Text.Json;
using Servly.Persistence.Redis;

namespace Servly.Idempotency.AspNetCore.Implementations;

public class RedisIdempotencyPersistenceProvider : IIdempotencyPersistenceProvider
{
    private readonly IRedisProvider _redisProvider;

    public RedisIdempotencyPersistenceProvider(IRedisProvider redisProvider)
    {
        _redisProvider = redisProvider;
    }

    /// <inheritdoc />
    public async ValueTask WriteIdempotencyData(string key, IdempotencyData idempotencyData, CancellationToken cancellationToken = default)
    {
        await using var resultStream = new MemoryStream();
        await using (var brotliStream = new BrotliStream(resultStream, CompressionLevel.Optimal))
            await JsonSerializer.SerializeAsync(brotliStream, idempotencyData, cancellationToken: cancellationToken);

        var db = _redisProvider.ConnectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, resultStream.ToArray(), TimeSpan.FromHours(2));
    }

    /// <inheritdoc />
    public async ValueTask<IdempotencyData?> ReadIdempotencyData(string key, CancellationToken cancellationToken = default)
    {
        var db = _redisProvider.ConnectionMultiplexer.GetDatabase();
        var redisValue = await db.StringGetAsync(key);
        if (redisValue.IsNull)
            return null;

        await using var inputStream = new MemoryStream((byte[])redisValue);
        await using var decompressor = new BrotliStream(inputStream, CompressionMode.Decompress);
        await using var outputStream = new MemoryStream();

        await decompressor.CopyToAsync(outputStream, cancellationToken);
        outputStream.Position = 0;

        return await JsonSerializer.DeserializeAsync<IdempotencyData>(outputStream, cancellationToken: cancellationToken);
    }
}
