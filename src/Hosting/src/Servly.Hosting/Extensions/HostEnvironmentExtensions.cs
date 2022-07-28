using Microsoft.Extensions.Hosting;
using Servly.Core;

namespace Servly.Hosting.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsLocal(this IHostEnvironment env)
    {
        Guard.Assert(env is not null, $"{nameof(env)} cannot be null");
        return env.IsEnvironment("Local");
    }
}
