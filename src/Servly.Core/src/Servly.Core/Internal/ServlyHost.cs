// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servly.Core.Internal
{
    internal class ServlyHost : IServlyHost
    {
        private readonly IHost _host;

        public ServlyHost(IHost host)
        {
            _host = host;
        }

        public IServiceProvider Services => _host.Services;

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _host.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _host.StopAsync(cancellationToken);
        }

        public async Task StopAsync(TimeSpan timeout)
        {
            await _host.StopAsync(timeout);
        }

        public async Task WaitForShutdownAsync(CancellationToken cancellationToken = default)
        {
            await _host.WaitForShutdownAsync(cancellationToken);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await StartAsync(cancellationToken).ConfigureAwait(false);

                await WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);
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
            switch (_host)
            {
            // ReSharper disable once SuspiciousTypeConversion.Global
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
            }
        }
    }
}
