using Servly.Core.Exceptions;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Exceptions;

public class AlreadyBuiltExceptionTests
{
    [Fact]
    public void ShouldHaveCodeOfDefault()
    {
        var subject = new AlreadyBuiltException();
        subject.Code.ShouldBe("already_built");
    }

    [Fact]
    public void ShouldHaveCorrectMessage()
    {
        var subject = new AlreadyBuiltException();
        subject.Message.ShouldBe("This IServlyBuilder instance has already been built");
    }
}
