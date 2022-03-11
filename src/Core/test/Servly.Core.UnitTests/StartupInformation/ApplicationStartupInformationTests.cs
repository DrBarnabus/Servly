using System.Reflection;
using Servly.Core.StartupInformation;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.StartupInformation;

public class ApplicationStartupInformationTests
{
    [Fact]
    public void ShouldContainCorrectSectionTitle()
    {
        var asi = new ApplicationStartupInformation();
        asi.SectionTitle.ShouldBe("Application");
    }

    [Fact]
    public void ShouldContainCorrectKeyCount()
    {
        var asi = new ApplicationStartupInformation();
        asi.Values.Count.ShouldBe(3);
    }

    [Fact]
    public void ShouldContainCorrectProcessorArchitecture()
    {
        var asi = new ApplicationStartupInformation();
        asi.Values.ShouldContainKeyAndValue("Application Name", Assembly.GetEntryAssembly()?.GetName().Name!);
    }

    [Fact]
    public void ShouldContainCorrectOperatingSystem()
    {
        string expectedValue = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Insert(0, "v") ?? "unknown";

        var asi = new ApplicationStartupInformation();
        asi.Values.ShouldContainKeyAndValue("Application Version", expectedValue);
    }

    [Fact]
    public void ShouldContainCorrectFrameworkDescription()
    {
        string expectedValue = typeof(ApplicationStartupInformation).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Insert(0, "v") ?? "unknown";

        var asi = new ApplicationStartupInformation();
        asi.Values.ShouldContainKeyAndValue("Servly Version", expectedValue);
    }
}
