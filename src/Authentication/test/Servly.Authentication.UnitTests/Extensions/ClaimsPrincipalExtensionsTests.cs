using System.Security.Claims;
using Servly.Authentication.Extensions;
using Shouldly;
using Xunit;

// ReSharper disable InvokeAsExtensionMethod

namespace Servly.Authentication.UnitTests.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal CreatePrincipal(string claimType, string claimValue)
    {
        return new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim(claimType, claimValue)
            })
        });
    }

    [Fact]
    public void GetClaimValue_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValue(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValue_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValue(principal, null!));
    }

    [Fact]
    public void GetClaimValue_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValue(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValue_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        string? result = ClaimsPrincipalExtensions.GetClaimValue(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Fact]
    public void GetClaimValue_ShouldReturnCorrectValueWhenFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        string? result = ClaimsPrincipalExtensions.GetClaimValue(principal, "ClaimType");
        result.ShouldBe("TestValue");
    }

    #region GetClaimValueAsBool

    [Fact]
    public void GetClaimValueAsBool_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsBool(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsBool_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsBool(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsBool_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsBool(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsBool_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        bool? result = ClaimsPrincipalExtensions.GetClaimValueAsBool(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("TRUE", true)]
    [InlineData("FALSE", false)]
    [InlineData("TrUe", true)]
    [InlineData("fAlSe", false)]
    [InlineData("Invalid", null)]
    public void GetClaimValueAsBool_ShouldReturnCorrectValueWhenFound(string claimValue, bool? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        bool? result = ClaimsPrincipalExtensions.GetClaimValueAsBool(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsByte

    [Fact]
    public void GetClaimValueAsByte_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsByte(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsByte_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsByte(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsByte_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsByte(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsByte_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        byte? result = ClaimsPrincipalExtensions.GetClaimValueAsByte(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("0", byte.MinValue)]
    [InlineData("255", byte.MaxValue)]
    [InlineData("256", null)]
    [InlineData("not a byte", null)]
    public void GetClaimValueAsByte_ShouldReturnCorrectValueWhenFound(string claimValue, byte? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        byte? result = ClaimsPrincipalExtensions.GetClaimValueAsByte(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsShort

    [Fact]
    public void GetClaimValueAsShort_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsShort(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsShort_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsShort(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsShort_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsShort(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsShort_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        short? result = ClaimsPrincipalExtensions.GetClaimValueAsShort(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("-32768", short.MinValue)]
    [InlineData("32767", short.MaxValue)]
    [InlineData("-32769", null)]
    [InlineData("32769", null)]
    [InlineData("not a short", null)]
    public void GetClaimValueAsShort_ShouldReturnCorrectValueWhenFound(string claimValue, short? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        short? result = ClaimsPrincipalExtensions.GetClaimValueAsShort(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsUnsignedShort

    [Fact]
    public void GetClaimValueAsUnsignedShort_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedShort(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsUnsignedShort_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedShort(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsUnsignedShort_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedShort(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsUnsignedShort_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        ushort? result = ClaimsPrincipalExtensions.GetClaimValueAsUnsignedShort(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("0", ushort.MinValue)]
    [InlineData("65535", ushort.MaxValue)]
    [InlineData("-1", null)]
    [InlineData("65536", null)]
    [InlineData("not a short", null)]
    public void GetClaimValueAsUnsignedShort_ShouldReturnCorrectValueWhenFound(string claimValue, ushort? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        ushort? result = ClaimsPrincipalExtensions.GetClaimValueAsUnsignedShort(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsInt

    [Fact]
    public void GetClaimValueAsInt_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsInt(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsInt_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsInt(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsInt_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsInt(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsInt_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        int? result = ClaimsPrincipalExtensions.GetClaimValueAsInt(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("-2147483648", int.MinValue)]
    [InlineData("2147483647", int.MaxValue)]
    [InlineData("-2147483649", null)]
    [InlineData("2147483648", null)]
    [InlineData("not a int", null)]
    public void GetClaimValueAsInt_ShouldReturnCorrectValueWhenFound(string claimValue, int? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        int? result = ClaimsPrincipalExtensions.GetClaimValueAsInt(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsUnsignedInt

    [Fact]
    public void GetClaimValueAsUnsignedInt_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedInt(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsUnsignedInt_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedInt(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsUnsignedInt_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedInt(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsUnsignedInt_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        uint? result = ClaimsPrincipalExtensions.GetClaimValueAsUnsignedInt(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("0", uint.MinValue)]
    [InlineData("4294967295", uint.MaxValue)]
    [InlineData("-1", null)]
    [InlineData("4294967296", null)]
    [InlineData("not a int", null)]
    public void GetClaimValueAsUnsignedInt_ShouldReturnCorrectValueWhenFound(string claimValue, uint? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        uint? result = ClaimsPrincipalExtensions.GetClaimValueAsUnsignedInt(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsLong

    [Fact]
    public void GetClaimValueAsLong_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsLong(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsLong_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsLong(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsLong_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsLong(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsLong_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        long? result = ClaimsPrincipalExtensions.GetClaimValueAsLong(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("-9223372036854775808", long.MinValue)]
    [InlineData("9223372036854775807", long.MaxValue)]
    [InlineData("-9223372036854775809", null)]
    [InlineData("9223372036854775808", null)]
    [InlineData("not a long", null)]
    public void GetClaimValueAsLong_ShouldReturnCorrectValueWhenFound(string claimValue, long? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        long? result = ClaimsPrincipalExtensions.GetClaimValueAsLong(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsUnsignedLong

    [Fact]
    public void GetClaimValueAsUnsignedLong_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedLong(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsUnsignedLong_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedLong(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsUnsignedLong_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsUnsignedLong(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsUnsignedLong_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        ulong? result = ClaimsPrincipalExtensions.GetClaimValueAsUnsignedLong(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("0", ulong.MinValue)]
    [InlineData("18446744073709551615", ulong.MaxValue)]
    [InlineData("-1", null)]
    [InlineData("18446744073709551616", null)]
    [InlineData("not a long", null)]
    public void GetClaimValueAsUnsignedLong_ShouldReturnCorrectValueWhenFound(string claimValue, ulong? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        ulong? result = ClaimsPrincipalExtensions.GetClaimValueAsUnsignedLong(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsDecimal

    [Fact]
    public void GetClaimValueAsDecimal_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsDecimal(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsDecimal_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsDecimal(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsDecimal_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsDecimal(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsDecimal_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        decimal? result = ClaimsPrincipalExtensions.GetClaimValueAsDecimal(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    // [InlineData("-15.5", -15.5m)]
    // [InlineData("15.5", 15.5m)]
    [InlineData("not a decimal", null)]
    public void GetClaimValueAsDecimal_ShouldReturnCorrectValueWhenFound(string claimValue, decimal? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        decimal? result = ClaimsPrincipalExtensions.GetClaimValueAsDecimal(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsDouble

    [Fact]
    public void GetClaimValueAsDouble_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsDouble(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsDouble_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsDouble(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsDouble_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsDouble(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsDouble_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        double? result = ClaimsPrincipalExtensions.GetClaimValueAsDouble(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("-15.5", -15.5)]
    [InlineData("15.5", 15.5)]
    [InlineData("not a decimal", null)]
    public void GetClaimValueAsDouble_ShouldReturnCorrectValueWhenFound(string claimValue, double? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        double? result = ClaimsPrincipalExtensions.GetClaimValueAsDouble(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsFloat

    [Fact]
    public void GetClaimValueAsFloat_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsFloat(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsFloat_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsFloat(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsFloat_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsFloat(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsFloat_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        float? result = ClaimsPrincipalExtensions.GetClaimValueAsFloat(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("-15.5", -15.5f)]
    [InlineData("15.5", 15.5f)]
    [InlineData("not a decimal", null)]
    public void GetClaimValueAsFloat_ShouldReturnCorrectValueWhenFound(string claimValue, float? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        float? result = ClaimsPrincipalExtensions.GetClaimValueAsFloat(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsGuid

    [Fact]
    public void GetClaimValueAsGuid_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsGuid(null!, "ClaimType"));
    }

    [Fact]
    public void GetClaimValueAsGuid_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsGuid(principal, null!));
    }

    [Fact]
    public void GetClaimValueAsGuid_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsGuid(principal, string.Empty));
    }

    [Fact]
    public void GetClaimValueAsGuid_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        var result = ClaimsPrincipalExtensions.GetClaimValueAsGuid(principal, "DifferentClaimType");
        result.ShouldBeNull();
    }

    // ReSharper disable once InconsistentNaming
    public static IEnumerable<object[]> GetClaimValueAsGuid_TestData = new List<object[]>
    {
        new object[]
        {
            "c014ec24-8b27-4ae3-8ae6-45f30eeb8758",
            Guid.Parse("c014ec24-8b27-4ae3-8ae6-45f30eeb8758")
        },
        new object[]
        {
            "",
            null!
        },
        new object[]
        {
            "not a guid",
            null!
        }
    };

    [Theory]
    [MemberData(nameof(GetClaimValueAsGuid_TestData))]
    public void GetClaimValueAsGuid_ShouldReturnCorrectValueWhenFound(string claimValue, Guid? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        var result = ClaimsPrincipalExtensions.GetClaimValueAsGuid(principal, "ClaimType");
        result.ShouldBe(expectedResult);
    }

    #endregion

    #region GetClaimValueAsEnum

    public enum ExampleEnum
    {
        First = 0,
        Second = 1
    }

    private struct NotAnEnum
    {
    }

    [Fact]
    public void GetClaimValueAsEnum_ShouldThrowAnExceptionWhenTEnumIsNotAnEnum()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsEnum<NotAnEnum>(principal, "ClaimType", false));
    }

    [Fact]
    public void GetClaimValueAsEnum_ShouldThrowAnExceptionWhenPrincipalIsNull()
    {
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsEnum<ExampleEnum>(null!, "ClaimType", false));
    }

    [Fact]
    public void GetClaimValueAsEnum_ShouldThrowAnExceptionWhenClaimTypeIsNull()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsEnum<ExampleEnum>(principal, null!, false));
    }

    [Fact]
    public void GetClaimValueAsEnum_ShouldThrowAnExceptionWhenClaimTypeIsEmptyString()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        Should.Throw<ArgumentException>(() => ClaimsPrincipalExtensions.GetClaimValueAsEnum<ExampleEnum>(principal, string.Empty, false));
    }

    [Fact]
    public void GetClaimValueAsEnum_ShouldReturnNullWhenValueNotFound()
    {
        var principal = CreatePrincipal("ClaimType", "TestValue");
        var result = ClaimsPrincipalExtensions.GetClaimValueAsEnum<ExampleEnum>(principal, "DifferentClaimType", false);
        result.ShouldBeNull();
    }

    // ReSharper disable once InconsistentNaming
    public static IEnumerable<object[]> GetClaimValueAsEnum_TestData = new List<object[]>
    {
        new object[]
        {
            "First",
            ExampleEnum.First
        },
        new object[]
        {
            "0",
            ExampleEnum.First
        },
        new object[]
        {
            "not an enum value",
            null!
        },
        new object[]
        {
            "",
            null!
        }
    };

    [Theory]
    [MemberData(nameof(GetClaimValueAsEnum_TestData))]
    public void GetClaimValueAsEnum_ShouldReturnCorrectValueWhenFound(string claimValue, ExampleEnum? expectedResult)
    {
        var principal = CreatePrincipal("ClaimType", claimValue);
        var result = ClaimsPrincipalExtensions.GetClaimValueAsEnum<ExampleEnum>(principal, "ClaimType", false);
        result.ShouldBe(expectedResult);
    }

    #endregion
}
