namespace Servly.Idempotency.AspNetCore.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class IdempotentAttribute : Attribute
{
    public string KeyPrefix { get; }

    public IdempotentAttribute(string keyPrefix)
    {
        KeyPrefix = keyPrefix;
    }
}
