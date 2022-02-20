using Build.Tasks.Packaging;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName(nameof(Package))]
[TaskDescription("Creates the packages (nuget)")]
[IsDependentOn(typeof(PackageNuget))]
public sealed class Package : FrostingTask<BuildContext>
{
}
