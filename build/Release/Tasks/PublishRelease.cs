using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.GitReleaseManager;
using Cake.Common.Tools.GitReleaseManager.Create;
using Cake.Frosting;
using Common;
using Common.Models;
using Common.Utilities;

namespace Release.Tasks;

[TaskName(nameof(PublishRelease))]
[TaskDescription("Publish release")]
[IsDependentOn(typeof(PublishReleaseInternal))]
public sealed class PublishRelease : FrostingTask<BuildContext>
{
}

[TaskName(nameof(PublishReleaseInternal))]
[TaskDescription("Publish release")]
public sealed class PublishReleaseInternal : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        bool shouldRun = true;
        shouldRun &= context.ShouldRun(context.IsGitHubActionsBuild, $"{nameof(PublishRelease)} only works on GitHub Actions.");
        shouldRun &= context.ShouldRun(context.IsStableRelease, $"{nameof(PublishRelease)} only works for releases.");
        return shouldRun;
    }

    public override void Run(BuildContext context)
    {
        string? token = context.Credentials?.GitHub?.Token;
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Could not resolve GitHub Token.");

        string? milestone = context.Version?.Milestone;
        if (milestone is null) return;

        context.GitReleaseManagerCreate(token, Constants.RepoOwner, Constants.Repository, new GitReleaseManagerCreateSettings
        {
            Milestone = milestone,
            Name = milestone,
            Prerelease = false,
            TargetCommitish = "main"
        });

        context.GitReleaseManagerClose(token, Constants.RepoOwner, Constants.Repository, milestone);
    }
}
