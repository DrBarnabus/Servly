using AutoFixture.Xunit2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests;

/// <summary>
///     This class tests the default implementations provided in the <see cref="IChainedServlyBuilder{TBuilder}" />
///     interface.
/// </summary>
public class ChainedServlyBuilderInterfaceTests
{
    [Fact]
    public void Services_ShouldReturnInstanceOfServices()
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.Services)
            .Returns(new ServiceCollection())
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        subject.Services.ShouldNotBeNull();

        servlyBuilder.Verify();
    }

    [Fact]
    public void Configuration_ShouldReturnInstanceOfConfiguration()
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.Configuration)
            .Returns(new ConfigurationRoot(new List<IConfigurationProvider>()))
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        subject.Configuration.ShouldNotBeNull();

        servlyBuilder.Verify();
    }

    [Theory]
    [AutoData]
    public void TryRegisterModule_ShouldBeCalledWithCorrectParameters(string moduleName, bool returnValue)
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.TryRegisterModule(moduleName))
            .Returns(returnValue)
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        bool result = subject.TryRegisterModule(moduleName);
        result.ShouldBe(returnValue);

        servlyBuilder.Verify();
    }

    [Theory]
    [AutoData]
    public void IsModuleRegistered_ShouldBeCalledWithCorrectParameters(string moduleName, bool returnValue)
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.IsModuleRegistered(moduleName))
            .Returns(returnValue)
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        bool result = subject.IsModuleRegistered(moduleName);
        result.ShouldBe(returnValue);

        servlyBuilder.Verify();
    }

    [Fact]
    public void AddBuildAction_ShouldBeCalledWithCorrectParameters()
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.AddBuildAction(It.IsAny<Action<IServiceCollection>>()))
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        subject.AddBuildAction(_ => { });

        servlyBuilder.Verify();
    }

    [Fact]
    public void AddInitializer_ShouldBeCalledWithCorrectParameters()
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.AddInitializer<TestInitializer>())
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        subject.AddInitializer<TestInitializer>();

        servlyBuilder.Verify();
    }

    [Theory]
    [AutoData]
    public void AddOptions_ShouldBeCalledWithCorrectParameters(string sectionKey, string? instanceName)
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.AddOptions(sectionKey, instanceName, It.IsAny<Func<TestOptions, bool>>()))
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        subject.AddOptions<TestOptions>(sectionKey, instanceName);

        servlyBuilder.Verify();
    }

    [Theory]
    [AutoData]
    public void GetOptions_ShouldBeCalledWithCorrectParameters(string? instanceName)
    {
        var testOptions = new TestOptions();

        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.GetOptions<TestOptions>(instanceName))
            .Returns(testOptions)
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        var result = subject.GetOptions<TestOptions>(instanceName);
        result.ShouldNotBeNull();
        result.ShouldBe(testOptions);

        servlyBuilder.Verify();
    }

    [Fact]
    public void GetService_ShouldBeCalledWithCorrectParameters()
    {
        var testInitializer = new TestInitializer();

        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.GetService<IInitializer>())
            .Returns(testInitializer)
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        var result = subject.GetService<IInitializer>();
        result.ShouldNotBeNull();
        result.ShouldBe(testInitializer);

        servlyBuilder.Verify();
    }

    [Fact]
    public void Build_ShouldBeCalledWithCorrectParameters()
    {
        var servlyBuilder = new Mock<IServlyBuilder>();
        servlyBuilder.Setup(e => e.Build())
            .Verifiable();

        IServlyBuilder subject = new TestChainedBuilder(servlyBuilder.Object);
        subject.Build();

        servlyBuilder.Verify();
    }

    private sealed class TestOptions
    {
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class TestInitializer : IInitializer
    {
        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }
    }

    private sealed class TestChainedBuilder : IChainedServlyBuilder<TestChainedBuilder>
    {
        public IServlyBuilder BaseBuilder { get; }

        public TestChainedBuilder(IServlyBuilder baseBuilder)
        {
            BaseBuilder = baseBuilder;
        }
    }
}
