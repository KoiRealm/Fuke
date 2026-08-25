// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Common.Tools.Coverlet;

public static partial class CoverletSettingsExtensions
{
    /// <summary>
    /// <p><em>Sets <see cref="CoverletSettings.Target"/> and <see cref="CoverletSettings.TargetArgs"/> to the values defined by <paramref name="targetSettings"/>.</em></p>
    /// </summary>
    /// <returns></returns>
    [Pure]
    public static CoverletSettings SetTargetSettings(this CoverletSettings toolSettings, ToolOptions targetSettings)
    {
        return toolSettings
            .SetTarget(targetSettings.ProcessToolPath)
            .SetTargetArgs(targetSettings.GetArguments());
    }

    /// <summary>
    /// <p><em>Resets <see cref="CoverletSettings.Target"/> and <see cref="CoverletSettings.TargetArgs"/>.</em></p>
    /// </summary>
    [Pure]
    public static CoverletSettings ResetTargetSettings(this CoverletSettings toolSettings)
    {
        return toolSettings
            .ResetTarget()
            .ClearTargetArgs();
    }
}
