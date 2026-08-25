// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.CI;
using Fuke.Common.Git;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;

namespace Fuke.Common.Execution;

[PublicAPI]
public class ArgumentsFromGitCommitMessageAttribute : BuildExtensionAttributeBase, IOnBuildCreated
{
    public string Prefix { get; set; } = "[fuke++]";

    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        if (BuildServerConfigurationGeneration.IsActive)
            return;

        var commit = GitRepository.GetCommitFromCI();
        if (commit == null)
            return;

        var git = ToolResolver.GetEnvironmentOrPathTool("git");
        var lastLine = git.Invoke($"show -s --format=%B {commit}", logInvocation: false, logOutput: false)
            .Select(x => x.Text)
            .LastOrDefault(x => !x.IsNullOrEmpty());
        if (!lastLine?.StartsWithOrdinalIgnoreCase(Prefix) ?? true)
            return;

        var arguments = lastLine[Prefix.Length..].TrimStart();
        ParameterService.Instance.ArgumentsFromCommitMessageService = new ArgumentParser(arguments);
    }
}
