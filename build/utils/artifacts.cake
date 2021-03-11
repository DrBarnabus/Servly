public class BuildArtifacts
{
    public ICollection<BuildArtifact> All { get; private set; }

    public static BuildArtifacts GetArtifacts(FilePath[] artifacts)
    {
        var toArtifact = BuildArtifact("artifacts");
        var buildArtifacts = artifacts.Select(toArtifact).ToArray();

        return new BuildArtifacts {
            All = buildArtifacts.ToArray(),
        };
    }

    private static Func<FilePath, BuildArtifact> BuildArtifact(string containerName)
    {
        return artifactPath => new BuildArtifact(containerName: containerName, artifactPath: artifactPath);
    }
}

public class BuildArtifact
{
    public string ContainerName { get; private set; }
    public FilePath ArtifactPath { get; private set; }
    public string ArtifactName { get; private set; }

    public BuildArtifact(
        string containerName,
        FilePath artifactPath)
    {
        ContainerName = containerName;
        ArtifactPath = artifactPath.FullPath;
        ArtifactName = ArtifactPath.GetFilename().ToString();
    }
}