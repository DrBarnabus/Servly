using System.Reflection;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;
using Common.Utilities;

namespace Common.Tasks
{
    [TaskName(nameof(Default))]
    [TaskDescription("Shows this output")]
    // ReSharper disable once UnusedType.Global
    public class Default : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var tasks = entryAssembly?.FindAllDerivedTypes(typeof(IFrostingTask)).Where(x => !x.Name.Contains("Internal")).ToList();
            if (tasks is null) return;

            var defaultTask = tasks.Find(x => x.Name.Contains(nameof(Default)));
            if (defaultTask is not null && tasks.Remove(defaultTask))
                tasks.Insert(0, defaultTask);

            context.Information($"Available targets:{Environment.NewLine}");
            foreach (var task in tasks)
            {
                string arguments = task.GetTaskArguments();
                context.Information($"# {task.GetTaskDescription()}");

                string taskName = task.GetTaskName();
                string target = taskName != nameof(Default) ? $"-Target {taskName}" : string.Empty;
                context.Information($"  ./build.ps1 -Stage {entryAssembly?.GetName().Name} {target} {arguments}\n");
            }
        }
    }
}
