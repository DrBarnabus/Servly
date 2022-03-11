using System.Runtime.InteropServices;

namespace Servly.Core.StartupInformation;

public class RuntimeStartupInformation : IStartupInformation
{
    public string SectionTitle => "Runtime";

    public Dictionary<string, string> Values => new()
    {
        { "Processor Architecture", RuntimeInformation.ProcessArchitecture.ToString() },
        { "Operating System", $"{RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})" },
        { "Framework Description", RuntimeInformation.FrameworkDescription },
        { "Runtime Identifier", RuntimeInformation.RuntimeIdentifier }
    };
}
