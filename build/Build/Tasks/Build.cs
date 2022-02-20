using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;
using Common;

namespace Build.Tasks;

[TaskName(nameof(Build))]
[TaskDescription("Builds the solution")]
[IsDependentOn(typeof(Clean))]
public sealed class Build : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Building Solution...");
        const string sln = "./Servly.sln";

        context.DotNetRestore(sln, new DotNetRestoreSettings
        {
            Verbosity = DotNetVerbosity.Minimal,
            Sources = new[] { Constants.NugetOrgUrl },
            MSBuildSettings = context.MsBuildSettings
        });

        context.DotNetBuild(sln, new DotNetBuildSettings
        {
            Verbosity = DotNetVerbosity.Minimal,
            Configuration = context.MsBuildConfiguration,
            NoRestore = true,
            MSBuildSettings = context.MsBuildSettings
        });
    }
}
