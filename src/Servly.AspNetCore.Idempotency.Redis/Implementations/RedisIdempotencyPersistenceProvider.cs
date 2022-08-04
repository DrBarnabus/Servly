using System.IO.Compression;
using System.Text.Json;
using Servly.AspNetCore.Idempotency.Providers;
using Servly.Persistence.Redis;
using StackExchange.Redis;

namespace Servly.AspNetCore.Idempotency.Redis.Implementations;

internal sealed class RedisIdempotencyPersistenceProvider : IIdempotencyPersistenceProvider
{
    private const string DataKey = "data";
    private const long NotPresent = -1;

    // KEYS[1] = key
    // ARGV[1] = data (byte[])
    // ARGV[2] = relative-expiration (long, in seconds, -1 for none)
    private const string SetScript = @"
        redis.call('HSET', KEYS[1], 'data', ARGV[1])
        if ARGV[2] ~= '-1' then
            redis.call('EXPIRE', KEYS[1], ARGV[2])
        end
        return 1";

    private readonly string _redisInstanceName;
    private readonly IRedisProviderResolver _redisProviderResolver;
    private IDatabase? _database;

    public RedisIdempotencyPersistenceProvider(string redisInstanceName, IRedisProviderResolver redisProviderResolver)
    {
        _redisInstanceName = redisInstanceName;
        _redisProviderResolver = redisProviderResolver;
    }

    /// <inheritdoc />
    public async ValueTask<IdempotencyData?> ReadIdempotencyData(string key, CancellationToken cancellationToken = default)
    {
        var database = await ConnectAsync(cancellationToken);

        var results = await database.HashGetAsync(key, new RedisValue[]
            {
                DataKey
            })
            .ConfigureAwait(false);

        if (results is null || results.Length != 1)
            return null;

        await using var inputStream = new MemoryStream((byte[])results[0]);
        await using var decompressor = new BrotliStream(inputStream, CompressionMode.Decompress);
        await using var outputStream = new MemoryStream();

        await decompressor.CopyToAsync(outputStream, cancellationToken);
        outputStream.Position = 0;

        return await JsonSerializer.DeserializeAsync<IdempotencyData>(outputStream, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask WriteIdempotencyData(string key, IdempotencyData idempotencyData, CancellationToken cancellationToken = default)
    {
        var database = await ConnectAsync(cancellationToken);

        await using var resultStream = new MemoryStream();
        await using (var compressor = new BrotliStream(resultStream, CompressionLevel.Fastest))
            await JsonSerializer.SerializeAsync(compressor, idempotencyData, cancellationToken: cancellationToken);

        var creationTime = DateTimeOffset.UtcNow;
        var absoluteExpiration = creationTime + TimeSpan.FromHours(2);
        long expirationInSeconds = (long)(absoluteExpiration - creationTime).TotalSeconds;

        await database.ScriptEvaluateAsync(SetScript, new RedisKey[] {key},
                new RedisValue[]
                {
                    resultStream.ToArray(),
                    expirationInSeconds
                })
            .ConfigureAwait(false);
    }

    private async ValueTask<IDatabase> ConnectAsync(CancellationToken cancellationToken)
    {
        if (_database is not null)
            return _database;

        _database = await _redisProviderResolver.ConnectAsync(_redisInstanceName, cancellationToken);
        return _database;
    }
}
