using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Core.IO;
using Cake.Coverlet;
using Common.Addins.Cake.Coverlet;

namespace Common.Utilities;

public static class ContextExtensions
{
    public static bool IsEnabled(this ICakeContext context, string variable, bool nullOrEmptyAsEnabled = true)
    {
        string? value = context.EnvironmentVariable(variable);
        return string.IsNullOrWhiteSpace(value) ? nullOrEmptyAsEnabled : bool.Parse(value);
    }

    public static bool ShouldRun(this ICakeContext context, bool criteria, string skipMessage)
    {
        if (criteria) return true;

        context.Information(skipMessage);
        return false;
    }

    public static IEnumerable<string> ExecuteCommand(this ICakeContext context, FilePath exe, string? args, DirectoryPath? workDir = null)
    {
        var processSettings = new ProcessSettings { Arguments = args, RedirectStandardOutput = true };
        if (workDir is not null)
            processSettings.WorkingDirectory = workDir;

        context.StartProcess(exe, processSettings, out var redirectedOutput);
        return redirectedOutput.ToList();
    }

    private static IEnumerable<string> ExecGitCmd(this ICakeContext context, string? cmd, DirectoryPath? workDir = null)
    {
        var gitExe = context.Tools.Resolve(context.IsRunningOnWindows() ? "git.exe" : "git");
        return context.ExecuteCommand(gitExe, cmd, workDir);
    }

    public static bool IsOriginalRepo(this ICakeContext context)
    {
        var buildSystem = context.BuildSystem();

        string repositoryName = string.Empty;
        if (buildSystem.IsRunningOnAzurePipelines)
            repositoryName = buildSystem.AzurePipelines.Environment.Repository.RepoName;
        else if (buildSystem.IsRunningOnGitHubActions)
            repositoryName = buildSystem.GitHubActions.Environment.Workflow.Repository;

        return !string.IsNullOrWhiteSpace(repositoryName)
               && StringComparer.OrdinalIgnoreCase.Equals($"{Constants.RepoOwner}/{Constants.Repository}", repositoryName);
    }

    public static bool IsMainBranch(this ICakeContext context)
    {
        string repositoryBranch = context.GetBranch();
        return !string.IsNullOrWhiteSpace(repositoryBranch)
               && StringComparer.OrdinalIgnoreCase.Equals("main", repositoryBranch);
    }

    public static string GetBranch(this ICakeContext context)
    {
        var buildSystem = context.BuildSystem();

        string repositoryBranch = context.ExecGitCmd("rev-parse --abbrev-ref HEAD").Single();
        if (buildSystem.IsRunningOnAzurePipelines)
            repositoryBranch = buildSystem.AzurePipelines.Environment.Repository.SourceBranchName;
        else if (buildSystem.IsRunningOnGitHubActions)
            repositoryBranch = buildSystem.GitHubActions.Environment.Workflow.Ref.Replace("refs/heads/", "");

        return repositoryBranch;
    }

    public static bool IsTagged(this ICakeContext context)
    {
        string sha = context.ExecGitCmd("rev-parse --verify HEAD").Single();
        bool isTagged = context.ExecGitCmd("tag --points-at " + sha).Any();

        return isTagged;
    }

    public static string GetOs(this ICakeContext context)
    {
        if (context.IsRunningOnWindows()) return "Windows";
        if (context.IsRunningOnLinux()) return "Linux";
        if (context.IsRunningOnMacOs()) return "macOs";
        return string.Empty;
    }

    public static string GetBuildAgent(this ICakeContext context)
    {
        var buildSystem = context.BuildSystem();
        return buildSystem.Provider switch
        {
            BuildProvider.Local => "Local",
            BuildProvider.AzurePipelines => "AzurePipelines",
            BuildProvider.GitHubActions => "GitHubActions",
            _ => string.Empty
        };
    }

    public static void StartGroup(this ICakeContext context, string title)
    {
        var buildSystem = context.BuildSystem();

        string startGroup = "[group]";
        if (buildSystem.IsRunningOnAzurePipelines)
            startGroup = "##[group]";
        else if (buildSystem.IsRunningOnGitHubActions)
            startGroup = "::group::";

        context.Information($"{startGroup}{title}");
    }
    public static void EndGroup(this ICakeContext context)
    {
        var buildSystem = context.BuildSystem();

        string endGroup = "[endgroup]";
        if (buildSystem.IsRunningOnAzurePipelines)
            endGroup = "##[endgroup]";
        else if (buildSystem.IsRunningOnGitHubActions)
            endGroup = "::endgroup::";

        context.Information($"{endGroup}");
    }

    public static void DotNetTest(this ICakeContext context, FilePath project, DotNetTestSettings settings, CoverletSettings coverletSettings)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var currentCustomization = settings.ArgumentCustomization;
        settings.ArgumentCustomization = args => ArgumentsProcessor.ProcessMSBuildArguments(
            coverletSettings,
            context.Environment,
            currentCustomization?.Invoke(args) ?? args,
            project);

        context.DotNetTest(project.FullPath, settings);
    }
}
