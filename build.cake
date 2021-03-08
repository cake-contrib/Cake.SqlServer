#tool "nuget:?package=NUnit.ConsoleRunner&version=3.12.0"
#tool "nuget:?package=GitVersion.CommandLine&version=5.6.6"
#load "./parameters.cake"


var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
bool publishingError = false;

BuildParameters parameters = BuildParameters.GetParameters(Context);

Setup(context =>
{
    parameters.Initialize(context);

    Information("SemVersion: {0}", parameters.SemVersion);
    Information("Version: {0}", parameters.Version);
    Information("Building from branch: " + AppVeyor.Environment.Repository.Branch);
});

Teardown(context =>
{
    Information("Cake.. NOM-NOM");
});


Task("debug")
    .Does(() => {
        Information("debug");
    });

Task("Clean")
    .Does(() =>
{
    // Clean solution directories.
    Information("Cleaning old files");


    CleanDirectories(new DirectoryPath[]{
        parameters.BuildDir,
        parameters.BuildResultDir,
        Directory("./src/Tests/bin/"),
        Directory("./src/Tests/obj/"),
        Directory(BuildParameters.ProjectDir + "bin"),
        Directory(BuildParameters.ProjectDir + "obj"),
    });
});



Task("Restore-Nuget-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Restoring packages in {0}", BuildParameters.Solution);

    DotNetCoreRestore(BuildParameters.Solution);
});





Task("Build")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() =>
{
    Information("Building {0}", BuildParameters.Solution);

    DotNetCoreMSBuild(
        BuildParameters.Solution,
        new DotNetCoreMSBuildSettings()
            .WithTarget("Build")
            .SetConfiguration(configuration));
});

Task("Start-LocalDB")
    .Description(@"Starts LocalDB - executes the following: C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe create -s")
    .WithCriteria(() => !parameters.SkipTests)
    .Does(() =>
    {
        var sqlLocalDbPath15 = @"c:\Program Files\Microsoft SQL Server\150\Tools\Binn\SqlLocalDB.exe";
        var sqlLocalDbPath14 = @"c:\Program Files\Microsoft SQL Server\140\Tools\Binn\SqlLocalDB.exe";
        var sqlLocalDbPath13 = @"c:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe";
        var sqlLocalDbPath12 = @"C:\Program Files\Microsoft SQL Server\120\Tools\Binn\SqlLocalDB.exe";

        if(FileExists(sqlLocalDbPath15))
        {
            StartProcess(sqlLocalDbPath15, new ProcessSettings(){ Arguments=@"create ""MSSqlLocalDb"" -s" });
            return;
        }

        if(FileExists(sqlLocalDbPath14))
        {
            StartProcess(sqlLocalDbPath14, new ProcessSettings(){ Arguments=@"create ""MSSqlLocalDb"" -s" });
            return;
        }

        if(FileExists(sqlLocalDbPath13))
        {
            StartProcess(sqlLocalDbPath13, new ProcessSettings(){ Arguments=@"create ""MSSqlLocalDb"" -s" });
            return;
        }

        if(FileExists(sqlLocalDbPath12))
        {
            StartProcess(sqlLocalDbPath12, new ProcessSettings(){ Arguments=@"create ""MSSqlLocalDb"" -s" });
            return;
        }

        Information("Unable to start LocalDB");
        throw new Exception("LocalDB v12 is not installed. Can't complete tests");
    });


Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .IsDependentOn("Start-LocalDB")
    .WithCriteria(() => !parameters.SkipTests)
    .Does(() =>
	{
		var testsFile ="./src/**/bin/" + configuration + "/**/Tests.dll";
		Information(testsFile);

		NUnit3(testsFile, new NUnit3Settings {
            Results = new List<NUnit3Result>(){
                new NUnit3Result(){
                    FileName = parameters.TestResultsFile,
                }
            }
		});
    })
    .Finally(() =>
    {
        if(FileExists(parameters.TestResultsFile) && parameters.IsRunningOnAppVeyor)
        {
            Information("File {0} Exists!", parameters.TestResultsFile);
            AppVeyor.UploadTestResults(parameters.TestResultsFile, AppVeyorTestResultsType.NUnit3);
        }
    });



Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
	{
        EnsureDirectoryExists(parameters.ResultBinDir);
        EnsureDirectoryExists(parameters.ResultBinDir + "/net461");
        EnsureDirectoryExists(parameters.ResultBinDir + "/netstandard2.0");
        EnsureDirectoryExists(parameters.ResultBinDir + "/net5.0");
        EnsureDirectoryExists(parameters.ResultBinDir + "/net5.0/ref");

        CopyFileToDirectory(parameters.BuildDir + "/net461/Cake.SqlServer.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Cake.SqlServer.pdb", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Cake.SqlServer.xml", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.SqlClient.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.SqlClient.SNI.x64.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.SqlClient.SNI.x64.pdb", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.SqlClient.SNI.x86.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.SqlClient.SNI.x86.pdb", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.Tools.Schema.Sql.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Data.Tools.Utilities.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.Identity.Client.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.IdentityModel.JsonWebTokens.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.IdentityModel.Logging.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.IdentityModel.Protocols.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.IdentityModel.Protocols.OpenIdConnect.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.IdentityModel.Tokens.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.SqlServer.Dac.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.SqlServer.Dac.Extensions.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.SqlServer.TransactSql.ScriptDom.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/Microsoft.SqlServer.Types.dll", parameters.ResultBinDir + "/net461");
        CopyFileToDirectory(parameters.BuildDir + "/net461/System.IdentityModel.Tokens.Jwt.dll", parameters.ResultBinDir + "/net461");

        CopyFileToDirectory(parameters.BuildDir + "/netstandard2.0/Cake.SqlServer.dll", parameters.ResultBinDir + "/netstandard2.0");
        CopyFileToDirectory(parameters.BuildDir + "/netstandard2.0/Cake.SqlServer.pdb", parameters.ResultBinDir + "/netstandard2.0");
        CopyFileToDirectory(parameters.BuildDir + "/netstandard2.0/Cake.SqlServer.xml", parameters.ResultBinDir + "/netstandard2.0");
        CopyFileToDirectory(parameters.BuildDir + "/netstandard2.0/Cake.SqlServer.deps.json", parameters.ResultBinDir + "/netstandard2.0");

        CopyFileToDirectory(parameters.BuildDir + "/net5.0/Cake.SqlServer.dll", parameters.ResultBinDir + "/net5.0");
        CopyFileToDirectory(parameters.BuildDir + "/net5.0/Cake.SqlServer.pdb", parameters.ResultBinDir + "/net5.0");
        CopyFileToDirectory(parameters.BuildDir + "/net5.0/Cake.SqlServer.xml", parameters.ResultBinDir + "/net5.0");
        CopyFileToDirectory(parameters.BuildDir + "/net5.0/Cake.SqlServer.deps.json", parameters.ResultBinDir + "/net5.0");
        CopyFileToDirectory(parameters.BuildDir + "/net5.0/ref/Cake.SqlServer.dll", parameters.ResultBinDir + "/net5.0/ref");

        CopyFiles(new FilePath[] { "LICENSE", "README.md", "ReleaseNotes.md" }, parameters.ResultBinDir);
	});



Task("Create-NuGet-Packages")
    .IsDependentOn("Copy-Files")
    .Does(() =>
	{
		var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

		NuGetPack("./src/Cake.SqlServer/Cake.SqlServer.nuspec", new NuGetPackSettings
		{
			Version = parameters.Version,
			ReleaseNotes = releaseNotes.Notes.ToArray(),
			BasePath = parameters.ResultBinDir,
			OutputDirectory = parameters.BuildResultDir,
			Symbols = false,
			NoPackageAnalysis = true
		});
	});



Task("Publish-Nuget")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => parameters.ShouldPublishToNugetOrg)
    .Does(() =>
	{
		// Resolve the API key.
		var apiKey = EnvironmentVariable("NUGET_API_KEY");

		if(string.IsNullOrEmpty(apiKey))
		{
			throw new InvalidOperationException("Could not resolve MyGet API key.");
		}

		// Push the package.
		NuGetPush(parameters.ResultNugetPath, new NuGetPushSettings
		{
			ApiKey = apiKey,
			Source = "https://www.nuget.org/api/v2/package"
		});
	})
	.OnError(exception =>
	{
		Information("Publish-NuGet Task failed, but continuing with next Task...");
		publishingError = true;
	});


Task("Publish-MyGet")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.ShouldPublishToMyGet)
    .Does(() =>
	{
		// Resolve the API key.
		var apiKey = EnvironmentVariable("MYGET_API_KEY");
		if(string.IsNullOrEmpty(apiKey)) {
			throw new InvalidOperationException("Could not resolve MyGet API key.");
		}

		// Resolve the API url.
		var apiUrl = EnvironmentVariable("MYGET_API_URL");
		if(string.IsNullOrEmpty(apiUrl)) {
			throw new InvalidOperationException("Could not resolve MyGet API url.");
		}

		// Push the package.
		NuGetPush(parameters.ResultNugetPath, new NuGetPushSettings {
			Source = apiUrl,
			ApiKey = apiKey
		});
	})
	.OnError(exception =>
	{
		Information("Publish-MyGet Task failed, but continuing with next Task...");
		publishingError = true;
	});


Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => parameters.IsRunningOnAppVeyor)
    .Does(() =>
	{
		AppVeyor.UploadArtifact(parameters.ResultNugetPath);
	});


Task("Package")
    .IsDependentOn("Create-NuGet-Packages");

Task("AppVeyor")
    //.IsDependentOn("Publish-Nuget")
    .IsDependentOn("Publish-MyGet")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .Finally(() =>
    {
        if(publishingError)
        {
            throw new Exception("An error occurred during the publishing of Cake.  All publishing tasks have been attempted.");
        }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);