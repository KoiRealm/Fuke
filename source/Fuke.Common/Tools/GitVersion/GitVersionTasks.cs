// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.Tools.GitVersion;

partial class GitVersionTasks
{
    protected override object GetResult<T>(ToolOptions options, IReadOnlyCollection<Output> output)
    {
        try
        {
            return output.EnsureOnlyStd().StdToJson<GitVersion>();
        }
        catch (Exception exception)
        {
            throw new Exception($"Cannot parse {nameof(GitVersion)} output:".Concat(output.Select(x => x.Text)).JoinNewLine(), exception);
        }
    }
}

[PublicAPI]
public record GitVersion(
    int Major,
    int Minor,
    int Patch,
    string PreReleaseTag,
    string PreReleaseTagWithDash,
    string PreReleaseLabel,
    string PreReleaseLabelWithDash,
    string PreReleaseNumber,
    string WeightedPreReleaseNumber,
    string BuildMetaData,
    string BuildMetaDataPadded,
    string FullBuildMetaData,
    string MajorMinorPatch,
    string SemVer,
    string LegacySemVer,
    string LegacySemVerPadded,
    string AssemblySemVer,
    string AssemblySemFileVer,
    string FullSemVer,
    string InformationalVersion,
    string BranchName,
    string EscapedBranchName,
    string Sha,
    string ShortSha,
    string NuGetVersionV2,
    string NuGetVersion,
    string NuGetPreReleaseTagV2,
    string NuGetPreReleaseTag,
    string VersionSourceSha,
    string CommitsSinceVersionSource,
    string CommitsSinceVersionSourcePadded,
    int? UncommittedChanges,
    string CommitDate);
