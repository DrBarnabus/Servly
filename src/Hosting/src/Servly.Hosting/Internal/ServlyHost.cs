using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Servly.Hosting.Internal;

internal class ServlyHost : IServlyHost
{
    private readonly ApplicationLifetime _applicationLifetime;
    private readonly ILogger<ServlyHost> _logger;
    private readonly IHostLifetime _hostLifetime;
    private readonly HostOptions _options;
    private IEnumerable<IHostedService>? _hostedServices;

    public ServlyHost(IServiceProvider services, IHostApplicationLifetime applicationLifetime, ILogger<ServlyHost> logger,
        IHostLifetime hostLifetime, IOptions<HostOptions> options)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        _applicationLifetime = (applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime)))
            as ApplicationLifetime ?? throw new ArgumentException("Replacing IHostApplicationLifetime is not supported.", nameof(applicationLifetime));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public IServiceProvider Services { get; }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Host Starting...");

        using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
        var combinedCancellationToken = combinedCancellationTokenSource.Token;

        await _hostLifetime.WaitForStartAsync(combinedCancellationToken).ConfigureAwait(false);

        combinedCancellationToken.ThrowIfCancellationRequested();
        _hostedServices = Services.GetService<IEnumerable<IHostedService>>();

        if (_hostedServices is not null)
            foreach (var hostedService in _hostedServices)
                await hostedService.StartAsync(combinedCancellationToken).ConfigureAwait(false);

        _applicationLifetime.NotifyStarted();

        _logger.LogDebug("Host Started.");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Host Stopping...");

        using var cts = new CancellationTokenSource(_options.ShutdownTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        var token = linkedCts.Token;
        _applicationLifetime.StopApplication();

        IList<Exception> exceptions = new List<Exception>();
        if (_hostedServices is not null)
        {
            foreach (var hostedService in _hostedServices.Reverse())
            {
                try
                {
                    await hostedService.StopAsync(token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        _applicationLifetime.NotifyStopped();

        try
        {
            await _hostLifetime.StopAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
        }

        if (exceptions.Count > 0)
        {
            var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
            _logger.LogDebug(ex, "Host stopped with Exception.");
            throw ex;
        }

        _logger.LogDebug("Host Stopping.");
    }

    /// <inheritdoc />
    public async Task StopAsync(TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        await StopAsync(cts.Token).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task WaitForShutdownAsync(CancellationToken token = default)
    {
        var applicationLifetime = Services.GetService<IHostApplicationLifetime>();

        token.Register(state => ((IHostApplicationLifetime?)state)?.StopApplication(), applicationLifetime);

        var waitForStop = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        applicationLifetime?.ApplicationStopped.Register(state =>
            ((TaskCompletionSource<object?>?) state)?.TrySetResult(null), waitForStop);

        await waitForStop.Task.ConfigureAwait(false);

        await StopAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken token = default)
    {
        try
        {
            await StartAsync(token).ConfigureAwait(false);
            await WaitForShutdownAsync(token).ConfigureAwait(false);
        }
        finally
        {
            await DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        switch (Services)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
}
