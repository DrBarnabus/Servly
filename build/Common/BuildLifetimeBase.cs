using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;
using Cake.Incubator.LoggingExtensions;
using Common.Addins.GitVersion;
using Common.Models;
using Common.Utilities;

namespace Common;

public class BuildLifetimeBase<TContext> : FrostingLifetime<TContext>
    where TContext : BuildContextBase
{
    public override void Setup(TContext context)
    {
        var buildSystem = context.BuildSystem();
        context.IsLocalBuild = buildSystem.IsLocalBuild;
        context.IsAzurePipelineBuild = buildSystem.IsRunningOnAzurePipelines;
        context.IsGitHubActionsBuild = buildSystem.IsRunningOnGitHubActions;

        context.IsPullRequest = buildSystem.IsPullRequest;
        context.IsOriginalRepo = context.IsOriginalRepo();
        context.IsMainBranch = context.IsMainBranch();
        context.IsTagged = context.IsTagged();

        context.IsOnWindows = context.IsRunningOnWindows();
        context.IsOnLinux = context.IsRunningOnLinux();
        context.IsOnMacOs = context.IsRunningOnMacOs();

        var gitVersion = context.GitVersion(new GitVersionSettings
        {
            LogFilePath = context.IsAzurePipelineBuild ? "console" : null,
            OutputTypes = new HashSet<GitVersionOutput> { GitVersionOutput.Json, GitVersionOutput.BuildServer }
        });

        context.Version = BuildVersion.Calculate(gitVersion);
    }

    public override void Teardown(TContext context, ITeardownContext info)
    {
        context.StartGroup("Build Teardown");

        try
        {
            context.Information("Starting Teardown...");

            LogBuildInformation(context);

            context.Information("Finished running tasks.");
        }
        catch (Exception ex)
        {
            context.Error(ex.Dump());
        }

        context.EndGroup();
    }

    protected void LogBuildInformation(TContext context)
    {
        context.Information("Version:               {0}", context.Version?.SemVersion);
        context.Information("Build Agent:           {0}", context.GetBuildAgent());
        context.Information("OS:                    {0}", context.GetOs());
        context.Information("Current Branch:        {0}", context.GetBranch());
        context.Information("Original Repo:         {0}", context.IsOriginalRepo);
        context.Information("Pull Request:          {0}", context.IsPullRequest);
        context.Information("Tagged:                {0}", context.IsTagged);
        context.Information("Main Branch:           {0}", context.IsMainBranch);
    }
}
