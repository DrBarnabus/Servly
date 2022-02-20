using Build.Models;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Core;
using Common;

namespace Build;

public class BuildContext : BuildContextBase
{
    public BuildContext(ICakeContext context) : base(context)
    {
    }

    public string MsBuildConfiguration { get; set; } = "Release";

    public bool EnableUnitTests { get; set; }

    public Credentials? Credentials { get; set; }

    public DotNetMSBuildSettings MsBuildSettings { get; } = new();
}
