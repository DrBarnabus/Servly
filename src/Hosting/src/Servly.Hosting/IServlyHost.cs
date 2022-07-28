using Microsoft.Extensions.Hosting;

namespace Servly.Hosting;

public interface IServlyHost : IHost, IAsyncDisposable
{
    /// <summary>
    /// Attempts to gracefully stop the host within the given timeout.
    /// </summary>
    /// <param name="timeout">The timeout for stopping gracefully. Once expired the server
    /// may terminate any active connections.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task StopAsync(TimeSpan timeout);

    /// <summary>
    /// Returns a <see cref="Task"/> that completes when shutdown is triggered via the given token or shutdown occurs.
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to trigger a shutdown.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task WaitForShutdownAsync(CancellationToken token = default);

    /// <summary>
    /// Runs the application and returns a <see cref="Task"/> that only completes once the token is triggered or shutdown occurs.
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to trigger a shutdown.</param>
    /// <returns></returns>
    Task RunAsync(CancellationToken token = default);
}
