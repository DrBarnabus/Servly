using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;
using Common.Models;

namespace Build.Tasks.Packaging;

[TaskName(nameof(PackageNuget))]
[TaskDescription("Creates the nuget packages")]
public sealed class PackageNuget : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.EnsureDirectoryExists(Paths.Packages);

        PackageWithCli(context);
    }

    private static void PackageWithCli(BuildContext context)
    {
        var settings = new DotNetPackSettings
        {
            Configuration = context.MsBuildConfiguration,
            OutputDirectory = Paths.Packages,
            MSBuildSettings = context.MsBuildSettings,
        };

        string[] projectPaths = {
            "./src/Core/src/Servly.Core",
            "./src/Authentication/src/Servly.Authentication",
            "./src/Authentication/src/Servly.Authentication.AspNetCore"
        };

        foreach (string project in projectPaths)
            context.DotNetPack(project, settings);
    }
}
