public class BuildVersion
{
    public GitVersion GitVersion { get; private set; }
    public string Version { get; private set; }
    public string SemVersion { get; private set; }

    public static BuildVersion Calculate(ICakeContext context, BuildParameters parameters, GitVersion gitVersion)
    {
        return new BuildVersion
        {
            GitVersion = gitVersion,
            Version = gitVersion.MajorMinorPatch,
            SemVersion = gitVersion.SemVer
        };
    }
}