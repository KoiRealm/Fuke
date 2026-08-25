// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;
using Fuke.Common;
using Fuke.Common.CI;
using Fuke.Common.CI.AppVeyor;
using Fuke.Common.CI.AzurePipelines;
using Fuke.Common.CI.GitHubActions;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.Execution;
using Fuke.Common.Git;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Tools.GitHub;
using Fuke.Common.Utilities;
using Fuke.Components;
using static Fuke.Common.ControlFlow;
using static Fuke.Common.Tools.DotNet.DotNetTasks;
using static Fuke.Common.Tools.ReSharper.ReSharperTasks;

[DotNetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
partial class Build
    : FukeBuild,
        IHazTwitterCredentials,
        IHazChangelog,
        IHazGitRepository,
        IHazSolution,
        IRestore,
        ICompile,
        IPack,
        ITest,
        IReportCoverage,
        IReportIssues,
        IReportDuplicates,
        IPublish,
        ICreateGitHubRelease
{
    /// FUKE supports generated integration files for ReSharper, Rider, Visual Studio, and VS Code.
    public static int Main() => Execute<Build>(x => ((IPack)x).Pack);

    [CI] readonly TeamCity TeamCity;
    [CI] readonly AzurePipelines AzurePipelines;
    [CI] readonly AppVeyor AppVeyor;
    [CI] readonly GitHubActions GitHubActions;

    GitRepository GitRepository => From<IHazGitRepository>().GitRepository;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    Fuke.Common.ProjectModel.Solution IHazSolution.Solution => Solution;

    IHazTwitterCredentials TwitterCredentials => From<IHazTwitterCredentials>();

    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath VersionFile => RootDirectory / "Version.props";

    const string RepositoryUrl = "https://github.com/KoiRealm/Fuke";
    string FukeVersion => XmlTasks.XmlPeekSingle(VersionFile, "/Project/PropertyGroup/FukeVersion");
    string IHazChangelog.NuGetReleaseNotes =>
        $"{Fuke.Common.ChangeLog.ChangelogTasks.GetNuGetReleaseNotes(((IHazChangelog)this).ChangelogFile)}" +
        $"{Environment.NewLine}{Environment.NewLine}Full changelog at {RepositoryUrl}/blob/{MainBranch}/CHANGELOG.md";

    const string MainBranch = "main";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";
    const string HotfixBranchPrefix = "hotfix";

    AbsolutePath IHazArtifacts.ArtifactsDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before<IRestore>()
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("*/bin", "*/obj").DeleteDirectories();
            OutputDirectory.CreateOrCleanDirectory();
        });

    Configure<DotNetBuildSettings> ICompile.CompileSettings => _ => _
        .When(!ScheduledTargets.Contains(((IPublish)this).Publish) && !ScheduledTargets.Contains(Install), _ => _
            .ClearProperties());

    Configure<DotNetPublishSettings> ICompile.PublishSettings => _ => _
        .When(!ScheduledTargets.Contains(((IPublish)this).Publish) && !ScheduledTargets.Contains(Install), _ => _
            .ClearProperties());

    IEnumerable<(Fuke.Common.ProjectModel.Project Project, string Framework)> ICompile.PublishConfigurations =>
        from project in new[] { Solution.Fuke_GlobalTool, Solution.Fuke_MSBuildTasks }
        from framework in project.GetTargetFrameworks()
        select (project, framework);

    IEnumerable<Fuke.Common.ProjectModel.Project> ITest.TestProjects => Partition.GetCurrent(Solution.GetAllProjects("*.Tests"));

    [Parameter]
    public int TestDegreeOfParallelism { get; } = 1;

    Target ITest.Test => _ => _
        .Inherit<ITest>()
        .OnlyWhenStatic(() => Host is not GitHubActions { Workflow: PrereleaseDeploymentWorkflow })
        .Partition(2);

    bool IReportCoverage.CreateCoverageHtmlReport => true;
    bool IReportCoverage.ReportToCodecov => false;

    IEnumerable<(string PackageId, string Version)> IReportIssues.InspectCodePlugins
        => new (string PackageId, string Version)[]
           {
               new("ReSharperPlugin.CognitiveComplexity", ReSharperPluginLatest)
           };

    bool IReportIssues.InspectCodeFailOnWarning => false;
    bool IReportIssues.InspectCodeReportWarnings => true;
    IEnumerable<string> IReportIssues.InspectCodeFailOnIssues => new string[0];
    IEnumerable<string> IReportIssues.InspectCodeFailOnCategories => new string[0];

    Configure<DotNetPackSettings> IPack.PackSettings => _ => _
        .SetRepositoryUrl(RepositoryUrl)
        .SetVersion(FukeVersion);

    string PublicNuGetSource => "https://api.nuget.org/v3/index.json";

    [Parameter("发布正式包所需的 NuGet API 密钥")] [Secret] readonly string PublicNuGetApiKey;
    [Parameter("预发行包的 NuGet 源；非正式发布时必须显式指定")] readonly string PrereleaseNuGetSource;
    [Parameter("发布预发行包所需的 NuGet API 密钥")] [Secret] readonly string PrereleaseNuGetApiKey;

    bool IsPublicRelease => GitRepository.IsOnMainBranch() || GitRepository.IsOnReleaseBranch();
    string IPublish.NuGetSource => IsPublicRelease
        ? PublicNuGetSource
        : PrereleaseNuGetSource.NotNull("未指定预发行 NuGet 源");
    string IPublish.NuGetApiKey => IsPublicRelease
        ? PublicNuGetApiKey
        : PrereleaseNuGetApiKey;

    Target IPublish.Publish => _ => _
        .Inherit<IPublish>()
        .Consumes(From<IPack>().Pack)
        .Requires(() => IsPublicRelease && Host is AppVeyor || GitRepository.IsOnDevelopBranch() && Host is GitHubActions && GitHubActions.Workflow == PrereleaseDeploymentWorkflow)
        .WhenSkipped(DependencyBehavior.Execute);

    IEnumerable<AbsolutePath> NuGetPackageFiles
        => From<IPack>().PackagesDirectory.GlobFiles("*.nupkg");

    Target DeletePackages => _ => _
        .DependentFor<IPublish>()
        .After<IPack>()
        .OnlyWhenStatic(() => Host is Terminal or GitHubActions { Workflow: PrereleaseDeploymentWorkflow })
        .Executes(() =>
        {
            if (Host is Terminal)
            {
                var packagesDirectory = NuGetPackageResolver.GetPackagesDirectory(packagesConfigFile: BuildProjectFile);
                var packageDirectories = packagesDirectory.GlobDirectories($"fuke.*/{FukeVersion}");
                packageDirectories.DeleteDirectories();
            }
            else if (Host is GitHubActions)
            {
                void DeletePackage(string id, string version)
                    => DotNet(
                        $"nuget delete {id} {version} --source {PrereleaseNuGetSource.NotNull("未指定预发行 NuGet 源")} --api-key {PrereleaseNuGetApiKey} --non-interactive",
                        logOutput: false);

                var packageIds = NuGetPackageFiles.Select(x => new PackageArchiveReader(x).NuspecReader.GetId());
                foreach (var packageId in packageIds)
                    SuppressErrors(() => DeletePackage(packageId, FukeVersion), logWarning: false);
            }
        });

    string ICreateGitHubRelease.Name => $"v{FukeVersion}";
    IEnumerable<AbsolutePath> ICreateGitHubRelease.AssetFiles => NuGetPackageFiles;

    Target ICreateGitHubRelease.CreateGitHubRelease => _ => _
        .Inherit<ICreateGitHubRelease>()
        .TriggeredBy<IPublish>()
        .ProceedAfterFailure()
        .OnlyWhenStatic(() => GitRepository.IsOnMainBranch())
        .Executes(async () =>
        {
            var issues = await GitRepository.GetGitHubMilestoneIssues(MilestoneTitle);
            foreach (var issue in issues)
                await GitHubActions.Instance.CreateComment(issue.Number, $"Released in {MilestoneTitle}! 🎉");
        });

    Target Install => _ => _
        .DependsOn<IPack>()
        .Executes(() =>
        {
            SuppressErrors(() => DotNet($"tool uninstall -g {Solution.Fuke_GlobalTool.Name}"), logWarning: false);
            DotNet($"tool install -g {Solution.Fuke_GlobalTool.Name} --add-source {OutputDirectory} --version {FukeVersion}");
        });

    T From<T>()
        where T : IFukeBuild
        => (T)(object)this;
}
