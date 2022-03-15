namespace Servly.AspNetCore.Idempotency.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
public class IdempotentAttribute : Attribute
{
    public string KeyPrefix { get; }

    public IdempotentAttribute(string keyPrefix)
    {
        KeyPrefix = keyPrefix;
    }
}
