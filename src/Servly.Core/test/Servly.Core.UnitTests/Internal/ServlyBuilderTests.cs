// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.DependencyInjection;
using Moq;
using Servly.Core.Exceptions;
using Servly.Core.Services.Implementations;
using Shouldly;
using System;
using Xunit;

namespace Servly.Core.UnitTests.Internal
{
    public class ServlyBuilderTests
    {
        [Fact]
        public void TryRegisterModuleShouldReturnTrueWhenSuccessfullyRegistered()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            // Register the Module should cause result to be true
            bool result = servlyBuilder.TryRegisterModule(nameof(ServlyBuilderTests));
            result.ShouldBeTrue();
        }

        [Fact]
        public void TryRegisterModuleShouldReturnFalseWhenModuleIsAlreadyRegistered()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            // Register the Module
            servlyBuilder.TryRegisterModule(nameof(ServlyBuilderTests));

            // Registering the Module again should now cause result to be false
            bool result = servlyBuilder.TryRegisterModule(nameof(ServlyBuilderTests));
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsModuleRegisteredShouldReturnFalseWhenModuleIsNotRegistered()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            bool result = servlyBuilder.IsModuleRegistered(nameof(ServlyBuilderTests));
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsModuleRegisteredShouldReturnTrueWhenModuleIsRegistered()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            // Register the Module
            servlyBuilder.TryRegisterModule(nameof(ServlyBuilderTests));

            bool result = servlyBuilder.IsModuleRegistered(nameof(ServlyBuilderTests));
            result.ShouldBeTrue();
        }

        [Fact]
        public void AddBuildActionShouldCorrectlyRegisterBuildActionsForExecutionByBuild()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            var serviceDescriptor = new ServiceDescriptor(typeof(ServlyBuilderTests), typeof(ServlyBuilderTests));
            servlyBuilder.AddBuildAction(sc =>
            {
                sc.Add(serviceDescriptor);
            });

            servlyBuilder.Build();

            mockServiceCollection.Verify(sc => sc.Add(serviceDescriptor), Times.Once);
        }

        [Fact]
        public void AddInitializerShouldCorrectlyRegisterBuildActionsForExecutionByBuild()
        {
            var serviceCollection = new ServiceCollection();
            var servlyBuilder = new Core.Internal.ServlyBuilder(serviceCollection);

            servlyBuilder.AddInitializer<ServiceIdInitializer>();
            servlyBuilder.Build();

            serviceCollection.ShouldContain(sd => sd.Lifetime == ServiceLifetime.Singleton
                                                  && sd.ServiceType == typeof(IInitializer)
                                                  && sd.ImplementationType == typeof(ServiceIdInitializer));
        }

        [Fact]
        public void AddBuildActionShouldThrowAnArgumentNullExceptionWhenBuildActionDelegateIsNull()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            Should.Throw<ArgumentNullException>(() => servlyBuilder.AddBuildAction(null!));
        }

        [Fact]
        public void BuildShouldThrowAServlyBuilderAlreadyBuiltExceptionWhenAlreadyBuilt()
        {
            var mockServiceCollection = new Mock<IServiceCollection>();
            var servlyBuilder = new Core.Internal.ServlyBuilder(mockServiceCollection.Object);

            // First build should be valid
            Should.NotThrow(servlyBuilder.Build);

            // Second build should throw exception
            Should.Throw<ServlyBuilderAlreadyBuiltException>(servlyBuilder.Build)
                .Code.ShouldBe("builder_already_built");
        }
    }
}
