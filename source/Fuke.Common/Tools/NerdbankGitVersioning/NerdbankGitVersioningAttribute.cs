// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.CI.AppVeyor;
using Fuke.Common.CI.AzurePipelines;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.Tooling;
using Fuke.Common.ValueInjection;

namespace Fuke.Common.Tools.NerdbankGitVersioning;

/// <summary>
/// Injects an instance of <see cref="NerdbankGitVersioning"/> based on the local repository.
/// </summary>
[PublicAPI]
[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class NerdbankGitVersioningAttribute : ValueInjectionAttributeBase
{
    public bool UpdateBuildNumber { get; set; }

    public override object GetValue(MemberInfo member, object instance)
    {
        var version = NerdbankGitVersioningTasks.NerdbankGitVersioningGetVersion(s => s
                .DisableProcessOutputLogging()
                .SetFormat(NerdbankGitVersioningFormat.json))
            .Result;

        if (UpdateBuildNumber)
        {
            AzurePipelines.Instance?.UpdateBuildNumber(version.SemVer2);
            TeamCity.Instance?.SetBuildNumber(version.SemVer2);
            AppVeyor.Instance?.UpdateBuildVersion($"{version.SemVer2}.build.{AppVeyor.Instance.BuildNumber}");
        }

        return version;
    }
}
