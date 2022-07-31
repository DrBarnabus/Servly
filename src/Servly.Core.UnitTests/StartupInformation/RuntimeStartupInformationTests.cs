using System.Runtime.InteropServices;
using Servly.Core.StartupInformation;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.StartupInformation;

public class RuntimeStartupInformationTests
{
    [Fact]
    public void ShouldContainCorrectSectionTitle()
    {
        var rsi = new RuntimeStartupInformation();
        rsi.SectionTitle.ShouldBe("Runtime");
    }

    [Fact]
    public void ShouldContainCorrectKeyCount()
    {
        var rsi = new RuntimeStartupInformation();
        rsi.Values.Count.ShouldBe(4);
    }

    [Fact]
    public void ShouldContainCorrectProcessorArchitecture()
    {
        var rsi = new RuntimeStartupInformation();
        rsi.Values.ShouldContainKeyAndValue("Processor Architecture", RuntimeInformation.ProcessArchitecture.ToString());
    }

    [Fact]
    public void ShouldContainCorrectOperatingSystem()
    {
        var rsi = new RuntimeStartupInformation();
        rsi.Values.ShouldContainKeyAndValue("Operating System", $"{RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
    }

    [Fact]
    public void ShouldContainCorrectFrameworkDescription()
    {
        var rsi = new RuntimeStartupInformation();
        rsi.Values.ShouldContainKeyAndValue("Framework Description", RuntimeInformation.FrameworkDescription);
    }

    [Fact]
    public void ShouldContainCorrectRuntimeIdentifier()
    {
        var rsi = new RuntimeStartupInformation();
        rsi.Values.ShouldContainKeyAndValue("Runtime Identifier", RuntimeInformation.RuntimeIdentifier);
    }
}
