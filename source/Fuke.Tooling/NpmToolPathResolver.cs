// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.IO;

namespace Fuke.Common.Tooling;

[PublicAPI]
public static class NpmToolPathResolver
{
    public static AbsolutePath NpmPackageJsonFile;

    public static string GetNpmExecutable(string npmExecutable)
    {
        Assert.FileExists(NpmPackageJsonFile);

        return ProcessTasks.StartProcess(
                toolPath: ToolPathResolver.GetPathExecutable("npx"),
                arguments: $"which {npmExecutable}",
                workingDirectory: NpmPackageJsonFile.Parent / "node_modules",
                logInvocation: false,
                logOutput: false)
            .AssertZeroExitCode()
            .Output.StdToText();
    }
}