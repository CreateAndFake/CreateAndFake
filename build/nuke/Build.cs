using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.OpenCover;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.IO.PathConstruction;

/// <summary>Manages build behavior for the solution.</summary>
[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
internal class Build : NukeBuild
{
    /// <summary>Output folder for the solution.</summary>
    private AbsolutePath ArtifactDir => _solution.Directory / "artifacts";

    /// <summary>Artifact output folder for tests.</summary>
    private AbsolutePath TestingDir => ArtifactDir / "testing";

    /// <summary>Artifact output folder for packages.</summary>
    private AbsolutePath PackageDir => ArtifactDir / "releases";

    /// <summary>Artifact output folder for test coverage.</summary>
    private AbsolutePath CoverageDir => ArtifactDir / "coverage";

    /// <summary>Target file for raw test coverage data.</summary>
    private AbsolutePath RawCoverageFile => CoverageDir / "CoverageRaw.xml";

    /// <summary>Settings file used for testing.</summary>
    private AbsolutePath TestSettingsFile => _solution.Directory / "tests" / "TestSettings.runsettings";

    /// <summary>Provides access to the structure of the solution.</summary>
    [Solution]
    private readonly Solution _solution;

    /// <summary>Controls pack versioning.</summary>
    [GitVersion]
    private readonly GitVersion _gitVersion;

    // Console application entry point. Also defines the default target.
    public static int Main()
    {
        return Execute<Build>(x => x.Compile);
    }

    /// <summary>Deletes output folders.</summary>
    internal Target Clean => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            FileSystemTasks.EnsureCleanDirectory(ArtifactDir / "obj");
            FileSystemTasks.EnsureCleanDirectory(ArtifactDir / "bin");
            FileSystemTasks.EnsureCleanDirectory(TestingDir);
            FileSystemTasks.EnsureCleanDirectory(PackageDir);
            FileSystemTasks.EnsureCleanDirectory(CoverageDir);
        });

    /// <summary>Builds the solution.</summary>
    internal Target Compile => _ => _
        .Executes(() =>
        {
            DotNetBuildSettings Set(DotNetBuildSettings s)
            {
                return s.SetProjectFile(_solution)
                    .SetFileVersion(_gitVersion.GetNormalizedFileVersion())
                    .SetInformationalVersion(_gitVersion.InformationalVersion)
                    .SetAssemblyVersion(_gitVersion.GetNormalizedAssemblyVersion());
            }

            DotNetTasks.DotNetBuild(s => Set(s).SetConfiguration("Debug"));
            DotNetTasks.DotNetBuild(s => Set(s).SetConfiguration("Release"));
        });

    /// <summary>Builds and packs the solution.</summary>
    internal Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(s => s
                .SetVersion(_gitVersion.NuGetVersionV2)
                .SetOutputDirectory(PackageDir)
                .SetConfiguration("Release")
                .SetProject(_solution)
                .EnableNoRestore()
                .EnableNoBuild());
        });

    /// <summary>Builds and tests the solution.</summary>
    internal Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTestSettings Set(DotNetTestSettings s)
            {
                return s.SetProjectFile(_solution)
                    .SetSettingsFile(TestSettingsFile)
                    .SetNoRestore(true)
                    .SetNoBuild(true);
            }

            DotNetTasks.DotNetTest(s => Set(s).SetConfiguration("Debug"));
            DotNetTasks.DotNetTest(s => Set(s).SetConfiguration("Release"));
        });

    /// <summary>Builds and analyzes test code coverage.</summary>
    internal Target Coverage => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Executes(() =>
        {
            FileSystemTasks.EnsureCleanDirectory(CoverageDir);

            CoverletSettings Set(CoverletSettings s, string testAssembly)
            {
                return s.SetAssembly(testAssembly)
                    .SetExclude("[xunit*]*")
                    .SetOutput(RawCoverageFile)
                    .SetMergeWith(RawCoverageFile)
                    .SetTargetSettings(new DotNetTestSettings()
                        .SetProjectFile(_solution)
                        .SetSettingsFile(TestSettingsFile)
                        .SetNoBuild(true));
            }

            AbsolutePath[] testAssemblies = TestingDir.GlobFiles("Debug/**/*Tests.dll").ToArray();
            for (int i = testAssemblies.Length - 1; i > 0; i--)
            {
                CoverletTasks.Coverlet(s => Set(s, testAssemblies[i]));
            }
            CoverletTasks.Coverlet(s => Set(s, testAssemblies[0])
                .SetFormat(CoverletOutputFormat.opencover));

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetTargetDirectory(CoverageDir / "report")
                .SetReports(RawCoverageFile));
        });

    /// <summary>Build process for AppVeyor.</summary>
    internal Target OnAppVeyor => _ => _
        .Requires(() => IsServerBuild)
        .DependsOn(Test)
        .DependsOn(Pack)
        .DependsOn(Coverage);

    /// <summary>Build process for Travis.</summary>
    internal Target OnTravis => _ => _
        .Requires(() => IsServerBuild)
        .DependsOn(Test);
}
