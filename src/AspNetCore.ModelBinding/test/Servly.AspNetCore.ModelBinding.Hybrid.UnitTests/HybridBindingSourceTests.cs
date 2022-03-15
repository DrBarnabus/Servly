using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shouldly;
using Xunit;

namespace Servly.AspNetCore.ModelBinding.Hybrid.UnitTests;

public class HybridBindingSourceTests
{
    [Fact]
    public void CanAcceptDataFrom_ShouldReturnTrueWhenBindingSourceIsBody()
    {
        var sut = HybridBindingSource.Hybrid;
        bool result = sut.CanAcceptDataFrom(BindingSource.Body);
        result.ShouldBeTrue();
    }

    [Fact]
    public void CanAcceptDataFrom_ShouldReturnTrueWhenBindingSourceIsSelf()
    {
        var sut = HybridBindingSource.Hybrid;
        bool result = sut.CanAcceptDataFrom(HybridBindingSource.Hybrid);
        result.ShouldBeTrue();
    }

    // ReSharper disable once InconsistentNaming
    public static IEnumerable<object[]> CanAcceptDataFrom_TestData = new List<object[]>
    {
        new object[] { BindingSource.Custom },
        new object[] { BindingSource.Form },
        new object[] { BindingSource.Header },
        new object[] { BindingSource.Path },
        new object[] { BindingSource.Query },
        new object[] { BindingSource.Services },
        new object[] { BindingSource.Special },
        new object[] { BindingSource.FormFile },
        new object[] { BindingSource.ModelBinding }
    };

    [Theory]
    [MemberData(nameof(CanAcceptDataFrom_TestData))]
    public void CanAcceptDataFrom_ShouldReturnFalseWhenBindingSourceIsNotBodyOrSelf(BindingSource bindingSource)
    {
        var sut = HybridBindingSource.Hybrid;
        bool result = sut.CanAcceptDataFrom(bindingSource);
        result.ShouldBeFalse();
    }
}
