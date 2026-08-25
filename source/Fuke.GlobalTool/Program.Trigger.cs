// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Git;
using Fuke.Common.IO;
using Fuke.Common.Tools.Git;
using Fuke.Common.Utilities;
using static Fuke.Common.ToolLocalization;

namespace Fuke.GlobalTool;

partial class Program
{
    [UsedImplicitly]
    public static int Trigger(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        var repository = GitRepository.FromLocalDirectory(rootDirectory.NotNull())
            .NotNull(L("The Git repository was not found.", "未找到 Git 仓库"));
        Assert.NotNull(repository.Branch, L("The Git repository cannot be in detached HEAD state.", "Git 仓库不能处于 detached HEAD 状态"));
        Assert.NotEmpty(args);

        try
        {
            var messageBody = args.JoinSpace();
            GitTasks.Git($"commit --allow-empty -m {messageBody.DoubleQuote()}");
            GitTasks.Git($"push {repository.RemoteName} {repository.Head}:{repository.RemoteBranch}");
            return 0;
        }
        catch
        {
            return 1;
        }
    }
}
