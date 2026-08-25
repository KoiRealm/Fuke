using System;
using System.Linq;
using Fuke.Common;
using Fuke.Common.CI;
using Fuke.Common.Execution;
using Fuke.Common.Git;                                                                          // GIT
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.DotNet;                                                                 // DOTNET
using Fuke.Common.Tools.GitVersion;                                                             // GITVERSION
using Fuke.Common.Tools.MSBuild;                                                                // MSBUILD
using Fuke.Common.Tools.NuGet;                                                                  // NUGET && MSBUILD
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.ChangeLog.ChangelogTasks;                                              // CHANGELOG
using static Fuke.Common.EnvironmentInfo;
using static Fuke.Common.IO.PathConstruction;
using static Fuke.Common.Tools.DotNet.DotNetTasks;                                              // DOTNET
using static Fuke.Common.Tools.MSBuild.MSBuildTasks;                                            // MSBUILD
using static Fuke.Common.Tools.NuGet.NuGetTasks;                                                // NUGET && MSBUILD

[CheckBuildProjectConfigurations]                                                               // SOLUTION_FILE
[ShutdownDotNetAfterServerBuild]                                                                // DOTNET
class Build : FukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://github.com/KoiRealm/Fuke/resharper
    ///   - JetBrains Rider            https://github.com/KoiRealm/Fuke/rider
    ///   - Microsoft VisualStudio     https://github.com/KoiRealm/Fuke/visualstudio
    ///   - Microsoft VSCode           https://github.com/KoiRealm/Fuke/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build. Defaults to 'Debug' locally and 'Release' on servers.", DescriptionChinese = "要构建的配置——本地默认为“Debug”，服务器默认为“Release”")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Source used to push NuGet packages.", DescriptionChinese = "推送 NuGet 包的源")]                                                // NUGET
    readonly string Source = "https://api.nuget.org/v3/index.json";                             // NUGET
    [Parameter("API endpoint used to push NuGet symbol packages.", DescriptionChinese = "推送 NuGet 源码包的 API 端点")]                                   // NUGET
    readonly string SymbolSource = "https://nuget.smbsrc.net/";                                 // NUGET
    [Parameter("API key used to push NuGet packages.", DescriptionChinese = "推送 NuGet 包的 API 密钥")]                                           // NUGET
    readonly string ApiKey;                                                                     // NUGET

    [Solution] readonly Solution Solution;                                                      // SOLUTION_FILE
    [GitRepository] readonly GitRepository GitRepository;                                       // GIT
    [GitVersion] readonly GitVersion GitVersion;                                                // GITVERSION

    AbsolutePath SourceDirectory => RootDirectory / "source";                                   // SOURCE_DIR
    AbsolutePath SourceDirectory => RootDirectory / "src";                                      // SRC_DIR
    AbsolutePath TestsDirectory => RootDirectory / "tests";                                     // TESTS_DIR
    AbsolutePath OutputDirectory => RootDirectory / "output";                                   // OUTPUT_DIR
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";                             // ARTIFACTS_DIR

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);       // SOURCE_DIR || SRC_DIR
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);        // TESTS_DIR
            EnsureCleanDirectory(OutputDirectory);                                              // OUTPUT_DIR
            EnsureCleanDirectory(ArtifactsDirectory);                                           // ARTIFACTS_DIR
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            MSBuild(s => s                                                                      // MSBUILD
                .SetTargetPath(Solution)                                                        // MSBUILD
                .SetTargets("Restore"));                                                        // MSBUILD
            DotNetRestore(s => s                                                                // DOTNET
                .SetProjectFile(Solution));                                                     // DOTNET
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuild(s => s                                                                      // MSBUILD
                .SetTargetPath(Solution)                                                        // MSBUILD
                .SetTargets("Rebuild")                                                          // MSBUILD
                .SetConfiguration(Configuration)                                                // MSBUILD
                .SetAssemblyVersion(GitVersion.AssemblySemVer)                                  // MSBUILD && GITVERSION
                .SetFileVersion(GitVersion.AssemblySemFileVer)                                  // MSBUILD && GITVERSION
                .SetInformationalVersion(GitVersion.InformationalVersion)                       // MSBUILD && GITVERSION
                .SetMaxCpuCount(Environment.ProcessorCount)                                     // MSBUILD
                .SetNodeReuse(IsLocalBuild));                                                   // MSBUILD
            DotNetBuild(s => s                                                                  // DOTNET
                .SetProjectFile(Solution)                                                       // DOTNET
                .SetConfiguration(Configuration)                                                // DOTNET
                .SetAssemblyVersion(GitVersion.AssemblySemVer)                                  // DOTNET && GITVERSION
                .SetFileVersion(GitVersion.AssemblySemFileVer)                                  // DOTNET && GITVERSION
                .SetInformationalVersion(GitVersion.InformationalVersion)                       // DOTNET && GITVERSION
                .EnableNoRestore());                                                            // DOTNET
        });

    string ChangelogFile => RootDirectory / "CHANGELOG.md";                                     // CHANGELOG

    Target Pack => _ => _                                                                       // NUGET
        .DependsOn(Compile)                                                                     // NUGET
        .Executes(() =>                                                                         // NUGET
        {                                                                                       // NUGET
            MSBuild(s => s                                                                      // NUGET && MSBUILD
                .SetTargetPath(Solution)                                                        // NUGET && MSBUILD
                .SetTargets("Restore", "Pack")                                                  // NUGET && MSBUILD
                .SetPackageVersion(GitVersion.NuGetVersionV2)                                   // NUGET && MSBUILD && GITVERSION
                .SetPackageReleaseNotes(GetNuGetReleaseNotes(ChangelogFile, GitRepository))     // NUGET && MSBUILD && CHANGELOG && GIT
                .SetPackageOutputPath(ArtifactsDirectory)                                       // NUGET && MSBUILD && ARTIFACTS_DIR
                .SetPackageOutputPath(OutputDirectory)                                          // NUGET && MSBUILD && OUTPUT_DIR
                .SetConfiguration(Configuration)                                                // NUGET && MSBUILD
                .EnableIncludeSymbols()                                                         // NUGET && MSBUILD
                .SetSymbolPackageFormat(NuGetSymbolPackageFormat.snupkg));                      // NUGET && MSBUILD
            DotNetPack(s => s                                                                   // NUGET && DOTNET
                .SetProject(Solution)                                                           // NUGET && DOTNET
                .SetVersion(GitVersion.NuGetVersionV2)                                          // NUGET && DOTNET && GITVERSION
                .SetPackageReleaseNotes(GetNuGetReleaseNotes(ChangelogFile, GitRepository))     // NUGET && DOTNET && CHANGELOG && GIT
                .SetOutputDirectory(ArtifactsDirectory)                                         // NUGET && DOTNET && ARTIFACTS_DIR
                .SetOutputDirectory(OutputDirectory)                                            // NUGET && DOTNET && OUTPUT_DIR
                .SetConfiguration(Configuration)                                                // NUGET && DOTNET
                .EnableNoBuild()                                                                // NUGET && DOTNET
                .EnableIncludeSymbols()                                                         // NUGET && DOTNET
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg));                     // NUGET && DOTNET
        });                                                                                     // NUGET
                                                                                                // NUGET
    Target Push => _ => _                                                                       // NUGET
        .DependsOn(Pack)                                                                        // NUGET
        .Requires(() => ApiKey)                                                                 // NUGET
        .Requires(() => Configuration.Equals(Configuration.Release))                            // NUGET
        .Executes(() =>                                                                         // NUGET
        {                                                                                       // NUGET
            DotNetNuGetPush(s => s                                                              // NUGET && DOTNET
            NuGetPush(s => s                                                                    // NUGET && MSBUILD
                    .SetSource(Source)                                                          // NUGET
                    .SetSymbolSource(SymbolSource)                                              // NUGET
                    .SetApiKey(ApiKey)                                                          // NUGET
                    .CombineWith(                                                               // NUGET
                        OutputDirectory.GlobFiles("*.nupkg"), (cs, v) => cs                     // NUGET && OUTPUT_DIR
                        ArtifactsDirectory.GlobFiles("*.nupkg"), (cs, v) => cs                  // NUGET && ARTIFACTS_DIR
                            .SetTargetPath(v)),                                                 // NUGET
                degreeOfParallelism: 5,                                                         // NUGET
                completeOnFailure: true);                                                       // NUGET
        });                                                                                     // NUGET
}
