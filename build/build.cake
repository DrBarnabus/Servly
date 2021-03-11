#load "../build.cake"

Task("Clean")
    .Does<BuildParameters>((parameters) => 
    {
        Information("Cleaning directories...");

        CleanDirectories("./src/**/src/**/bin/" + parameters.Configuration);
        CleanDirectories("./src/**/src/**/obj");

        // Clean test
        CleanDirectories("./src/**/test/**/bin/" + parameters.Configuration);
        CleanDirectories("./src/**/test/**/obj");

        CleanDirectories(parameters.Paths.Directories.ToClean);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does<BuildParameters>((parameters) => 
    {
        var sln = "./Servly.sln";
        DotNetCoreRestore(sln, new DotNetCoreRestoreSettings
        {
            Verbosity = DotNetCoreVerbosity.Minimal,
            Sources = new [] { "https://api.nuget.org/v3/index.json" },
            MSBuildSettings = parameters.MSBuildSettings
        });
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does<BuildParameters>((parameters) => 
    {
        var sln = "./Servly.sln";
        var slnPath = MakeAbsolute(new DirectoryPath(sln));
        DotNetCoreBuild(slnPath.FullPath, new DotNetCoreBuildSettings
        {
            Verbosity = DotNetCoreVerbosity.Minimal,
            Configuration = parameters.Configuration,
            NoRestore = true,
            MSBuildSettings = parameters.MSBuildSettings
        });
    });