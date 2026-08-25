// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.Execution;

namespace Fuke.Common.Tools.DotCover;

[PublicAPI]
public class TeamCitySetDotCoverHomePathAttribute : BuildExtensionAttributeBase, IOnBuildInitialized
{
    public void OnBuildInitialized(
        IReadOnlyCollection<ExecutableTarget> executableTargets,
        IReadOnlyCollection<ExecutableTarget> executionPlan)
    {
        TeamCity.Instance?.SetConfigurationParameter("teamcity.dotCover.home", DotCoverTasks.DotCoverPath);
    }
}
