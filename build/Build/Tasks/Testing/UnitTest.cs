using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core.IO;
using Cake.Coverlet;
using Cake.Frosting;
using Cake.Incubator.LoggingExtensions;
using Common;
using Common.Models;
using Common.Utilities;

namespace Build.Tasks.Testing;

[TaskName(nameof(UnitTest))]
[TaskDescription("Run the unit tests")]
[TaskArgument(Arguments.DotNetTarget, Constants.NetVersion60)]
[IsDependentOn(typeof(Build))]
public sealed class UnitTest : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context)
    {
        return context.EnableUnitTests;
    }

    public override void Run(BuildContext context)
    {
        string dotnetTarget = context.Argument(Arguments.DotNetTarget, string.Empty);

        string[] frameworks = { Constants.NetVersion60 };
        if (!string.IsNullOrWhiteSpace(dotnetTarget))
        {
            if (!frameworks.Contains(dotnetTarget, StringComparer.OrdinalIgnoreCase))
                throw new Exception($"Dotnet Target {dotnetTarget} is not supported at the moment");

            frameworks = new[] { dotnetTarget };
        }

        foreach (string framework in frameworks)
        {
            var projects = context.GetFiles($"{Paths.Src}/**/test/**/*.{{Unit,Functional}}Tests.csproj");
            foreach (var project in projects)
                TestProjectForTarget(context, project, framework);
        }
    }

    public override void OnError(Exception exception, BuildContext context)
    {
        var error = (exception as AggregateException)?.InnerExceptions[0];
        context.Error(error.Dump());
        throw exception;
    }

    public override void Finally(BuildContext context)
    {
        var testResultsFiles = context.GetFiles($"{Paths.TestResults}/*.results.xml");
        if (!context.IsAzurePipelineBuild || !testResultsFiles.Any()) return;

        context.BuildSystem().AzurePipelines.Commands.PublishTestResults( new AzurePipelinesPublishTestResultsData
        {
            TestResultsFiles = testResultsFiles.ToArray(),
            Platform = context.Environment.Platform.Family.ToString(),
            TestRunner = AzurePipelinesTestRunnerType.NUnit
        });
    }

    private static void TestProjectForTarget(BuildContext context, FilePath project, string framework)
    {
        string projectName = $"{project.GetFilenameWithoutExtension()}.{framework}";
        var resultsPath = context.MakeAbsolute(Paths.TestResults.CombineWithFilePath($"{projectName}.results.xml"));

        var settings = new DotNetTestSettings
        {
            Framework = framework,
            NoBuild = true,
            NoRestore = true,
            Configuration = context.MsBuildConfiguration,
            TestAdapterPath = new DirectoryPath("."),
            Loggers = new[] { $"trx;LogFileName={resultsPath}" }
        };

        var coverletSettings = new CoverletSettings
        {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.cobertura,
            CoverletOutputDirectory = Paths.TestResults,
            CoverletOutputName = $"{projectName}.coverage.xml",
            Exclude = new List<string> { "[Servly*.UnitTests]*", "[Servly*.FunctionalTests]*", "[*.Sample]*" }
        };

        context.DotNetTest(project.FullPath, settings, coverletSettings);
    }
}
