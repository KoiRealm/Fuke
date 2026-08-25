// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Tools.DotNet;
using static Fuke.Common.Tools.DotNet.DotNetTasks;

namespace Fuke.Components;

[PublicAPI]
public interface IGlobalTool : IFukeBuild
{
    string GlobalToolPackageName => Path.GetFileNameWithoutExtension(BuildProjectFile);
    string GlobalToolVersion => "1.0.0";

    Target PackGlobalTool => _ => _
        .Unlisted()
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(BuildProjectFile)
                .SetOutputDirectory(TemporaryDirectory));
        });

    Target InstallGlobalTool => _ => _
        .Unlisted()
        .DependsOn(UninstallGlobalTool)
        .DependsOn(PackGlobalTool)
        .Executes(() =>
        {
            DotNetToolInstall(_ => _
                .SetPackageName(GlobalToolPackageName)
                .EnableGlobal()
                .AddSources(TemporaryDirectory)
                .SetVersion(GlobalToolVersion));
        });

    Target UninstallGlobalTool => _ => _
        .Unlisted()
        .ProceedAfterFailure()
        .Executes(() =>
        {
            DotNetToolUninstall(_ => _
                .SetPackageName(GlobalToolPackageName)
                .EnableGlobal());
        });
}
