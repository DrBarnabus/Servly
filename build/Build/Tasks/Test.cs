using Build.Tasks.Testing;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName(nameof(Test))]
[TaskDescription("(CI only) Run the tests and publish the results")]
[IsDependentOn(typeof(PublishCoverage))]
public sealed class Test : FrostingTask<BuildContext>
{
}
