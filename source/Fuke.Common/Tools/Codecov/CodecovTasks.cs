// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using Fuke.Common.Tooling;

namespace Fuke.Common.Tools.Codecov;

partial class CodecovTasks
{
    protected override string GetToolPath(ToolOptions options = null)
    {
        return NuGetToolPathResolver.GetPackageExecutable(
            packageId: PackageId,
            packageExecutable: EnvironmentInfo.Platform switch
            {
                PlatformFamily.Windows => "codecov.exe",
                PlatformFamily.OSX => "codecov-macos",
                PlatformFamily.Linux => "codecov-linux",
                _ => throw new ArgumentOutOfRangeException()
            });
    }
}
