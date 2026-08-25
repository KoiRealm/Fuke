// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Tools.MSBuild;

public static partial class MSBuildSettingsExtensions
{
    /// <summary><em>Sets <see cref="MSBuildSettings.TargetPath" />.</em></summary>
    public static MSBuildSettings SetSolutionFile(this MSBuildSettings toolSettings, string solutionFile)
    {
        return toolSettings.SetTargetPath(solutionFile);
    }

    /// <summary><em>Sets <see cref="MSBuildSettings.TargetPath" />.</em></summary>
    public static MSBuildSettings SetProjectFile(this MSBuildSettings toolSettings, string projectFile)
    {
        return toolSettings.SetTargetPath(projectFile);
    }
}
