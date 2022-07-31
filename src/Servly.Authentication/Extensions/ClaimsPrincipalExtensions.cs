using System.Security.Claims;
using Servly.Core;

namespace Servly.Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType)
    {
        Guard.Assert(principal is not null, $"Principal cannot be null");
        Guard.Assert(!string.IsNullOrEmpty(claimType), $"ClaimType cannot be null or empty");

        var claim = principal.FindFirst(claimType);
        return claim?.Value;
    }

    public static bool? GetClaimValueAsBool(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            bool.TryParse(claimAsString.AsSpan(), out bool value) ? value : null;
    }

    public static byte? GetClaimValueAsByte(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            byte.TryParse(claimAsString.AsSpan(), out byte value) ? value : null;
    }

    public static short? GetClaimValueAsShort(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            short.TryParse(claimAsString.AsSpan(), out short value) ? value : null;
    }

    public static ushort? GetClaimValueAsUnsignedShort(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            ushort.TryParse(claimAsString.AsSpan(), out ushort value) ? value : null;
    }

    public static int? GetClaimValueAsInt(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            int.TryParse(claimAsString.AsSpan(), out int value) ? value : null;
    }

    public static uint? GetClaimValueAsUnsignedInt(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            uint.TryParse(claimAsString.AsSpan(), out uint value) ? value : null;
    }

    public static long? GetClaimValueAsLong(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            long.TryParse(claimAsString.AsSpan(), out long value) ? value : null;
    }

    public static ulong? GetClaimValueAsUnsignedLong(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            ulong.TryParse(claimAsString.AsSpan(), out ulong value) ? value : null;
    }

    public static decimal? GetClaimValueAsDecimal(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            decimal.TryParse(claimAsString.AsSpan(), out decimal value) ? value : null;
    }

    public static double? GetClaimValueAsDouble(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            double.TryParse(claimAsString.AsSpan(), out double value) ? value : null;
    }

    public static float? GetClaimValueAsFloat(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            float.TryParse(claimAsString.AsSpan(), out float value) ? value : null;
    }

    public static Guid? GetClaimValueAsGuid(this ClaimsPrincipal principal, string claimType)
    {
        string? claimAsString = GetClaimValue(principal, claimType);
        return claimAsString is null ? null :
            Guid.TryParse(claimAsString.AsSpan(), out var value) ? value : null;
    }

    public static TEnum? GetClaimValueAsEnum<TEnum>(this ClaimsPrincipal principal, string claimType, bool ignoreCase)
        where TEnum : struct
    {
        Guard.Assert(typeof(TEnum).IsEnum, $"{typeof(TEnum)} is not an enum.");

        string? claimAsString = GetClaimValue(principal, claimType);
        if (claimAsString is null) return null;
        return Enum.TryParse<TEnum>(claimAsString.AsSpan(), ignoreCase, out var value) ? value : null;
    }
}
