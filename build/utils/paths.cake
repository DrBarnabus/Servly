#load "./parameters.cake"

public class BuildPaths
{
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(ICakeContext context, BuildParameters parameters, string configuration, BuildVersion version)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        if (string.IsNullOrEmpty(configuration))
            throw new ArgumentNullException(nameof(configuration));

        if (version == null)
            throw new ArgumentNullException(nameof(version));

        var semVersion = version.SemVersion;

        var rootDir                       = (DirectoryPath)(context.Directory("."));
        var sourceDir                     = rootDir.Combine("src");
        var artifactsRootDir              = rootDir.Combine("artifacts");
        var artifactsDir                  = artifactsRootDir.Combine("v" + semVersion);

        var testResultsOutputDir = artifactsDir.Combine("test-results");

        // Directories
        var buildDirectories = new BuildDirectories(
            rootDir,
            sourceDir,
            artifactsRootDir,
            artifactsDir,
            testResultsOutputDir
        );

        return new BuildPaths
        {
            Directories = buildDirectories
        };
    }
}

public class BuildDirectories
{
    public DirectoryPath Root { get; private set; }
    public DirectoryPath Source { get; private set; }
    public DirectoryPath ArtifactsRoot { get; private set; }
    public DirectoryPath Artifacts { get; private set; }
    public DirectoryPath TestResultsOutput { get; private set; }

    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath rootDir,
        DirectoryPath sourceDir,
        DirectoryPath artifactsRootDir,
        DirectoryPath artifactsDir,
        DirectoryPath testResultsOutputDir
        )
    {
        Root = rootDir;
        Source = sourceDir;
        ArtifactsRoot = artifactsRootDir;
        Artifacts = artifactsDir;
        TestResultsOutput = testResultsOutputDir;
        ToClean = new[] {
            Artifacts,
            TestResultsOutput
        };
    }
}