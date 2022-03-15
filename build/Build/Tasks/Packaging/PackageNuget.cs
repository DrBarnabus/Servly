using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;
using Common.Models;

namespace Build.Tasks.Packaging;

[TaskName(nameof(PackageNuget))]
[TaskDescription("Creates the nuget packages")]
[IsDependentOn(typeof(Build))]
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
            "./src/AspNetCore.Idempotency/src/Servly.AspNetCore.Idempotency",
            "./src/AspNetCore.ModelBinding/src/Servly.AspNetCore.ModelBinding.Hybrid",
            "./src/Authentication/src/Servly.Authentication",
            "./src/Authentication/src/Servly.AspNetCore.Authentication",
            "./src/Core/src/Servly.Core",
            "./src/Core/src/Servly.AspNetCore.Core"
        };

        foreach (string project in projectPaths)
            context.DotNetPack(project, settings);
    }
}
