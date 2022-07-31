using System.Reflection;
using Servly.Core.Exceptions;

namespace Servly.Core;

public static class Utilities
{
    /// <summary>
    ///     Returns the name of the entry assembly or the assembly that has been passed in.
    /// </summary>
    /// <param name="assembly">The assembly to return the name of, if not specified `GetEntryAssembly` is used instead.</param>
    /// <returns>The simple name of the assembly.</returns>
    /// <exception cref="ServlyException">In the event the assembly's name can't be located.</exception>
    public static string GetAssemblyName(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly();
        return assembly?.GetName().Name ?? throw new ServlyException("Unable to locate Assembly's Name.");
    }

    /// <summary>
    ///     Returns the value of <see cref="AssemblyInformationalVersionAttribute" /> applied to the entry assembly or the assembly that has been passed in.
    /// </summary>
    /// <param name="assembly">The assembly to return the informational version of, if not specified `GetEntryAssembly` is used instead.</param>
    /// <returns>The value of the assembly's informational version or "unknown" in the event it can't be located.</returns>
    public static string GetAssemblyInformationalVersion(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly();
        var attribute = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        return attribute?.InformationalVersion.Insert(0, "v") ?? "unknown";
    }
}
