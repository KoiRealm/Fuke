// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.DotNet;
using static Fuke.Common.Tools.DotNet.DotNetTasks;

namespace Fuke.Components;

[PublicAPI]
public interface IRestore : IHazSolution, IFukeBuild
{
    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .Apply(RestoreSettingsBase)
                .Apply(RestoreSettings));
        });

    sealed Configure<DotNetRestoreSettings> RestoreSettingsBase => _ => _
        .SetProjectFile(Solution)
        .SetIgnoreFailedSources(IgnoreFailedSources);
    // RestorePackagesWithLockFile
    // .SetProperty("RestoreLockedMode", true));

    Configure<DotNetRestoreSettings> RestoreSettings => _ => _;

    [Parameter("在 " + nameof(Restore) + " 期间忽略无法访问的源")]
    bool IgnoreFailedSources => TryGetValue<bool?>(() => IgnoreFailedSources) ?? false;
}
