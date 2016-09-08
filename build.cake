// this script is inspired by https://github.com/SharpeRAD/Cake.SqlServer/blob/master/build.cake
#tool "xunit.runner.console"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var appName = "Cake.SqlServer";





//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

// Get whether or not this is a local build.
var local = BuildSystem.IsLocalBuild;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

// Parse release notes.
var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

//TODO use GitVersion
// Get version.
var buildNumber = AppVeyor.Environment.Build.Number;
// var version = releaseNotes.Version.ToString();
var appVeyorVersion = AppVeyor.Environment.Build.Version;
var version = local ? "1.0.1" : appVeyorVersion;
// var semVersion = local ? version : (version + string.Concat("-build-", buildNumber));
var semVersion = local ? version : appVeyorVersion;


// Define directories.
var buildDir = "./src/Cake.SqlServer/bin/" + configuration;
var buildTestDir = "./src/Tests/bin/" + configuration;

var buildResultDir = "./build-results/v" + semVersion;
var nugetRoot = buildResultDir + "/nuget";
var binDir = buildResultDir + "/bin";

//Get Solutions
var solutions  = GetFiles("./src/*.sln");

// Package
var zipPackage = buildResultDir + "/Cake-SqlServer-v" + semVersion + ".zip";





///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    //Executed BEFORE the first task.
    Information("Building version {0} of {1}.", semVersion, appName);
    Information("Tools dir: {0}.", EnvironmentVariable("CAKE_PATHS_TOOLS"));
});

Teardown(context =>
{
    // Executed AFTER the last task.
    Information("Finished building version {0} of {1}.", semVersion, appName);
});





///////////////////////////////////////////////////////////////////////////////
// PREPARE
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    Information("Cleaning old files");

    CleanDirectories(new DirectoryPath[]
    {
        buildDir,
        buildTestDir,
        buildResultDir,
        binDir, 
        nugetRoot
    });
});

Task("Restore-Nuget-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}", solution);

        NuGetRestore(solution);
    }
});





///////////////////////////////////////////////////////////////////////////////
// BUILD
///////////////////////////////////////////////////////////////////////////////

Task("Patch-Assembly-Info")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() =>
{
    var file = "./src/Cake.SqlServer/Properties/AssemblyInfo.cs";

    CreateAssemblyInfo(file, new AssemblyInfoSettings
    {
        Product = appName,
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = "Copyright (c) 2016 - " + DateTime.Now.Year.ToString() + " AMV Software"
    });
});

Task("Build")
    .IsDependentOn("Patch-Assembly-Info")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
        Information("Building {0}", solution);

        MSBuild(solution, settings =>
            settings.SetPlatformTarget(PlatformTarget.MSIL)
                    .WithTarget("Build")
                    .SetConfiguration(configuration));
    }
});



///////////////////////////////////////////////////////////////////////////////
// PACKAGE
///////////////////////////////////////////////////////////////////////////////

Task("Copy-Files")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Addin
    CopyFileToDirectory(buildDir + "/Cake.SqlServer.dll", binDir);
    CopyFileToDirectory(buildDir + "/Cake.SqlServer.pdb", binDir);

    CopyFiles(new FilePath[] { "LICENSE", "README.md", "ReleaseNotes.md" }, binDir);
});

Task("Zip-Files")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    Zip(binDir, zipPackage);
});



Task("Create-NuGet-Packages")
    .IsDependentOn("Zip-Files")
    .Does(() =>
{
    NuGetPack("./src/Cake.SqlServer/Cake.SqlServer.nuspec", new NuGetPackSettings
    {
        Version = version,
        ReleaseNotes = releaseNotes.Notes.ToArray(),
        BasePath = binDir,
        OutputDirectory = nugetRoot,
        Symbols = false,
        NoPackageAnalysis = true
    });
});

Task("Publish-Nuget")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => isRunningOnAppVeyor)
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
    // Resolve the API key.
    var apiKey = EnvironmentVariable("NUGET_API_KEY");

    if(string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }



    // Push the package.
    var package = nugetRoot + "/Cake.SqlServer." + version + ".nupkg";

    NuGetPush(package, new NuGetPushSettings
    {
        ApiKey = apiKey,
        Source = "https://www.nuget.org/api/v2/package"
    });
});





///////////////////////////////////////////////////////////////////////////////
// APPVEYOR
///////////////////////////////////////////////////////////////////////////////

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(semVersion);
});

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Zip-Files")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UploadArtifact(zipPackage);
});






//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Package")
    .IsDependentOn("Zip-Files")
    .IsDependentOn("Create-NuGet-Packages");

Task("Publish")
    .IsDependentOn("Publish-Nuget");

Task("AppVeyor")
    .IsDependentOn("Publish")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Upload-AppVeyor-Artifacts");



Task("Default")
    .IsDependentOn("Package");





///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);