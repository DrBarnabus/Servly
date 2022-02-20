using Build.Models;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet.MSBuild;
using Common;
using Common.Utilities;

namespace Build;

public class BuildLifetime : BuildLifetimeBase<BuildContext>
{
    public override void Setup(BuildContext context)
    {
        base.Setup(context);

        context.MsBuildConfiguration = context.Argument(Arguments.Configuration, "Release");
        context.EnableUnitTests = context.IsEnabled(EnvVars.EnabledUnitTests);

        context.Credentials = Credentials.GetCredentials(context);

        SetMsBuildSettingsVersion(context);

        context.StartGroup("Build Setup");
        LogBuildInformation(context);
        context.Information("Configuration:         {0}", context.MsBuildConfiguration);
        context.EndGroup();
    }

    private static void SetMsBuildSettingsVersion(BuildContext context)
    {
        var msBuildSettings = context.MsBuildSettings;
        (var gitVersion, string? version, string? semVersion, _) = context.Version!;

        msBuildSettings.SetVersion(semVersion);
        msBuildSettings.SetAssemblyVersion(version);
        msBuildSettings.SetPackageVersion(semVersion);
        msBuildSettings.SetFileVersion(version);
        msBuildSettings.SetInformationalVersion(gitVersion.InformationalVersion);
        msBuildSettings.SetContinuousIntegrationBuild(!context.IsLocalBuild);
        msBuildSettings.WithProperty("RepositoryBranch", gitVersion.BranchName);
        msBuildSettings.WithProperty("RepositoryCommit", gitVersion.Sha);
        msBuildSettings.WithProperty("NoPackageAnalysis", "true");
    }
}
