// Copyright (c) 2021 DrBarnabus

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servly.Core
{
    public interface IServlyHost : IDisposable, IAsyncDisposable
    {
        IServiceProvider Services { get; }

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);

        Task StopAsync(TimeSpan timeout);

        Task WaitForShutdownAsync(CancellationToken cancellationToken = default);

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}
