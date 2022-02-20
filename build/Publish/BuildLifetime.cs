using Cake.Common.IO;
using Common;
using Common.Models;
using Common.Utilities;
using Publish.Models;

namespace Publish;

public class BuildLifetime : BuildLifetimeBase<BuildContext>
{
    public override void Setup(BuildContext context)
    {
        base.Setup(context);

        context.Credentials = Credentials.GetCredentials(context);

        if (context.Version?.SemVersion != null)
        {
            string version = context.Version?.SemVersion!;

            var packageFiles = context.GetFiles(Paths.Packages + "/*.nupkg");
            foreach (var packageFile in packageFiles)
            {
                string packageName = packageFile.GetFilenameWithoutExtension().ToString()[..^(version.Length + 1)].ToLower();
                context.Packages.Add(new NugetPackage(packageName, packageFile));
            }
        }

        context.StartGroup("Build Setup");
        LogBuildInformation(context);
        context.EndGroup();
    }
}
