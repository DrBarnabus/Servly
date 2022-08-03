using Microsoft.Extensions.Options;
using Servly.Core;
using StackExchange.Redis;

namespace Servly.Persistence.Redis.Implementations;

internal sealed class RedisProviderInstance : IRedisProviderInstance, IDisposable, IAsyncDisposable
{
    private readonly RedisProviderOptions _options;
    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);

    private IConnectionMultiplexer? _connection;
    private IDatabase? _database;
    private bool _disposed;

    public RedisProviderInstance(IOptionsSnapshot<RedisProviderOptions> options, string instanceName)
    {
        Guard.Assert(options is not null, $"{nameof(options)} cannot be null");
        Guard.Assert(instanceName is not null, $"{nameof(instanceName)} cannot be null");

        _options = options.Get(instanceName);
        InstanceName = instanceName;
    }

    public string InstanceName { get; }

    public IDatabase Connect()
    {
        CheckDisposed();

        if (_database is not null)
            return _database;

        _connectionSemaphore.Wait();
        try
        {
            if (_database == null)
            {
                _connection = _options.ConfigurationOptions is not null
                    ? ConnectionMultiplexer.Connect(_options.ConfigurationOptions)
                    : ConnectionMultiplexer.Connect(_options.Configuration);

                _database = _connection.GetDatabase();
            }
        }
        finally
        {
            _connectionSemaphore.Release();
        }

        return _database;
    }

    public async Task<IDatabase> ConnectAsync(CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (_database is not null)
            return _database;

        await _connectionSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_database == null)
            {
                if (_options.ConfigurationOptions is not null)
                    _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions).ConfigureAwait(false);
                else
                    _connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration).ConfigureAwait(false);

                _database = _connection.GetDatabase();
            }
        }
        finally
        {
            _connectionSemaphore.Release();
        }

        return _database;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _connection?.Close();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        if (_connection is not null)
            await _connection.CloseAsync();
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }
}
