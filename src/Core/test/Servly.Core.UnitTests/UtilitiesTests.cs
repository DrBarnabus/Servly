using System.Reflection;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests;

public class UtilitiesTests
{
    [Fact]
    public void GetAssemblyName_ShouldReturnCorrectNameWhenNoAssemblySpecified()
    {
        // Act
        string assemblyName = Utilities.GetAssemblyName();

        // Assert - This assertion can't be hardcoded as it depends on the test runner.
        assemblyName.ShouldBe(Assembly.GetEntryAssembly()?.GetName().Name);
    }

    [Fact]
    public void GetAssemblyName_ShouldReturnCorrectNameWhenAssemblySpecified()
    {
        // Act
        string assemblyName = Utilities.GetAssemblyName(typeof(UtilitiesTests).Assembly);

        // Assert
        assemblyName.ShouldBe("Servly.Core.UnitTests");
    }

    [Fact]
    public void GetAssemblyInformationalVersion_ShouldReturnCorrectVersion()
    {
        // Act
        string assemblyVersion = Utilities.GetAssemblyInformationalVersion(typeof(UtilitiesTests).Assembly);

        // Assert - This relies on the version applied to the Servly.Core.UnitTests assembly.
        assemblyVersion.ShouldBe("v1.0.0");
    }
}
