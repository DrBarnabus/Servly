using Servly.Core.Implementations;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Implementations;

public class StartupInitializerTests
{
    [Fact]
    public async Task ShouldImmediatelyReturnWhenInitializersIsEmpty()
    {
        var sut = new StartupInitializer(new List<IInitializer>());
        await sut.InitializeAsync();
    }

    [Fact]
    public async Task ShouldCallAllInitializersWhenNotEmpty()
    {
        var initializerOne = new TestInitializer();
        var initializerTwo = new TestInitializer();

        var sut = new StartupInitializer(new List<IInitializer>
        {
            initializerOne, initializerTwo
        });

        await sut.InitializeAsync();
        initializerOne.Called.ShouldBeTrue();
        initializerTwo.Called.ShouldBeTrue();
    }

    private class TestInitializer : IInitializer
    {
        public bool Called { get; private set; }

        public Task InitializeAsync()
        {
            Called = true;
            return Task.CompletedTask;
        }
    }
}
