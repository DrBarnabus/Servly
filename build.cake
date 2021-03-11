// Install addins.
#addin "nuget:?package=Cake.Compression&version=0.2.6"
#addin "nuget:?package=Cake.Json&version=6.0.0"
#addin "nuget:?package=Cake.Incubator&version=6.0.0"

#addin "nuget:?package=Newtonsoft.Json&version=12.0.3"
#addin "nuget:?package=SharpZipLib&version=1.3.1"
#addin "nuget:?package=Cake.Codecov&version=1.0.0"
#addin "nuget:?package=Cake.Coverlet&version=2.5.4"

// Install .NET Core Global Tools
#tool "dotnet:?package=GitVersion.Tool&version=5.6.6"

#load "./build/utils/parameters.cake"
#load "./build/utils/utils.cake"

#load "./build/build.cake"
#load "./build/pack.cake"
#load "./build/publish.cake"

using System.Diagnostics;

bool publishingError = false;

Setup<BuildParameters>(context =>
{
    return LogGroup<BuildParameters>("Cake Setup", () => 
    {
        try
        {
            EnsureDirectoryExists("artifacts");

            var parameters = BuildParameters.GetParameters(context);
            
            var gitVersion = GetVersion(parameters);
            parameters.Initialize(context, gitVersion);

            // Increase verbosity?
            if ((parameters.IsMainBranch || parameters.IsHotfixBranch || parameters.IsReleaseBranch || parameters.IsDevelopBranch) && (context.Log.Verbosity != Verbosity.Diagnostic))
            {
                Information("Increasing verbosity to diagnostic.");
                context.Log.Verbosity = Verbosity.Diagnostic;
            }

            if (parameters.IsLocalBuild)
                Information("Building locally");

            if (parameters.IsRunningOnAzurePipeline)
                Information("Building on AzurePipeline");

            Information("Building version {0} of Servly ({1}, {2})",
                parameters.Version.SemVersion,
                parameters.Configuration,
                parameters.Target);

            Information("Repository info : IsMainRepo {6}, IsMainBranch {0}, IsHotfixBranch {1}, IsReleaseBranch {2}, IsDevelopBranch {3}, IsTagged: {4}, IsPullRequest: {5}",
                parameters.IsMainBranch,
                parameters.IsHotfixBranch,
                parameters.IsReleaseBranch,
                parameters.IsDevelopBranch,
                parameters.IsTagged,
                parameters.IsPullRequest,
                parameters.IsMainRepo);

            return parameters;
        }
        catch (Exception ex)
        {
            Error(ex.Dump());
            return null;
        }
    });
});

Teardown<BuildParameters>((context, parameters) =>
{
    LogGroup("Cake Teardown", () =>
    {
        try
        {
            Information("Starting Teardown...");

            Information("Repository info : IsMainRepo {6}, IsMainBranch {0}, IsHotfixBranch {1}, IsReleaseBranch {2}, IsDevelopBranch {3}, IsTagged: {4}, IsPullRequest: {5}",
                parameters.IsMainBranch,
                parameters.IsHotfixBranch,
                parameters.IsReleaseBranch,
                parameters.IsDevelopBranch,
                parameters.IsTagged,
                parameters.IsPullRequest,
                parameters.IsMainRepo);

            Information("Finished running tasks.");
        }
        catch (Exception exception)
        {
            Error(exception.Dump());
        }
    });
});


TaskSetup(setupContext =>
{
    var message = string.Format("Task: {0}", setupContext.Task.Name);
    StartGroup(message);
});

TaskTeardown(teardownContext =>
{
    var message = string.Format("Task: {0}", teardownContext.Task.Name);
    EndGroup();
});

Task("Pack")
    .IsDependentOn("Pack-NuGet")
    .Does<BuildParameters>((parameters) =>
    {
        Information("The build artifacts: \n");
        foreach(var artifact in parameters.Artifacts.All)
            if (FileExists(artifact.ArtifactPath)) 
                Information("Artifact: {0}", artifact.ArtifactPath);
    })
    .ReportError((ex) => 
    {
        Error(ex.Dump());
    });

Task("Publish-CI")
    .IsDependentOn("Publish-AzurePipelines")
    .IsDependentOn("Publish-NuGet")
    .Finally(() => 
    {
        if (publishingError)
            throw new Exception("An error occurred during the publishing of Servly.");
    });

Task("Publish-Coverage")
    .IsDependentOn("Publish-Coverage-Internal")
    .Finally(() => 
    {
        if (publishingError)
            throw new Exception("An error occurred during the publishing of Servly.");
    });

Task("Publish-NuGet")
    .IsDependentOn("Publish-NuGet-Internal")
    .Finally(() => 
    {
        if (publishingError)
            throw new Exception("An error occurred during the publishing of Servly.");
    });

Task("Publish")
    .IsDependentOn("Publish-CI")
    .IsDependentOn("Publish-Coverage")
    .IsDependentOn("Publish-NuGet")
    .Finally(() => 
    {
        if (publishingError)
            throw new Exception("An error occurred during the publishing of Servly. All publishing tasks have been attempted.");
    });

Task("Default")
    .IsDependentOn("Pack");

var target = Argument("target", "Default");
RunTarget(target);