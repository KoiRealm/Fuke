// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;

namespace Fuke.MSBuildTasks;

[UsedImplicitly]
public class EmbedPackagesForSelfContainedTask : ContextAwareTask
{
    [Required]
    public string ProjectAssetsFile { get; set; }

    [Required]
    public string TargetFramework { get; set; }

    [Output]
    public ITaskItem[] TargetOutputs { get; set; }

    protected override bool ExecuteInner()
    {
        var packages = NuGetPackageResolver.GetLocalInstalledPackages(ProjectAssetsFile);
        TargetOutputs = packages
            .Where(x => !x.Id.StartsWithOrdinalIgnoreCase("microsoft.netcore.app.runtime"))
            .Where(x => Directory.GetDirectories(x.Directory, "tools").Any())
            .Select(x => new TaskItem(x.File)).ToArray<ITaskItem>();
        return true;
    }
}
