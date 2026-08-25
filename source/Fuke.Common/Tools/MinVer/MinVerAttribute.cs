// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.CI.AppVeyor;
using Fuke.Common.CI.AzurePipelines;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.Tooling;
using Fuke.Common.ValueInjection;

namespace Fuke.Common.Tools.MinVer;

/// <summary>
/// Injects an instance of <see cref="MinVer"/> based on the local repository.
/// </summary>
[PublicAPI]
[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class MinVerAttribute : ValueInjectionAttributeBase
{
    public string Framework { get; set; }
    public bool UpdateBuildNumber { get; set; }

    public override object GetValue(MemberInfo member, object instance)
    {
        var version = MinVerTasks.MinVer(s => s
                .SetFramework(Framework)
                .DisableProcessOutputLogging())
            .Result;

        if (UpdateBuildNumber)
        {
            AzurePipelines.Instance?.UpdateBuildNumber(version.Version);
            TeamCity.Instance?.SetBuildNumber(version.Version);
            AppVeyor.Instance?.UpdateBuildVersion(version.Version);
        }

        return version;
    }
}
