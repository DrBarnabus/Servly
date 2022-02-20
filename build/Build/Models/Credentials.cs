using Cake.Common;
using Cake.Core;
using Common.Models;

namespace Build.Models;

public class Credentials
{
    public CodeCovCredentials? CodeCov { get; private set; }

    public static Credentials GetCredentials(ICakeContext context) => new()
    {
        CodeCov = new CodeCovCredentials(context.EnvironmentVariable("CODECOV_TOKEN")),
    };
}
