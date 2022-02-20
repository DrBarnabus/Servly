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
        // Setup
        string expectedValue = typeof(UtilitiesTests).Assembly?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Insert(0, "v") ?? "unknown";

        // Act
        string assemblyVersion = Utilities.GetAssemblyInformationalVersion(typeof(UtilitiesTests).Assembly);

        // Assert
        assemblyVersion.ShouldBe(expectedValue);

        // TODO: This isn't a very good test as the assertion relies on re-running the same logic.
        // Potentially need to look into a better way to do this, or just remove this test.
    }
}
