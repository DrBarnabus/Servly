namespace Servly.Core.Exceptions;

public class ChainedServlyBuilderTypeException : ServlyException
{
    public ChainedServlyBuilderTypeException(Type expectedType, Type receivedType)
        : base($"Expected input builder to be of type '{expectedType.FullName}' but it was '{receivedType.FullName}'")
    {
    }

    public override string Code => "chained_servly_builder_type_exception";
}
