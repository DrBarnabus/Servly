public static FilePath FindToolInPath(this ICakeContext context, string tool)
{
    var pathEnv = context.EnvironmentVariable("PATH");
    if (string.IsNullOrEmpty(pathEnv) || string.IsNullOrEmpty(tool)) return tool;

    var paths = pathEnv.Split(new []{ context.IsRunningOnUnix() ? ':' : ';'},  StringSplitOptions.RemoveEmptyEntries);
    return paths.Select(path => new DirectoryPath(path).CombineWithFilePath(tool)).FirstOrDefault(filePath => context.FileExists(filePath.FullPath));
}

public static bool IsOnMainRepo(this ICakeContext context)
{
    var buildSystem = context.BuildSystem();
    string repositoryName = null;

    if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
        repositoryName = buildSystem.AzurePipelines.Environment.Repository.RepoName;

    return !string.IsNullOrWhiteSpace(repositoryName) && StringComparer.OrdinalIgnoreCase.Equals($"{BuildParameters.MainRepoOwner}/{BuildParameters.MainRepoName}", repositoryName);
}

public static bool IsOnBranch(this ICakeContext context, string branch)
{
    var buildSystem = context.BuildSystem();
    string repositoryBranch = ExecGitCmd(context, "rev-parse --abbrev-ref HEAD").Single();

    if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
        repositoryBranch = buildSystem.AzurePipelines.Environment.Repository.SourceBranchName;

    return !string.IsNullOrWhiteSpace(repositoryBranch) && StringComparer.OrdinalIgnoreCase.Equals(branch, repositoryBranch);
}

public static bool IsOnBranchStartingWith(this ICakeContext context, string branchPrefix)
{
    var buildSystem = context.BuildSystem();
    string repositoryBranch = ExecGitCmd(context, "rev-parse --abbrev-ref HEAD").Single();

    if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
    {
        branchPrefix = branchPrefix.Insert(0, "refs/heads/");
        repositoryBranch = buildSystem.AzurePipelines.Environment.Repository.SourceBranch;
    }

    return !string.IsNullOrWhiteSpace(repositoryBranch) && repositoryBranch.StartsWith(branchPrefix, StringComparison.OrdinalIgnoreCase);
}

public static bool IsBuildTagged(this ICakeContext context)
{
    var sha = ExecGitCmd(context, "rev-parse --verify HEAD").Single();
    var isTagged = ExecGitCmd(context, "tag --points-at " + sha).Any();

    return isTagged;
}

public static bool IsEnabled(this ICakeContext context, string envVar, bool nullOrEmptyAsEnabled = true)
{
    var value = context.EnvironmentVariable(envVar);

    return string.IsNullOrWhiteSpace(value) ? nullOrEmptyAsEnabled : bool.Parse(value);
}

public static List<string> ExecuteCommand(this ICakeContext context, FilePath exe, string args)
{
    context.StartProcess(exe, new ProcessSettings { Arguments = args, RedirectStandardOutput = true }, out var redirectedOutput);

    return redirectedOutput.ToList();
}

public static List<string> ExecGitCmd(this ICakeContext context, string cmd)
{
    var gitExe = context.Tools.Resolve(context.IsRunningOnWindows() ? "git.exe" : "git");
    return context.ExecuteCommand(gitExe, cmd);
}

GitVersion GetVersion(BuildParameters parameters)
{
    var gitversionFilePath = $"artifacts/gitversion.json";
    var gitversionFile = GetFiles(gitversionFilePath).FirstOrDefault();
    GitVersion gitVersion = null;
    if (gitversionFile == null || parameters.IsLocalBuild)
    {
        var settings = new GitVersionSettings { OutputType = GitVersionOutput.Json };
        SetGitVersionTool(settings, parameters);

        gitVersion = GitVersion(settings);
        SerializeJsonToPrettyFile(gitversionFilePath, gitVersion);
    }
    else
    {
        gitVersion = DeserializeJsonFromFile<GitVersion>(gitversionFile);
    }

    RunGitVersionOnCI(parameters);

    return gitVersion;
}

void RunGitVersionOnCI(BuildParameters parameters)
{
    // set the CI build version number with GitVersion
    if (!parameters.IsLocalBuild)
    {
        var settings = new GitVersionSettings { LogFilePath = "console", OutputType = GitVersionOutput.BuildServer };
        SetGitVersionTool(settings, parameters);

        GitVersion(settings);
    }
}

GitVersionSettings SetGitVersionTool(GitVersionSettings settings, BuildParameters parameters)
{
    var gitversionTool = GetGitVersionToolLocation(parameters);

    settings.ToolPath = gitversionTool;
    settings.ArgumentCustomization = args => args.Render();

    return settings;
}

FilePath GetGitVersionToolLocation(BuildParameters parameters)
{
    string toolName = IsRunningOnUnix() ? "dotnet-gitversion" : "dotnet-gitversion.exe";
    return GetFiles($"./tools/{toolName}").SingleOrDefault();
}

void LogGroup(string title, Action action)
{
    StartGroup(title);
    action();
    EndGroup();
}

T LogGroup<T>(string title, Func<T> action)
{
    StartGroup(title);
    var result = action();
    EndGroup();

    return result;
}

void StartGroup(string title)
{
    var buildSystem = Context.BuildSystem();

    var startGroup = "[group]";
    if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
        startGroup = "##[group]";

    Information($"{startGroup}{title}");
}

void EndGroup()
{
    var buildSystem = Context.BuildSystem();

    var endgroup = "[endgroup]";
    if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
        endgroup = "##[endgroup]";

    Information($"{endgroup}");
}
