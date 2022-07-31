using Shouldly;
using Xunit;

namespace Servly.Authentication.UnitTests;

public class AuthenticationContextTests
{
    [Fact]
    public void ShouldReturnTheOriginalStateWhenStateHasNotBeenModified()
    {
        var originalState = new AuthenticationContextState
        {
            IsAuthenticated = true,
            SubjectId = Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261")
        };

        var sut = new AuthenticationContext<AuthenticationContextState>(originalState);

        var newState = sut.GetState();
        newState.ShouldNotBeNull();
        newState.ShouldBeSameAs(originalState);
    }

    [Fact]
    public void ShouldReturnModifiedStateWhenStateHasBeenModified()
    {
        var originalState = new AuthenticationContextState
        {
            IsAuthenticated = true,
            SubjectId = Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261")
        };
        var sut = new AuthenticationContext<AuthenticationContextState>(originalState);

        sut.SetContext(state =>
        {
            state.IsAuthenticated = false;
        });

        var newState = sut.GetState();

        newState.ShouldNotBeNull();
        newState.ShouldNotBeSameAs(originalState);
        newState.IsAuthenticated.ShouldBe(false);
        newState.SubjectId.ShouldBe(originalState.SubjectId);
    }

    [Fact]
    public void ShouldReturnOriginalStateWhenModificationIsPoppedViaDisposable()
    {
        var originalState = new AuthenticationContextState
        {
            IsAuthenticated = true,
            SubjectId = Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261")
        };
        var sut = new AuthenticationContext<AuthenticationContextState>(originalState);

        var disposable = sut.SetContext(state =>
        {
            state.IsAuthenticated = false;
        });

        var newState = sut.GetState();
        newState.ShouldNotBeNull();
        newState.ShouldNotBeSameAs(originalState);
        newState.IsAuthenticated.ShouldBe(false);
        newState.SubjectId.ShouldBe(originalState.SubjectId);

        disposable.Dispose();

        var poppedState = sut.GetState();
        poppedState.ShouldNotBeNull();
        poppedState.ShouldBeSameAs(originalState);
        poppedState.IsAuthenticated.ShouldBe(true);
        poppedState.SubjectId.ShouldBe(originalState.SubjectId);
    }

    [Fact]
    public void ShouldReturnOriginalStateWhenModificationIsPoppedViaDisposableTwice()
    {
        var originalState = new AuthenticationContextState();
        var sut = new AuthenticationContext<AuthenticationContextState>(originalState);

        using var firstState = sut.SetContext(_ => { });
        using var secondState = sut.SetContext(state =>
        {
            state.IsAuthenticated = true;
            state.SubjectId = Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261");
        });

        var disposable = sut.SetContext(state =>
        {
            state.IsAuthenticated = false;
        });

        var newState = sut.GetState();
        newState.ShouldNotBeNull();
        newState.ShouldNotBeSameAs(originalState);
        newState.IsAuthenticated.ShouldBe(false);
        newState.SubjectId.ShouldBe(Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261"));

        disposable.Dispose();
        disposable.Dispose();

        var poppedState = sut.GetState();
        poppedState.ShouldNotBeNull();
        poppedState.IsAuthenticated.ShouldBe(true);
        poppedState.SubjectId.ShouldBe(Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261"));
    }
}
