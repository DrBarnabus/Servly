using Servly.Core.Exceptions;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Exceptions;

public class ServlyExceptionTests
{
    [Fact]
    public void ShouldHaveCodeOfDefault()
    {
        var subject = new ServlyException(null);
        subject.Code.ShouldBe("default");
    }

    [Fact]
    public void ShouldHaveCorrectMessage()
    {
        var subject = new ServlyException("Expected Message");
        subject.Message.ShouldBe("Expected Message");
    }

    [Fact]
    public void ShouldHaveCorrectInnerException()
    {
        var exceptedInner = new Exception("Expected Inner");

        var subject = new ServlyException(null, exceptedInner);
        subject.InnerException.ShouldNotBeNull();
        subject.InnerException.ShouldBe(exceptedInner);
        subject.InnerException.Message.ShouldBe(exceptedInner.Message);
    }
}
