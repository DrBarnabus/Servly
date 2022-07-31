namespace Servly.Core.Services.Implementations;

/// <summary>
///     Provides access to the system clock for consumers of <see cref="IClock" />.
/// </summary>
public class SystemClock : IClock
{
    /// <summary>
    ///     Retrieves the current system time in UTC.
    /// </summary>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
