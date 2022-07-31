namespace Servly.Core.Services;

/// <summary>
///     Provides an abstraction of the system clock to facilitate unit testing.
/// </summary>
public interface IClock
{
    /// <summary>
    ///     Retrieves the current time in UTC.
    /// </summary>
    public DateTimeOffset UtcNow { get; }
}
