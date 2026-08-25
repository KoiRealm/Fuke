// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.Tools.DotNet.DotNetTasks;

namespace Fuke.Components;

[PublicAPI]
public interface IPack : ICompile, IHazArtifacts
{
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(_ => _
                .Apply(PackSettingsBase)
                .Apply(PackSettings));

            ReportSummary(_ => _
                .AddPair("Packages", PackagesDirectory.GlobFiles("*.nupkg").Count.ToString()));
        });

    sealed Configure<DotNetPackSettings> PackSettingsBase => _ => _
        .SetProject(Solution)
        .SetConfiguration(Configuration)
        .SetNoBuild(SucceededTargets.Contains(Compile))
        .SetOutputDirectory(PackagesDirectory)
        .WhenNotNull(this as IHazGitRepository, (_, o) => _
            .SetRepositoryUrl(o.GitRepository.HttpsUrl))
        .WhenNotNull(this as IHazGitVersion, (_, o) => _
            .SetVersion(o.Versioning.NuGetVersionV2))
        .WhenNotNull(this as IHazNerdbankGitVersioning, (_, o) => _
            .SetVersion(o.Versioning.NuGetPackageVersion))
        .WhenNotNull(this as IHazChangelog, (_, o) => _
            .SetPackageReleaseNotes(o.NuGetReleaseNotes));

    Configure<DotNetPackSettings> PackSettings => _ => _;
}
