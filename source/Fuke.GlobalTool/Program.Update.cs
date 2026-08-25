// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Fuke.Common;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Utilities;
using static Fuke.Common.Constants;
using static Fuke.Common.ToolLocalization;

namespace Fuke.GlobalTool;

partial class Program
{
    [UsedImplicitly]
    public static int Update(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        PrintInfo();
        Logging.Configure();

        Assert.NotNull(rootDirectory);

        if (buildScript != null)
        {
            ConfirmExecution(L("Update build scripts", "更新构建脚本"), () => UpdateBuildScripts(rootDirectory, buildScript));
            ConfirmExecution(L("Update build project", "更新构建项目"), () => UpdateBuildProject(buildScript));
        }

        ConfirmExecution(L("Update configuration files", "更新配置文件"), () => UpdateConfigurationFile(rootDirectory));
        ConfirmExecution(L("Update global.json", "更新 global.json"), () => UpdateGlobalJsonFile(rootDirectory));

        ShowCompletion(L("Update", "更新"));

        return 0;
    }

    private static void UpdateBuildScripts(AbsolutePath rootDirectory, AbsolutePath buildScript)
    {
        var configuration = GetConfiguration(buildScript, evaluate: true);
        var buildProjectFile = (AbsolutePath) configuration[BUILD_PROJECT_FILE];

        WriteBuildScripts(
            scriptDirectory: buildScript.Parent,
            rootDirectory,
            buildDirectory: buildProjectFile.NotNull().Parent,
            buildProjectName: Path.GetFileNameWithoutExtension(buildProjectFile));
    }

    private static void UpdateBuildProject(AbsolutePath buildScript)
    {
        var configuration = GetConfiguration(buildScript, evaluate: true);
        var projectFile = configuration[BUILD_PROJECT_FILE];
        ProjectModelTasks.Initialize();
        ProjectUpdater.Update(projectFile);
    }

    private static void UpdateConfigurationFile(AbsolutePath rootDirectory)
    {
        var configurationFile = rootDirectory / FukeDirectoryName;
        if (!configurationFile.Exists())
            return;

        var solutionFile = rootDirectory / configurationFile.ReadAllLines().FirstOrDefault(x => !x.IsNullOrEmpty());
        configurationFile.DeleteFile();

        WriteConfigurationFile(rootDirectory, solutionFile);
        Host.Warning(L("The legacy .fuke file was converted to a .fuke directory.", "原 .fuke 文件已转换为 .fuke 目录。"));
        Host.Warning(L("The contents of .tmp were moved to .fuke/temp and can now be cleaned up.", ".tmp 目录的内容已移至 .fuke/temp，现在可以清理。"));
        if (solutionFile != null)
            Host.Warning(L(
                $"Ensure that the property referencing the solution has the same name as the member marked with {nameof(SolutionAttribute)}.",
                $"请确认引用解决方案的属性与标记 {nameof(SolutionAttribute)} 的成员同名。"));
    }

    private static void UpdateGlobalJsonFile(AbsolutePath rootDirectory)
    {
        var latestInstalledSdk = DotNetTasks.DotNet("--list-sdks", logInvocation: false, logOutput: false)
            .LastOrDefault().Text?.Split(" ").First();
        if (latestInstalledSdk == null)
            return;

        var globalJsonFile = rootDirectory / "global.json";
        var jobject = globalJsonFile.Existing()?.ReadJson() ?? new JObject();
        jobject["sdk"] ??= new JObject();
        jobject["sdk"].NotNull()["version"] = latestInstalledSdk;
        globalJsonFile.WriteJson(jobject);
    }
}
