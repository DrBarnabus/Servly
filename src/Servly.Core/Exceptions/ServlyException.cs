namespace Servly.Core.Exceptions;

public class ServlyException : Exception
{
    public ServlyException(string? message)
        : base(message)
    {
    }

    public ServlyException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public virtual string Code { get; } = "default";
}
