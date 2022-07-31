using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Guard;

/// <summary>
///     These tests test both <see cref="Guard" /> and <see cref="GuardInterpolatedStringHandler" />.
/// </summary>
public class GuardTests
{
    [Fact]
    public void Assert_ShouldNotThrowBasedOnInputExpression_WhenConditionIsMet()
    {
        // Setup
        string testValue = "Example";
        void Subject() => Core.Guard.Assert(testValue is "Example", $"Test Value was {testValue}.");

        // Act & Assert
        Should.NotThrow(Subject);
    }

    [Fact]
    public void Assert_ShouldThrowBasedOnInputExpression_WhenConditionNotMet()
    {
        // Setup
        string testValue = "Example";
        void Subject() => Core.Guard.Assert(testValue is not "Example", $"Test Value was {testValue}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.ParamName.ShouldBe("testValue is not \"Example\"");
        exception.Message.ShouldBe("Test Value was Example. (Parameter 'testValue is not \"Example\"')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleTemplate()
    {
        // Setup
        int value = 15;
        void Subject() => Core.Guard.Assert(false, $"Value is {value}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is 15. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleTemplateWithFormat()
    {
        // Setup
        int value = 15;
        void Subject() => Core.Guard.Assert(false, $"Value is {value:x4}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is 000f. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleTemplateWithAlignment()
    {
        // Setup
        int value = 15;
        void Subject() => Core.Guard.Assert(false, $"Value is {value,-4}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is 15  . (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleTemplateWithFormatAndAlignment()
    {
        // Setup
        int value = 15;
        void Subject() => Core.Guard.Assert(false, $"Value is {value,6:x4}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is   000f. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleReadOnlySpanChar()
    {
        // Setup
        string value = "Some Value";
        void Subject() => Core.Guard.Assert(false, $"Value is {value.AsSpan()}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is Some Value. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleReadOnlySpanCharWithAlignment()
    {
        // Setup
        string value = "Some Value";
        void Subject() => Core.Guard.Assert(false, $"Value is {value.AsSpan(),14}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is     Some Value. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleString()
    {
        // Setup
        string value = "Some Value";
        void Subject() => Core.Guard.Assert(false, $"Value is {value}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is Some Value. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldHandleStringWithAlignment()
    {
        // Setup
        string value = "Some Value";
        void Subject() => Core.Guard.Assert(false, $"Value is {value,14}.");

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(Subject);
        exception.Message.ShouldBe("Value is     Some Value. (Parameter 'false')");
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldNotAppendWhenConditionIsTrue()
    {
        // Setup
        var mockedToString = new MockedToString();
        void Subject() => Core.Guard.Assert(true, $"Value is {mockedToString}");

        // Act
        Should.NotThrow(Subject);

        // Assert
        mockedToString.CallCount.ShouldBe(0);
    }

    [Fact]
    public void GuardInterpolatedStringHandler_ShouldAppendWhenConditionIsFalse()
    {
        // Setup
        var mockedToString = new MockedToString();
        void Subject() => Core.Guard.Assert(false, $"Value is {mockedToString}");

        // Act
        Should.Throw<ArgumentException>(Subject);

        // Assert
        mockedToString.CallCount.ShouldBe(1);
    }
}
