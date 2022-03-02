using System.Runtime.CompilerServices;

namespace Servly.Core;

[InterpolatedStringHandler]
public ref struct GuardInterpolatedStringHandler
{
    private DefaultInterpolatedStringHandler _innerHandler;

    public GuardInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, out bool shouldAppend)
    {
        if (condition)
        {
            _innerHandler = default;
            shouldAppend = false;
            return;
        }

        _innerHandler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
        shouldAppend = true;
    }

    public void AppendLiteral(string value)
    {
        _innerHandler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _innerHandler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _innerHandler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _innerHandler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _innerHandler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(ReadOnlySpan<char> value)
    {
        _innerHandler.AppendFormatted(value);
    }

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _innerHandler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _innerHandler.AppendFormatted(value);
    }

    public string ToStringAndClear()
    {
        return _innerHandler.ToStringAndClear();
    }
}
