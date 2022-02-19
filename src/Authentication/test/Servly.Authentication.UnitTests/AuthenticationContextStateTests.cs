using Shouldly;
using Xunit;

namespace Servly.Authentication.UnitTests;

public class AuthenticationContextStateTests
{
    [Fact]
    public void Clone_ShouldReturnACloneOfTheObject()
    {
        var originalState = new AuthenticationContextState
        {
            IsAuthenticated = true,
            SubjectId = Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261")
        };

        var clonedState = originalState.Clone();
        clonedState.ShouldNotBeSameAs(originalState);
        clonedState.IsAuthenticated.ShouldBe(true);
        clonedState.SubjectId.ShouldBe(Guid.Parse("ecf06726-3f30-45ca-b68d-1f5bd7fa5261"));
    }
}
