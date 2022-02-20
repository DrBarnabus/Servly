using Cake.Common;
using Cake.Core;
using Common.Models;

namespace Publish.Models;

public class Credentials
{
    public GitHubCredentials? GitHub { get; private set; }
    public NugetCredentials? Nuget { get; private set; }

    public static Credentials GetCredentials(ICakeContext context) => new()
    {
        GitHub = new GitHubCredentials(context.EnvironmentVariable("GITHUB_TOKEN")),
        Nuget = new NugetCredentials(context.EnvironmentVariable("NUGET_API_KEY"))
    };
}
