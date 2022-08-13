using Microsoft.Extensions.DependencyInjection;
using Moq;
using Servly.Core.Services;
using Servly.Core.Services.Implementations;
using Servly.Extensions;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Extensions;

public class ServlyBuilderExtensionsTests
{
    [Fact]
    public void AddSystemClock_ShouldRegisterCorrectServices()
    {
        var servlyBuilder = new Mock<IServlyBuilder>(MockBehavior.Strict);

        servlyBuilder.Setup(e => e.TryRegisterModule("Core.SystemClock"))
            .Returns(false)
            .Verifiable();

        servlyBuilder.Setup(e => e.Services.Add(It.Is<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IClock) && sd.ImplementationType == typeof(SystemClock) && sd.Lifetime == ServiceLifetime.Singleton)))
            .Verifiable();

        var result = servlyBuilder.Object.AddSystemClock();
        result.ShouldNotBeNull();
        result.ShouldBe(servlyBuilder.Object);

        servlyBuilder.Verify();
    }

    [Fact]
    public void AddSystemClock_ShouldImmediatelyReturnWhenAlreadyRegistered()
    {
        var servlyBuilder = new Mock<IServlyBuilder>(MockBehavior.Strict);

        servlyBuilder.Setup(e => e.TryRegisterModule("Core.SystemClock"))
            .Returns(true)
            .Verifiable();

        var result = servlyBuilder.Object.AddSystemClock();
        result.ShouldNotBeNull();
        result.ShouldBe(servlyBuilder.Object);

        servlyBuilder.Verify();
    }
}
