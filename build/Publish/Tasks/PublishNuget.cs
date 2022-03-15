using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Frosting;
using Common;
using Common.Utilities;

namespace Publish.Tasks;

[TaskName(nameof(PublishNuget))]
[TaskDescription("Publish nuget packages")]
[IsDependentOn(typeof(PublishNugetInternal))]
public sealed class PublishNuget : FrostingTask<BuildContext>
{
}

[TaskName(nameof(PublishNugetInternal))]
[TaskDescription("Publish nuget packages")]
public sealed class PublishNugetInternal : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        bool shouldRun = true;
        shouldRun &= context.ShouldRun(context.IsGitHubActionsBuild, $"{nameof(PublishNuget)} only works on GitHub Actions.");
        shouldRun &= context.ShouldRun(context.IsPreRelease || context.IsStableRelease, $"{nameof(PublishNuget)} only works for releases.");
        return shouldRun;
    }

    public override void Run(BuildContext context)
    {
        // publish to nuget.org for tagged stable releases only
        if (context.IsStableRelease)
        {
            string? nugetApiKey = context.Credentials?.Nuget?.ApiKey;
            if (string.IsNullOrEmpty(nugetApiKey))
                throw new InvalidOperationException("Could not resolve NuGet org API key.");

            PublishToNugetRepo(context, nugetApiKey, Constants.NugetOrgUrl);
        }

        // publish to github packages for commits on main and on original repo
        if (context.IsGitHubActionsBuild && context.IsOnMainBranchOriginalRepo)
        {
            string? token = context.Credentials?.GitHub?.Token;
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Could not resolve NuGet GitHub Packages token.");

            PublishToNugetRepo(context, token, Constants.GithubPackagesUrl);
        }
    }

    private static void PublishToNugetRepo(BuildContext context, string apiKey, string apiUrl)
    {
        string? nugetVersion = context.Version!.SemVersion;
        foreach ((string packageName, var filePath) in context.Packages)
        {
            context.Information($"Package {packageName}, version {nugetVersion} is being published.");
            context.DotNetNuGetPush(filePath.FullPath, new DotNetNuGetPushSettings
            {
                ApiKey = apiKey,
                Source = apiUrl,
                SkipDuplicate = true
            });
        }
    }
}
