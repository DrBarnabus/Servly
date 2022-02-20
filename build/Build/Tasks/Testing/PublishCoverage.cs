using Cake.Codecov;
using Cake.Common.IO;
using Cake.Frosting;
using Common.Models;
using Common.Utilities;

namespace Build.Tasks.Testing;

[TaskName(nameof(PublishCoverage))]
[TaskDescription("Publishes the test coverage")]
[IsDependentOn(typeof(UnitTest))]
public sealed class PublishCoverage : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        bool shouldRun = true;
        shouldRun &= context.ShouldRun(context.IsOnLinux, $"{nameof(PublishCoverage)} only works on Linux agents.");
        shouldRun &= context.ShouldRun(context.IsOnMainBranchOriginalRepo, $"{nameof(PublishCoverage)} only works on main branch original repository.");
        shouldRun &= context.ShouldRun(!string.IsNullOrEmpty(context.Credentials?.CodeCov?.Token), $"{nameof(PublishCoverage)} only works when 'CODECOV_TOKEN' is supplied.");

        return shouldRun;
    }

    public override void Run(BuildContext context)
    {
        var coverageFiles = context.GetFiles($"{Paths.TestResults}/*.coverage.xml");

        string? token = context.Credentials?.CodeCov?.Token;
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Could not resolve CodeCov token.");

        foreach (var coverageFile in coverageFiles)
            context.Codecov(new CodecovSettings
            {
                Files = new[] { coverageFile.ToString() },
                Token = token
            });
    }
}
