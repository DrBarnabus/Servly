namespace Servly.Core.StartupInformation;

public class ApplicationStartupInformation : IStartupInformation
{
    public string SectionTitle => "Application";

    public Dictionary<string, string> Values => new()
    {
        { "Application Name", Utilities.GetAssemblyName() },
        { "Application Version", Utilities.GetAssemblyInformationalVersion() },
        { "Servly Version", Utilities.GetAssemblyInformationalVersion(typeof(ApplicationStartupInformation).Assembly) }
    };
}
