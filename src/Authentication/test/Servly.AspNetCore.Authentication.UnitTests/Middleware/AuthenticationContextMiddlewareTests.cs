using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Servly.AspNetCore.Authentication.Middleware;
using Servly.Authentication;
using Shouldly;
using Xunit;

namespace Servly.AspNetCore.Authentication.UnitTests.Middleware;

public class AuthenticationContextMiddlewareTests
{
    [Fact]
    public void ShouldThrowIfConstructedWithNullLogger()
    {
        Should.Throw<ArgumentException>(() =>
            new AuthenticationContextMiddleware<AuthenticationContextState>(null!, (_, _) => { }));
    }

    [Fact]
    public void ShouldThrowIfConstructedWithNullAction()
    {
        var nullLogger = NullLogger<AuthenticationContextMiddleware<AuthenticationContextState>>.Instance;
        Should.Throw<ArgumentException>(() =>
            new AuthenticationContextMiddleware<AuthenticationContextState>(nullLogger, null!));
    }

    [Fact]
    public async Task ShouldNotModifyTheAuthenticationContextWhenHttpContextIdentityIsNotAuthenticated()
    {
        // Setup
        var nullLogger = NullLogger<AuthenticationContextMiddleware<AuthenticationContextState>>.Instance;
        var sut = new AuthenticationContextMiddleware<AuthenticationContextState>(nullLogger, (state, _) =>
        {
            state.SubjectId = Guid.Parse("77268032-39a0-488c-81de-493452fdc29b");
        });

        var originalState = new AuthenticationContextState
        {
            IsAuthenticated = false, SubjectId = Guid.Parse("a09a42bc-aae0-4e70-a4a2-f880d4c9ce6c")
        };
        var authenticationContext = new AuthenticationContext<AuthenticationContextState>(originalState);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationContext<AuthenticationContextState>)))
            .Returns(authenticationContext);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal());
        httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);

        var requestDelegate = new RequestDelegate(_ =>
        {
            var inProgressState = authenticationContext.GetState();
            inProgressState.ShouldBeSameAs(originalState);
            inProgressState.ShouldNotBeNull();
            inProgressState.IsAuthenticated.ShouldBeFalse();
            inProgressState.SubjectId.ShouldBe(Guid.Parse("a09a42bc-aae0-4e70-a4a2-f880d4c9ce6c"));

            return Task.FromResult(0);
        });

        // Action
        await sut.InvokeAsync(httpContextMock.Object, requestDelegate);
        var newState = authenticationContext.GetState();

        // Assert & Validate
        serviceProviderMock.Verify(x => x.GetService(typeof(IAuthenticationContext<AuthenticationContextState>)), Times.Never);
        newState.ShouldBeSameAs(originalState);
        newState.ShouldNotBeNull();
        newState.IsAuthenticated.ShouldBeFalse();
        newState.SubjectId.ShouldBe(Guid.Parse("a09a42bc-aae0-4e70-a4a2-f880d4c9ce6c"));
    }

    [Fact]
    public async Task ShouldModifyTheAuthenticationContextWhenHttpContextIdentityIsAuthenticated()
    {
        // Setup
        var nullLogger = NullLogger<AuthenticationContextMiddleware<AuthenticationContextState>>.Instance;
        var sut = new AuthenticationContextMiddleware<AuthenticationContextState>(nullLogger, (state, _) =>
        {
            state.SubjectId = Guid.Parse("77268032-39a0-488c-81de-493452fdc29b");
        });

        var originalState = new AuthenticationContextState
        {
            IsAuthenticated = false, SubjectId = Guid.Parse("a09a42bc-aae0-4e70-a4a2-f880d4c9ce6c")
        };
        var authenticationContext = new AuthenticationContext<AuthenticationContextState>(originalState);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(typeof(IAuthenticationContext<AuthenticationContextState>)))
            .Returns(authenticationContext);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new List<ClaimsIdentity>
        {
            new(new List<Claim>
            {
                new("ClaimType", "ClaimValue")
            }, "UnitTestAuthentication")
        }));
        httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);

        var requestDelegate = new RequestDelegate(_ =>
        {
            var inProgressState = authenticationContext.GetState();
            inProgressState.ShouldNotBeSameAs(originalState);
            inProgressState.ShouldNotBeNull();
            inProgressState.IsAuthenticated.ShouldBeTrue();
            inProgressState.SubjectId.ShouldBe(Guid.Parse("77268032-39a0-488c-81de-493452fdc29b"));

            return Task.FromResult(0);
        });

        // Action
        await sut.InvokeAsync(httpContextMock.Object, requestDelegate);
        var newState = authenticationContext.GetState();

        // Assert & Validate
        serviceProviderMock.Verify(x => x.GetService(typeof(IAuthenticationContext<AuthenticationContextState>)), Times.Once);
        newState.ShouldBeSameAs(originalState);
        newState.ShouldNotBeNull();
        newState.IsAuthenticated.ShouldBeFalse();
        newState.SubjectId.ShouldBe(Guid.Parse("a09a42bc-aae0-4e70-a4a2-f880d4c9ce6c"));
    }
}
