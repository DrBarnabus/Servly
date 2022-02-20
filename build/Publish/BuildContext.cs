using Cake.Core;
using Common;
using Publish.Models;

namespace Publish;

public class BuildContext : BuildContextBase
{
    public BuildContext(ICakeContext context) : base(context)
    {
    }

    public Credentials? Credentials { get; set; }

    public List<NugetPackage> Packages { get; set; } = new();
}
