using Shouldly;
using Xunit;

namespace Servly.AspNetCore.ModelBinding.Hybrid.UnitTests;

public class FromHybridAttributeTests
{
    [Fact]
    public void ShouldReturnHybridBindingSource()
    {
        var sut = new FromHybridAttribute();
        sut.BindingSource.ShouldBe(HybridBindingSource.Hybrid);
    }
}
