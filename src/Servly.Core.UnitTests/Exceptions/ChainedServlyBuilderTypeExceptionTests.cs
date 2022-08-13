using Servly.Core.Exceptions;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Exceptions;

public class ChainedServlyBuilderTypeExceptionTests
{
    [Fact]
    public void ShouldHaveCodeOfDefault()
    {
        var subject = new ChainedServlyBuilderTypeException(
            typeof(ChainedServlyBuilderTypeException),
            typeof(ChainedServlyBuilderTypeExceptionTests));

        subject.Code.ShouldBe("chained_servly_builder_type_exception");
    }

    [Fact]
    public void ShouldHaveCorrectMessage()
    {
        var expectedType = typeof(ChainedServlyBuilderTypeException);
        var receivedType = typeof(ChainedServlyBuilderTypeExceptionTests);

        var subject = new ChainedServlyBuilderTypeException(expectedType, receivedType);

        subject.Message.ShouldBe($"Expected input builder to be of type '{expectedType.FullName}' but it was '{receivedType.FullName}'");
    }
}
