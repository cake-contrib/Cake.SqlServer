public class BuildParameters
{
    public String ProjectDir => "./src/Cake.SqlServer/";
    public String ProjectDacDir => "./src/Cake.SqlServer.DacFx/";
    public String Solution => "./src/Cake.SqlServer.sln";

    public DirectoryPath BuildDir => ProjectDir + "bin/" + Configuration;
    public DirectoryPath BuildDacDir => ProjectDacDir + "bin/" + Configuration;
    public string BuildResultDir => "./build-results/v" + SemVersion + "/";

    public string ResultBinDir => BuildResultDir + "bin";

    public string ResultNugetPath => BuildResultDir + "Cake.SqlServer." + Version + ".nupkg";

    public string TestResultsFile  => BuildResultDir + "/TestsResults.xml";

    public bool ShouldPublishToNugetOrg => false;
    public bool ShouldPublishToMyGet => true;
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPullRequest  { get; private set; }
    public ReleaseNotes ReleaseNotes { get; private set; }
    public bool IsMasterBranch { get; private set; }

    public string Version { get; private set; }
    public string SemVersion { get; private set; }
    public bool SkipTests { get; private set; }


    public void Initialize(ICakeContext context)
    {
        context.Information("Executing GitVersion");
        var result = context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfoFilePath = ProjectDir + "properties/AssemblyInfo.cs",
            UpdateAssemblyInfo = true,
        });
        context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfoFilePath = ProjectDacDir + "properties/AssemblyInfo.cs",
            UpdateAssemblyInfo = true,
        });        
        Version = result.MajorMinorPatch ?? "0.0.1";
        SemVersion = result.LegacySemVerPadded ?? "0.0.1";

		// print gitversion
        context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
    }

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var configuration = context.Argument("configuration", "Release");
        var buildSystem = context.BuildSystem();
		var isMaster = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch);

		context.Information("IsTagged: {0}", IsBuildTagged(buildSystem));
		context.Information("IsMasterBranch: {0}", isMaster);

        return new BuildParameters {
            Target = target,
            Configuration = configuration,
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
            IsTagged = IsBuildTagged(buildSystem),

            IsMasterBranch = isMaster,
            ReleaseNotes = context.ParseReleaseNotes("./ReleaseNotes.md"),
            SkipTests = StringComparer.OrdinalIgnoreCase.Equals("True", context.Argument("skiptests", "false")),
        };
    }

    private static bool IsBuildTagged(BuildSystem buildSystem)
    {
        return buildSystem.AppVeyor.Environment.Repository.Tag.IsTag
            && !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name);
    }
}

