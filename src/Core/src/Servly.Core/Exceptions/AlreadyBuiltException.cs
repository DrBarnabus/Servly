namespace Servly.Core.Exceptions;

public class AlreadyBuiltException : ServlyException
{
    public AlreadyBuiltException()
        : base($"This {nameof(IServlyBuilder)} instance has already been built")
    {
    }

    public override string Code { get; } = "already_built";
}
