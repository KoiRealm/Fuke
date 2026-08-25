// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Tools.Docker;
using Fuke.Common.Utilities;
using Serilog;

partial class Build
{
    [UsedImplicitly]
    Target RunTargetInDockerImageTest => _ => _
        .DockerRun(_ => _
            .EnableBuildCaching()
            .SetImage("mcr.microsoft.com/dotnet/sdk:6.0")
            .When(EnvironmentInfo.IsArm64, _ => _
                .SetPlatform("linux/arm64")
                .SetDotNetRuntime("linux-arm64"))
            .When(EnvironmentInfo.IsWin, _ => _
                .SetPlatform("windows/amd64")
                .SetDotNetRuntime("win-x64"))
            .When(EnvironmentInfo.IsLinux, _ => _
                .SetPlatform("linux/amd64")
                .SetDotNetRuntime("linux-x64")))
        .Executes(() =>
        {
            Log.Information("Hello, the computer name is {Name}", Environment.MachineName);
        });
}
