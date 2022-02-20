using Cake.Core;
using Cake.Frosting;
using Common.Utilities;

namespace Common;

public class BuildTaskLifetime : FrostingTaskLifetime
{
    public override void Setup(ICakeContext context, ITaskSetupContext info) => context.StartGroup($"Task: {info.Task.Name}");

    public override void Teardown(ICakeContext context, ITaskTeardownContext info) => context.EndGroup();
}
