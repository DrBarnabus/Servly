using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Servly.Core.Exceptions;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Implementations;

public class ServlyBuilderTests
{
    [Fact]
    public void Services_ShouldReturnTheServiceCollectionUsedToConstruct()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        sut.Services.ShouldBe(mockServices.Object);
    }

    [Fact]
    public void Configuration_ShouldReturnTheConfigurationUsedToConstruct()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        sut.Configuration.ShouldBe(mockConfiguration.Object);
    }

    [Fact]
    public void TryRegisterModule_ShouldReturnTrueWhenSuccessfullyRegistered()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        bool result = sut.TryRegisterModule("Test");
        result.ShouldBeTrue();

        sut.RegisteredModules.ShouldContainKey("Test");
    }

    [Fact]
    public void TryRegisterModule_ShouldReturnFalseWhenModuleIsAlreadyRegistered()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        sut.TryRegisterModule("Test");

        bool result = sut.TryRegisterModule("Test");
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsModuleRegistered_ShouldReturnTrueWhenModuleIsRegistered()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);
        sut.RegisteredModules.TryAdd("Test", true);

        bool result = sut.IsModuleRegistered("Test");
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsModuleRegistered_ShouldReturnFalseWhenModuleIsRegisteredNotRegistered()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        bool result = sut.IsModuleRegistered("Test");
        result.ShouldBeFalse();
    }

    [Fact]
    public void AddBuildAction_ShouldAppendToBuildActionsWhenNotNull()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        sut.AddBuildAction(_ => {});

        sut.BuildActions.Count.ShouldBe(1);
    }

    [Fact]
    public void AddBuildAction_ShouldThrowWhenBuildActionIsNull()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        var exception = Should.Throw<ArgumentException>(() => sut.AddBuildAction(null!));
        exception.Message.ShouldBe("BuildAction cannot be null (Parameter 'buildAction is not null')");
    }

    [Fact]
    public void AddInitializer_ShouldAppendToBuildActions()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        sut.AddInitializer<TestInitializer>();
        sut.BuildActions.Count.ShouldBe(1);
    }

    private class TestInitializer : IInitializer
    {
        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void AddOptions_ShouldThrowWhenConfigurationSectionIsNull()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        var exception = Should.Throw<ArgumentException>(() => sut.AddOptions<ServlyOptions>(null!));
        exception.Message.ShouldBe("sectionKey cannot be null or empty (Parameter '!string.IsNullOrEmpty(sectionKey)')");
    }

    [Fact]
    public void AddOptions_ShouldThrowWhenConfigurationSectionIsEmpty()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        var exception = Should.Throw<ArgumentException>(() => sut.AddOptions<ServlyOptions>(""));
        exception.Message.ShouldBe("sectionKey cannot be null or empty (Parameter '!string.IsNullOrEmpty(sectionKey)')");
    }

    [Fact]
    public void GetOptions_ShouldReturnOptionsSnapshotFromServices()
    {
        var testOptions = new ServlyOptions {ServiceName = "Test"};

        var mockOptions = new Mock<IOptionsSnapshot<ServlyOptions>>();
        mockOptions.Setup(x => x.Value)
            .Returns(testOptions);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IOptionsSnapshot<ServlyOptions>>(mockOptions.Object);

        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(serviceCollection, mockConfiguration.Object);

        var options = sut.GetOptions<ServlyOptions>();
        options.ShouldBeSameAs(testOptions);
        options.ServiceName.ShouldBe("Test");
    }

    [Fact]
    public void GetService_ShouldReturnFromServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<string>("TestValue");

        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(serviceCollection, mockConfiguration.Object);

        string testString = sut.GetService<string>();
        testString.ShouldBe("TestValue");
    }

    [Fact]
    public void Build_ShouldCallAllBuildActionsOnBuild()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);

        bool actionOne = false;
        sut.BuildActions.Add(_ => actionOne = true);

        bool actionTwo = false;
        sut.BuildActions.Add(_ => actionTwo = true);

        sut.Build();
        actionOne.ShouldBeTrue();
        actionTwo.ShouldBeTrue();
    }

    [Fact]
    public void Build_ShouldThrowAnAlreadyBuiltExceptionWhenAlreadyBuilt()
    {
        var mockServices = new Mock<IServiceCollection>();
        var mockConfiguration = new Mock<IConfiguration>();
        var sut = new Core.Implementations.ServlyBuilder(mockServices.Object, mockConfiguration.Object);
        sut.Build();

        Should.Throw<AlreadyBuiltException>(() => sut.Build());
    }
}
