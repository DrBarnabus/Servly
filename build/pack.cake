#load "../build.cake"

Task("Test")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledUnitTests, "Unit Tests were disabled.")
    .IsDependentOn("Build")
    .Does<BuildParameters>(parameters =>
    {
        var frameworks = new List<string> { parameters.NetVersion50 };
        var testResultsPath = parameters.Paths.Directories.TestResultsOutput;

        foreach (var framework in frameworks)
        {
            // run using dotnet test
            var actions = new List<Action>();
            var projects = GetFiles("./src/**/test/**/*.UnitTests.csproj");

            foreach (var project in projects)
            {
                actions.Add(() =>
                {
                    var projectName = $"{project.GetFilenameWithoutExtension()}.{framework}";
                    var settings = new DotNetCoreTestSettings
                    {
                        Framework = framework,
                        NoBuild = true,
                        NoRestore = true,
                        Configuration = parameters.Configuration
                    };

                    if (!parameters.IsRunningOnMacOS)
                    {
                        settings.TestAdapterPath = new DirectoryPath(".");
                        var resultsPath = MakeAbsolute(testResultsPath.CombineWithFilePath($"{projectName}.results.xml"));
                        settings.Loggers.Add($"trx;LogFileName={resultsPath}");
                    }

                    var coverletSettings = new CoverletSettings
                    {
                        CollectCoverage = true,
                        CoverletOutputFormat = CoverletOutputFormat.opencover,
                        CoverletOutputDirectory = testResultsPath,
                        CoverletOutputName = $"{projectName}.coverage.xml"
                    };

                    DotNetCoreTest(project.FullPath, settings, coverletSettings);
                });
            }

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = -1,
                CancellationToken = default
            };

            Parallel.Invoke(options, actions.ToArray());
        }
    })
    .ReportError(exception =>
    {
        var error = (exception as AggregateException).InnerExceptions[0];
        Error(error.Dump());
    })
    .Finally(() =>
    {
        var parameters = Context.Data.Get<BuildParameters>();
        if (parameters.IsRunningOnAzurePipeline)
        {
            var testResultsFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.results.xml");
            if (testResultsFiles.Any())
            {
                var data = new AzurePipelinesPublishTestResultsData
                {
                    TestRunTitle = $"Tests_{parameters.Configuration}_{Context.Environment.Platform.Family.ToString()}",
                    TestResultsFiles = testResultsFiles.ToArray(),
                    TestRunner = AzurePipelinesTestRunnerType.VSTest
                };

                AzurePipelines.Commands.PublishTestResults(data);
            }
        }
    });

Task("Pack-NuGet")
    .IsDependentOn("Test")
    .Does<BuildParameters>(parameters =>
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = parameters.Configuration,
            NoRestore = true,
            OutputDirectory = parameters.Paths.Directories.Artifacts,
            MSBuildSettings = parameters.MSBuildSettings
        };

        var projects = new List<string> { "./src/Servly.Core/src/Servly.Core/Servly.Core.csproj" };
        foreach (var project in projects)
            DotNetCorePack(project, settings);
    });