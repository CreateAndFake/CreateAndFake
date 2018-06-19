using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.OpenCover;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.IO.PathConstruction;

/// <summary>Manages build behavior for the solution.</summary>
internal class Build : NukeBuild
{
    /// <summary>Output folder for the solution.</summary>
    private AbsolutePath ArtifactDir => SolutionDirectory / "artifacts";

    /// <summary>Artifact output folder for tests.</summary>
    private AbsolutePath TestingDir => ArtifactDir / "testing";

    /// <summary>Artifact output folder for test coverage.</summary>
    private AbsolutePath CoverageDir => ArtifactDir / "coverage";

    /// <summary>Target file for raw test coverage data.</summary>
    private AbsolutePath RawCoverageFile => CoverageDir / "CoverageRaw.xml";

    /// <summary>Settings file used for testing.</summary>
    private AbsolutePath TestSettingsFile => SolutionDirectory / "tests" / "TestSettings.runsettings";

    /// <summary>Provides access to the structure of the solution.</summary>
    [Solution]
    private readonly Solution s_Solution;

    // Console application entry point. Also defines the default target.
    public static int Main() => Execute<Build>(x => x.Compile);

    /// <summary>Deletes output folders.</summary>
    internal Target Clean => _ => _
        .Executes(() =>
        {
            FileSystemTasks.DeleteDirectory(ArtifactDir / "obj");
            FileSystemTasks.DeleteDirectory(ArtifactDir / "bin");
            FileSystemTasks.DeleteDirectory(TestingDir);
            FileSystemTasks.DeleteDirectory(CoverageDir);
        });

    /// <summary>Builds the solution.</summary>
    internal Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetConfiguration("Debug")
                .SetProjectFile(SolutionFile));

            DotNetTasks.DotNetBuild(s => s
                .SetConfiguration("Release")
                .SetProjectFile(SolutionFile));
        });

    /// <summary>Builds and tests the solution.</summary>
    internal Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            foreach (Project proj in s_Solution.GetProjects("*Tests"))
            {
                DotNetTasks.DotNetTest(s => s
                    .SetConfiguration("Debug")
                    .SetProjectFile(proj.Path)
                    .SetSettingsFile(TestSettingsFile)
                    .SetNoBuild(true)
                    .SetNoRestore(true));

                DotNetTasks.DotNetTest(s => s
                    .SetConfiguration("Release")
                    .SetProjectFile(proj.Path)
                    .SetSettingsFile(TestSettingsFile)
                    .SetNoBuild(true)
                    .SetNoRestore(true));
            }
        });

    /// <summary>Builds and analyzes test code coverage.</summary>
    internal Target Coverage => _ => _
        .OnlyWhen(() => EnvironmentInfo.IsWin)
        .Executes(() =>
        {
            FileSystemTasks.EnsureCleanDirectory(CoverageDir);

            DotNetTasks.DotNetBuild(s => s
                .SetConfiguration("Full")
                .SetProjectFile(SolutionFile));

            if (IsServerBuild)
            {
                NuGetTasks.NuGetRestore(s => s
                    .SetSource("OpenCover", "ReportGenerator"));
            }

            foreach (Project proj in s_Solution.GetProjects("*Tests"))
            {
                OpenCoverTasks.OpenCover(s => s
                    .SetTargetPath(DotNetTasks.DotNetPath)
                    .SetTargetArguments($"test {proj.Path} -c Full -s {TestSettingsFile} --no-build --no-restore")
                    .SetSearchDirectories($"{TestingDir / "Full"}")
                    .SetOutput(RawCoverageFile)
                    .SetFilters("+[*]* -[*Tests]*")
                    .SetRegistration(RegistrationType.User)
                    .SetHideSkippedKinds(OpenCoverSkipping.Filter)
                    .SetMergeOutput(true)
                    .SetOldStyle(true));
            }

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetReports(RawCoverageFile)
                .SetTargetDirectory(CoverageDir));
        });

    /// <summary>Build process for Travis.</summary>
    internal Target OnTravis => _ => _
        .OnlyWhen(() => IsServerBuild)
        .Executes(() =>
        {
            foreach (Project proj in s_Solution.GetProjects("*Tests"))
            {
                DotNetTasks.DotNetTest(s => s
                    .SetConfiguration("Travis")
                    .SetFramework("netcoreapp2.0")
                    .SetProjectFile(proj.Path)
                    .SetSettingsFile(TestSettingsFile));
            }
        });
}
