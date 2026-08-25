// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fuke.Common.IO;
using Serilog;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

internal class HandleReSharperSurrogateArgumentsAttribute : BuildExtensionAttributeBase, IOnBuildCreated
{
    private AbsolutePath ReSharperSurrogateFile => Constants.GetReSharperSurrogateFile(Build.RootDirectory);

    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        if (!ReSharperSurrogateFile.Exists())
            return;

        var argumentLines = ReSharperSurrogateFile.ReadAllLines();
        var lastWriteTime = File.GetLastWriteTime(ReSharperSurrogateFile);

        Assert.HasSingleItem(argumentLines, $"{ReSharperSurrogateFile} must have only one single line");
        ReSharperSurrogateFile.DeleteFile();
        if (lastWriteTime.AddMinutes(value: 1) < DateTime.Now)
        {
            Log.Warning(L(
                "{File} was last written at {LastWriteTime}; skipping it ...",
                "{File} 的最后写入时间为 {LastWriteTime}，已跳过……"), ReSharperSurrogateFile, lastWriteTime);
            return;
        }

        var arguments = argumentLines.Single();
        EnvironmentInfo.ArgumentParser = new ArgumentParser(arguments);
    }
}
