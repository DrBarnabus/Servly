using Cake.Core;
using Common;
using Release.Models;

namespace Release;

public class BuildContext : BuildContextBase
{
    public BuildContext(ICakeContext context) : base(context)
    {
    }

    public Credentials? Credentials { get; set; }
}
