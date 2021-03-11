#load "./utils.cake"
#load "./version.cake"
#load "./paths.cake"
#load "./artifacts.cake"
#load "./credentials.cake"

public class BuildParameters
{
    public string Target { get; set; }
    public string Configuration { get; set; }

    public const string MainRepoOwner = "DrBarnabus";
    public const string MainRepoName = "Servly";

    public string NetVersion50 { get; private set; } = "net5.0";

    public bool EnabledUnitTests { get; private set; }
    public bool EnabledPublishNuGet { get; private set; }

    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnLinux { get; private set; }
    public bool IsRunningOnMacOS { get; private set; }

    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAzurePipeline { get; private set; }

    public bool IsReleasingCI { get; private set; }

    public bool IsMainRepo { get; private set; }
    public bool IsMainBranch { get; private set; }
    public bool IsHotfixBranch { get; private set; }
    public bool IsReleaseBranch { get; private set; }
    public bool IsDevelopBranch { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPullRequest { get; private set; }

    public DotNetCoreMSBuildSettings MSBuildSettings { get; private set; }

    public BuildCredentials Credentials { get; private set; }
    public BuildVersion Version { get; private set; }
    public BuildPaths Paths { get; private set; }
    public BuildArtifacts Artifacts { get; private set; }
    public Dictionary<PlatformFamily, string[]> NativeRuntimes { get; private set; }

    public bool IsStableRelease() => !IsLocalBuild && IsMainBranch && !IsPullRequest && IsTagged;
    public bool IsPreviewRelease()    => !IsLocalBuild && (IsHotfixBranch && IsReleaseBranch) && !IsPullRequest && !IsTagged;
    public bool IsBetaRelease()    => !IsLocalBuild && IsDevelopBranch && !IsPullRequest && !IsTagged;

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var target = context.Argument("target", "Default");

        var buildSystem = context.BuildSystem();
        var isReleasingCI = buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted;

        return new BuildParameters {
            Target        = target,
            Configuration = context.Argument("configuration", "Release"),

            EnabledUnitTests = IsEnabled(context, "ENABLED_UNIT_TESTS"),
            EnabledPublishNuGet = IsEnabled(context, "ENABLED_PUBLISH_NUGET"),

            IsRunningOnUnix    = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsRunningOnLinux   = context.Environment.Platform.Family == PlatformFamily.Linux,
            IsRunningOnMacOS   = context.Environment.Platform.Family == PlatformFamily.OSX,

            IsLocalBuild             = buildSystem.IsLocalBuild,
            IsRunningOnAzurePipeline = buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted,

            IsReleasingCI = isReleasingCI,

            IsMainRepo = context.IsOnMainRepo(),
            IsMainBranch  = context.IsOnBranch("main"),
            IsHotfixBranch = context.IsOnBranchStartingWith("hotfix"),
            IsReleaseBranch = context.IsOnBranchStartingWith("release"),
            IsDevelopBranch = context.IsOnBranch("develop"),
            IsTagged      = context.IsBuildTagged(),
            IsPullRequest = buildSystem.IsPullRequest,

            MSBuildSettings = new DotNetCoreMSBuildSettings()
        };
    }

    public void Initialize(ICakeContext context, GitVersion gitVersion)
    {
        Credentials = BuildCredentials.GetCredentials(context);

        Version = BuildVersion.Calculate(context, this, gitVersion);

        Paths = BuildPaths.GetPaths(context, this, Configuration, Version);

        var buildArtifacts = context.GetFiles(Paths.Directories.Artifacts + "/*.*");
        Artifacts = BuildArtifacts.GetArtifacts(buildArtifacts.ToArray());

        NativeRuntimes = new Dictionary<PlatformFamily, string[]>
        {
            [PlatformFamily.Windows] = new[] { "win-x64", "win-x86" },
            [PlatformFamily.Linux]   = new[] { "linux-x64", "linux-musl-x64" },
            [PlatformFamily.OSX]     = new[] { "osx-x64" }
        };
        
        SetMSBuildSettingsVersion(MSBuildSettings, Version);
    }

    private void SetMSBuildSettingsVersion(DotNetCoreMSBuildSettings msBuildSettings, BuildVersion version)
    {
        msBuildSettings.WithProperty("Version", version.SemVersion);
        msBuildSettings.WithProperty("AssemblyVersion", version.Version);
        msBuildSettings.WithProperty("PackageVersion", version.SemVersion);
        msBuildSettings.WithProperty("FileVersion", version.Version);
        msBuildSettings.WithProperty("InformationalVersion", version.GitVersion.InformationalVersion);
        msBuildSettings.WithProperty("RepositoryBranch", version.GitVersion.BranchName);
        msBuildSettings.WithProperty("RepositoryCommit", version.GitVersion.Sha);
        msBuildSettings.WithProperty("NoPackageAnalysis", "true");
        msBuildSettings.WithProperty("RunningViaCake", "true");
    }
}
