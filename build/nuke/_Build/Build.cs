using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
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
    [GitRepository] private readonly GitRepository _gitRepository;
    [GitVersion] private readonly GitVersion _gitVersion;

    /// <summary>Output folder for the solution.</summary>
    private AbsolutePath ArtifactDir => _solution.Directory / "artifacts";

    /// <summary>Artifact output folder for tests.</summary>
    private AbsolutePath TestingDir => ArtifactDir / "testing";

    /// <summary>Artifact output folder for test coverage.</summary>
    private AbsolutePath CoverageDir => ArtifactDir / "coverage";

    /// <summary>Target file for raw test coverage data.</summary>
    private AbsolutePath RawCoverageFile => CoverageDir / "CoverageRaw.xml";

    /// <summary>Settings file used for testing.</summary>
    private AbsolutePath TestSettingsFile => _solution.Directory / "tests" / "TestSettings.runsettings";

    /// <summary>Provides access to the structure of the solution.</summary>
    [Solution]
    private readonly Solution _solution;

    // Console application entry point. Also defines the default target.
    public static int Main()
    {
        return Execute<Build>(x => x.Compile);
    }

    /// <summary>Deletes output folders.</summary>
    internal Target Clean => _ => _
        .Executes(() =>
        {
            FileSystemTasks.EnsureCleanDirectory(ArtifactDir / "obj");
            FileSystemTasks.EnsureCleanDirectory(ArtifactDir / "bin");
            FileSystemTasks.EnsureCleanDirectory(TestingDir);
            FileSystemTasks.EnsureCleanDirectory(CoverageDir);
        });

    /// <summary>Builds the solution.</summary>
    internal Target Compile => _ => _
        .Executes(() =>
        {
            DotNetBuildSettings Set(DotNetBuildSettings s)
            {
                return s.SetProjectFile(_solution)
                    .SetAssemblyVersion(_gitVersion.GetNormalizedAssemblyVersion())
                    .SetFileVersion(_gitVersion.GetNormalizedFileVersion())
                    .SetInformationalVersion(_gitVersion.InformationalVersion);
            }

            Logger.Info("Version - " + _gitVersion.GetNormalizedAssemblyVersion());
            Logger.Info("Version - " + _gitVersion.GetNormalizedFileVersion());
            Logger.Info("Version - " + _gitVersion.InformationalVersion);
            Logger.Info("Version - " + _gitVersion.NuGetVersionV2);
            Logger.Info("Version - " + _gitVersion.NuGetVersion);
            Logger.Info("Version - " + _gitVersion.AssemblySemVer);
            Logger.Info("Version - " + _gitVersion.FullSemVer);

            DotNetTasks.DotNetBuild(s => Set(s).SetConfiguration("Debug"));
            DotNetTasks.DotNetBuild(s => Set(s).SetConfiguration("Release"));
        });

    /// <summary>Builds and packs the solution.</summary>
    internal Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPackSettings Set(DotNetPackSettings s)
            {
                return s.SetProject(_solution)
                    .SetVersion(_gitVersion.NuGetVersionV2)
                    .EnableNoRestore()
                    .EnableNoBuild();
            }

            DotNetTasks.DotNetPack(s => Set(s).SetConfiguration("Debug"));
            DotNetTasks.DotNetPack(s => Set(s).SetConfiguration("Release"));
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
        .OnlyWhenStatic(() => EnvironmentInfo.IsWin)
        .Executes(() =>
        {
            FileSystemTasks.EnsureCleanDirectory(CoverageDir);

            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(_solution)
                .SetConfiguration("Full"));

            OpenCoverSettings Set(OpenCoverSettings s, string f)
            {
                return s.SetTargetPath(DotNetTasks.DotNetPath)
                    .SetTargetArguments($"test {_solution.Path} -c Full -s {TestSettingsFile} -f {f} --no-build --no-restore")
                    .SetSearchDirectories($"{TestingDir / "Full" / f}")
                    .SetOutput(RawCoverageFile)
                    .SetFilters("+[*]* -[*Tests]*")
                    .SetRegistration(RegistrationType.User)
                    .SetHideSkippedKinds(OpenCoverSkipping.Filter)
                    .SetMergeOutput(true)
                    .SetOldStyle(true);
            }

            OpenCoverTasks.OpenCover(s => Set(s, "netcoreapp2.0"));
            OpenCoverTasks.OpenCover(s => Set(s, "net461"));

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetReports(RawCoverageFile)
                .SetTargetDirectory(CoverageDir));
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
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(s => s
                .SetProjectFile(_solution)
                .SetConfiguration("Travis")
                .SetFramework("netcoreapp2.0")
                .SetSettingsFile(TestSettingsFile));
        });
}
