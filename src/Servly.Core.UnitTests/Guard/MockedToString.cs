namespace Servly.Core.UnitTests.Guard;

public class MockedToString
{
    private int _callCount;

    public int CallCount => _callCount;

    public override string ToString()
    {
        Interlocked.Increment(ref _callCount);
        return string.Empty;
    }
}
