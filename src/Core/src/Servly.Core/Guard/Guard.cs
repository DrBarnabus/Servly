using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

#pragma warning disable CS8777

namespace Servly.Core;

public static class Guard
{
    [AssertionMethod]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert(
        [AssertionCondition(AssertionConditionType.IS_FALSE)] [DoesNotReturnIf(false)] bool condition,
        [InterpolatedStringHandlerArgument("condition")] ref GuardInterpolatedStringHandler message,
        [CallerArgumentExpression("condition")] string? expression = null)
    {
        if (!condition)
            ThrowException(message.ToStringAndClear(), expression);
    }

    [DoesNotReturn]
    private static void ThrowException(string? message, string? paramName)
        => throw new ArgumentException(message, paramName);
}
