using Servly.Core.Exceptions;
using Shouldly;
using Xunit;

namespace Servly.Core.UnitTests.Exceptions;

public class ModuleAlreadyRegisteredExceptionTests
{
    [Fact]
    public void ShouldHaveCodeOfDefault()
    {
        var subject = new ModuleAlreadyRegisteredException("Test");
        subject.Code.ShouldBe("module_already_registered");
    }

    [Fact]
    public void ShouldHaveCorrectMessage()
    {
        var subject = new ModuleAlreadyRegisteredException("Test");
        subject.Message.ShouldBe("Servly module 'Test' has already been registered and cannot be registered again");
    }
}
