using Servly.Core.Services.Implementations;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Services.Implementations;

public class SystemClockTests
{
    [Fact]
    public void ShouldReturnCurrentTime()
    {
        var subject = new SystemClock();
        subject.UtcNow.ShouldBeGreaterThan(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromSeconds(1)));
        subject.UtcNow.ShouldBeLessThan(DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(1)));
    }
}
