#load "../build.cake"

Task("Publish-AzurePipelines")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnLinux, "Publish-NuGet works only on Linux agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-AzurePipelines works only on AzurePipelines.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreviewRelease() || parameters.IsBetaRelease(), "Publish-AzurePipelines works only on stable, preview or beta builds.")
    .Does<BuildParameters>(parameters =>
    {
        foreach (var artifact in parameters.Artifacts.All)
            if (FileExists(artifact.ArtifactPath))
                AzurePipelines.Commands.UploadArtifact("", artifact.ArtifactPath, "artifacts");
    })
    .OnError(exception =>
    {
        Information("Publish-AzurePipelines Task failed, but continuing with next Task...");
        Error(exception.Dump());
        publishingError = true;
    });

Task("Publish-Coverage-Internal")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnLinux, "Publish-NuGet works only on Linux agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-Coverage works only on AzurePipelines.")
    .Does<BuildParameters>(parameters =>
    {
        var coverageFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.coverage.*.xml");

        var token = parameters.Credentials.CodeCov.Token;
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Could not resolve CodeCov token.");

        foreach (var coverageFile in coverageFiles)
        {
            Codecov(new CodecovSettings
            {
                Files = new [] { coverageFile.ToString() },
                Token = token
            });
        }
    })
    .OnError(exception =>
    {
        Information("Publish-Coverage Task Failed, but continuing with next Task...");
        Error(exception.Dump());
        publishingError = true;
    });

Task("Publish-NuGet-Internal")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledPublishNuGet, "Publish-NuGet was disabled.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnLinux, "Publish-NuGet works only on Linux agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-NuGet works only on AzurePipelines.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreviewRelease() || parameters.IsBetaRelease(), "Publish-NuGet works only on stable, preview or beta builds.")
    .Does<BuildParameters>(parameters =>
    {
        var apiKey = parameters.Credentials.NuGet.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Could not resolve NuGet API Key.");

        var apiUrl = parameters.Credentials.NuGet.ApiUrl;
        if (string.IsNullOrEmpty(apiUrl))
            throw new InvalidOperationException("Could not resolve NuGet API Url.");

        foreach (var nupkg in GetFiles(parameters.Paths.Directories.Artifacts + "/*.nupkg"))
        {
            NuGetPush(nupkg, new NuGetPushSettings
            {
                ApiKey = apiKey,
                Source = apiUrl
            });
        }
    })
    .OnError(exception =>
    {
        Information("Publish-NuGet Task Failed, but continuing with next Task...");
        Error(exception.Dump());
        publishingError = true;
    });
