// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.IO;
using static Fuke.Common.Constants;

namespace Fuke.GlobalTool;

partial class Program
{
    // function fuke- { fuke :PopDirectory; cd $(fuke :GetNextDirectory) }
    // function fuke/ { fuke :PushWithChosenRootDirectory; cd $(fuke :GetNextDirectory) }
    // function fuke. { fuke :PushWithCurrentRootDirectory; cd $(fuke :GetNextDirectory) }
    // function fuke.. { fuke :PushWithParentRootDirectory; cd $(fuke :GetNextDirectory) }

    private static string SessionId
        => EnvironmentInfo.Platform switch
        {
            PlatformFamily.OSX => EnvironmentInfo.GetVariable("TERM_SESSION_ID").NotNull()[7..],
            PlatformFamily.Windows => EnvironmentInfo.GetVariable("WT_SESSION").NotNull(),
            _ => throw new NotSupportedException($"平台 {EnvironmentInfo.Platform} 没有会话 ID 选择器。")
        };

    private static AbsolutePath SessionFile => GlobalTemporaryDirectory / $"fuke-{SessionId}.dat";

    [UsedImplicitly]
    private static int GetNextDirectory(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        var content = SessionFile.Existing()?.ReadAllLines();
        if (content == null || string.IsNullOrWhiteSpace(content[0]))
        {
            Console.WriteLine(EnvironmentInfo.WorkingDirectory);
            return 1;
        }

        var nextDirectory = content[0];
        content[0] = string.Empty;
        SessionFile.WriteAllLines(content);
        Console.WriteLine(nextDirectory);
        return 0;
    }

    [UsedImplicitly]
    private static int PopDirectory(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        var content = SessionFile.Existing()?.ReadAllLines().ToList();
        if (content == null || content.Count <= 1)
        {
            Console.Error.WriteLine("没有上一个目录");
            return 1;
        }

        content[0] = content[1];
        content.RemoveAt(1);
        SessionFile.WriteAllLines(content);
        return 0;
    }

    [UsedImplicitly]
    private static int PushWithCurrentRootDirectory(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        return PushAndSetNext(() => rootDirectory.NotNull("未找到根目录"));
    }

    [UsedImplicitly]
    private static int PushWithParentRootDirectory(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        return PushAndSetNext(() => TryGetRootDirectoryFrom(Path.GetDirectoryName(rootDirectory.NotNull("未找到根目录")))
            .NotNull("未找到上级根目录"));
    }

    [UsedImplicitly]
    private static int PushWithChosenRootDirectory(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        return PushAndSetNext(() =>
        {
            var directories = EnvironmentInfo.WorkingDirectory.GlobDirectories($"**/{FukeDirectoryName}")
                .Concat(EnvironmentInfo.WorkingDirectory.GlobFiles($"**/{FukeDirectoryName}"))
                .Where(x => !x.Equals(EnvironmentInfo.WorkingDirectory))
                .Select(x => x.Parent)
                .Select(x => (x, EnvironmentInfo.WorkingDirectory.GetRelativePathTo(x).ToString()))
                .OrderBy(x => x.Item2).ToArray();

            return PromptForChoice("接下来要前往哪个目录？", directories);
        });
    }

    private static int PushAndSetNext(Func<string> directoryProvider)
    {
        try
        {
            var content = SessionFile.Existing()?.ReadAllLines().ToList() ?? new List<string> { null };
            content[0] = directoryProvider.Invoke();
            content.Insert(index: 1, EnvironmentInfo.WorkingDirectory);
            SessionFile.WriteAllLines(content);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }
}
