// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using Spectre.Console;
using static Fuke.Common.Constants;
using static Fuke.Common.EnvironmentInfo;
using static Fuke.Common.Tooling.ProcessTasks;
using static Fuke.Common.Utilities.TemplateUtility;

namespace Fuke.GlobalTool;

partial class Program
{
    // ReSharper disable InconsistentNaming

    private const string TARGET_FRAMEWORK = "net8.0";
    private const string PROJECT_KIND = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";

    // ReSharper disable once CognitiveComplexity
    [UsedImplicitly]
    public static int Setup(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        PrintInfo();
        Logging.Configure();

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]开始初始化新的构建项目！[/]");
        AnsiConsole.WriteLine();

        #region Basic

        var fukeLatestReleaseVersion = NuGetVersionResolver.GetLatestVersion(FukeCommonPackageId, includePrereleases: false);
        var fukeLatestPrereleaseVersion = NuGetVersionResolver.GetLatestVersion(FukeCommonPackageId, includePrereleases: true);
        var fukeLatestLocalVersion = NuGetPackageResolver.GetGlobalInstalledPackage(FukeCommonPackageId, version: null, packagesConfigFile: null)
            ?.Version.ToString();

        if (rootDirectory == null)
            rootDirectory = WorkingDirectory.FindParentOrSelf(x => x.ContainsDirectory(".git") || x.ContainsDirectory(".svn"));

        if (rootDirectory == null)
        {
            Host.Warning("未找到根目录，将使用当前工作目录……");
            rootDirectory = WorkingDirectory;
        }
        ShowInput("deciduous_tree", "根目录", rootDirectory);

        var buildProjectName = PromptForInput("构建项目要使用什么名称？", "_build");
        ClearPreviousLine();
        ShowInput("bookmark", "构建项目名称", buildProjectName);

        var buildProjectRelativeDirectory = PromptForInput("构建项目要放在哪里？", "./build");
        ClearPreviousLine();
        ShowInput("round_pushpin", "构建项目位置", buildProjectRelativeDirectory);

        var fukeVersion = PromptForChoice("要使用哪个 Fuke.Common 版本？",
            new[]
                {
                    ("最新正式版", fukeLatestReleaseVersion.GetAwaiter().GetResult()),
                    ("最新预发行版", fukeLatestPrereleaseVersion.GetAwaiter().GetResult()),
                    ("最新本地版", fukeLatestLocalVersion),
                    ("与全局工具相同", typeof(Program).GetTypeInfo().Assembly.GetVersionText())
                }
                .Where(x => x.Item2 != null)
                .Distinct(x => x.Item2)
                .Select(x => (x.Item2, $"{x.Item2} ({x.Item1})")).ToArray());
        ShowInput("gem_stone", "Fuke.Common 版本", fukeVersion);

        var solutionFile = (AbsolutePath) PromptForChoice(
            "哪个解决方案作为默认项？",
            choices: new DirectoryInfo(rootDirectory)
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(x => x.FullName.EndsWithOrdinalIgnoreCase(".sln"))
                .OrderByDescending(x => x.FullName)
                .Select(x => (x, rootDirectory.GetRelativePathTo(x.FullName).ToString()))
                .Concat((null, "无")).ToArray())?.FullName;
        ShowInput("toolbox", "默认解决方案", solutionFile != null ? rootDirectory.GetRelativePathTo(solutionFile) : "<无>");

        #endregion

        #region Generation

        var buildDirectory = rootDirectory / buildProjectRelativeDirectory;
        var buildProjectFile = rootDirectory / buildProjectRelativeDirectory / buildProjectName + ".csproj";
        var buildProjectGuid = Guid.NewGuid().ToString().ToUpper();

        (rootDirectory / FukeDirectoryName).CreateDirectory();

        WriteBuildScripts(
            scriptDirectory: WorkingDirectory,
            rootDirectory,
            buildDirectory,
            buildProjectName);

        WriteConfigurationFile(rootDirectory, solutionFile);

        if (solutionFile != null)
        {
            var solutionFileContent = solutionFile.ReadAllLines().ToList();
            var buildProjectFileRelative = solutionFile.Parent.GetWinRelativePathTo(buildProjectFile);
            UpdateSolutionFileContent(solutionFileContent, buildProjectFileRelative, buildProjectGuid, buildProjectName);
            solutionFile.WriteAllLines(solutionFileContent, Encoding.UTF8);
        }

        buildProjectFile.WriteAllLines(
            FillTemplate(
                GetTemplate("_build.csproj"),
                GetDictionary(
                    new
                    {
                        RootDirectory = buildDirectory.GetWinRelativePathTo(rootDirectory),
                        ScriptDirectory = buildDirectory.GetWinRelativePathTo(WorkingDirectory),
                        TargetFramework = TARGET_FRAMEWORK,
                        FukeVersion = fukeVersion,
                    })));

        (buildDirectory / "Directory.Build.props").WriteAllLines(GetTemplate("Directory.Build.props"));
        (buildDirectory / "Directory.Build.targets").WriteAllLines(GetTemplate("Directory.Build.targets"));
        (buildProjectFile + ".DotSettings").WriteAllLines(GetTemplate("_build.csproj.DotSettings"));
        (buildDirectory / ".editorconfig").WriteAllLines(GetTemplate(".editorconfig"));
        (buildDirectory / "Build.cs").WriteAllLines(FillTemplate(GetTemplate("Build.cs")));
        (buildDirectory / "Configuration.cs").WriteAllLines(GetTemplate("Configuration.cs"));

        #endregion

        ShowCompletion("初始化");

        return 0;
    }

    internal static void UpdateSolutionFileContent(
        List<string> content,
        string buildProjectFileRelative,
        string buildProjectGuid,
        string buildProjectName)
    {
        if (content.Any(x => x.Contains(buildProjectFileRelative)))
            return;

        var globalIndex = content.IndexOf("Global");
        Assert.True(globalIndex != -1, "解决方案文件中未找到“Global”节");

        var projectConfigurationIndex = content.FindIndex(x => x.Contains("GlobalSection(ProjectConfigurationPlatforms)"));
        if (projectConfigurationIndex == -1)
        {
            var solutionConfigurationIndex = content.FindIndex(x => x.Contains("GlobalSection(SolutionConfigurationPlatforms)"));
            if (solutionConfigurationIndex == -1)
            {
                content.Insert(globalIndex + 1, "\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
                content.Insert(globalIndex + 2, "\t\tDebug|Any CPU = Debug|Any CPU");
                content.Insert(globalIndex + 3, "\t\tRelease|Any CPU = Release|Any CPU");
                content.Insert(globalIndex + 4, "\tEndGlobalSection");

                solutionConfigurationIndex = globalIndex + 1;
            }

            var endGlobalSectionIndex = content.FindIndex(solutionConfigurationIndex, x => x.Contains("EndGlobalSection"));

            content.Insert(endGlobalSectionIndex + 1, "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            content.Insert(endGlobalSectionIndex + 2, "\tEndGlobalSection");

            projectConfigurationIndex = endGlobalSectionIndex + 1;
        }

        content.Insert(projectConfigurationIndex + 1, $"\t\t{{{buildProjectGuid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        content.Insert(projectConfigurationIndex + 2, $"\t\t{{{buildProjectGuid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");

        content.Insert(globalIndex,
            $"Project(\"{{{PROJECT_KIND}}}\") = \"{buildProjectName}\", \"{buildProjectFileRelative}\", \"{{{buildProjectGuid}}}\"");
        content.Insert(globalIndex + 1,
            "EndProject");
    }

    private static string[] GetTemplate(string templateName)
    {
        return ResourceUtility.GetResourceAllLines<Program>($"templates.{templateName}");
    }


    private static void WriteBuildScripts(
        AbsolutePath scriptDirectory,
        AbsolutePath rootDirectory,
        AbsolutePath buildDirectory,
        string buildProjectName)
    {
        (scriptDirectory / "build.cmd").WriteAllLines(
            FillTemplate(GetTemplate("build.cmd")),
            platformFamily: PlatformFamily.Linux);

        (scriptDirectory / "build.sh").WriteAllLines(
            FillTemplate(
                GetTemplate("build.sh"),
                tokens: GetDictionary(
                    new
                    {
                        RootDirectory = scriptDirectory.GetUnixRelativePathTo(rootDirectory),
                        BuildDirectory = scriptDirectory.GetUnixRelativePathTo(buildDirectory),
                        BuildProjectName = buildProjectName,
                    })),
            platformFamily: PlatformFamily.Linux);

        (scriptDirectory / "build.ps1").WriteAllLines(
            FillTemplate(
                GetTemplate("build.ps1"),
                tokens: GetDictionary(
                    new
                    {
                        RootDirectory = scriptDirectory.GetWinRelativePathTo(rootDirectory),
                        BuildDirectory = scriptDirectory.GetWinRelativePathTo(buildDirectory),
                        BuildProjectName = buildProjectName,
                    })),
            platformFamily: PlatformFamily.Windows);

        MakeExecutable(scriptDirectory / "build.cmd");
        MakeExecutable(scriptDirectory / "build.sh");

        void MakeExecutable(AbsolutePath scriptFile)
        {
            if (rootDirectory.ContainsDirectory(".git"))
                StartProcess("git", $"update-index --add --chmod=+x {scriptFile}", logInvocation: false, logOutput: false);

            if (rootDirectory.ContainsDirectory(".svn"))
                StartProcess("svn", $"propset svn:executable on {scriptFile}", logInvocation: false, logOutput: false);

            if (IsUnix)
                StartProcess("chmod", $"+x {scriptFile}", logInvocation: false, logOutput: false);
        }
    }

    private static void WriteConfigurationFile(AbsolutePath rootDirectory, [CanBeNull] AbsolutePath solutionFile)
    {
        var parametersFile = GetDefaultParametersFile(rootDirectory);
        var dictionary = new Dictionary<string, string> { ["$schema"] = BuildSchemaFileName };
        if (solutionFile != null)
            dictionary["Solution"] = rootDirectory.GetUnixRelativePathTo(solutionFile).ToString();
        parametersFile.WriteJson(dictionary);
    }
}
