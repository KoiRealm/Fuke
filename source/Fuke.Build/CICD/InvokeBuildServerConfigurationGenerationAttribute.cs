// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Serilog;
using static Fuke.Common.CI.BuildServerConfigurationGeneration;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.CI;

public class InvokeBuildServerConfigurationGenerationAttribute
    : BuildServerConfigurationGenerationAttributeBase, IOnBuildCreated
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        if (Build.IsServerBuild || Build.IsInterceptorExecution)
            return;

        var hasConfigurationChanged = GetGenerators(Build)
            .Where(x => x.AutoGenerate)
            .AsParallel()
            .Select(HasConfigurationChanged).ToList();
        if (hasConfigurationChanged.All(x => !x))
            return;

        if (Build.Help)
            return;

        if (Console.IsInputRedirected)
            return;

        Host.Information(L("Press any key to continue ...", "按任意键继续……"));
        Console.ReadKey();
    }

    private bool HasConfigurationChanged(IConfigurationGenerator generator)
    {
        var generatedFiles = generator.GeneratedFiles.ToList();
        generatedFiles.ForEach(x => x.Parent.CreateDirectory());

        var previousHashes = generatedFiles
            .WhereFileExists()
            .ToDictionary(x => x, x => x.GetFileHash());

        ProcessTasks.StartProcess(
                Build.BuildAssemblyFile,
                $"--{ConfigurationParameterName} {generator.Id} --host {generator.HostName}",
                logInvocation: false,
                logOutput: false)
            .AssertZeroExitCode();

        var changedFiles = generatedFiles
            .Where(x => x.GetFileHash() != previousHashes.GetValueOrDefault(x))
            .Select(x => Build.RootDirectory.GetRelativePathTo(x)).ToList();

        if (changedFiles.Count == 0)
            return false;

        // TODO: multi-line logging
        Log.Warning(L("Configuration files for {Configuration} have changed.", "{Configuration} 的配置文件已更改。"), generator.DisplayName);
        changedFiles.ForEach(x => Log.Verbose(L("Updated {File}", "已更新 {File}"), x));
        return true;
    }
}
