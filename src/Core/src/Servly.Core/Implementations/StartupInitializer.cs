namespace Servly.Core.Implementations;

internal class StartupInitializer : IStartupInitializer
{
    private readonly List<IInitializer> _initializers;

    public StartupInitializer(IEnumerable<IInitializer> initializers)
    {
        _initializers = initializers.ToList();
    }

    public async Task InitializeAsync()
    {
        if (_initializers.Count == 0)
            return;

        await Task.WhenAll(_initializers.Select(i => i.InitializeAsync()));
    }
}
