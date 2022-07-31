namespace Servly.Core.StartupInformation;

public interface IStartupInformation
{
    public string SectionTitle { get; }

    public Dictionary<string, string> Values { get; }
}
