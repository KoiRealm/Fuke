// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using static Fuke.Common.ChangeLog.ChangelogTasks;

namespace Fuke.Components;

[PublicAPI]
public interface IHazChangelog : IFukeBuild
{
    // TODO: assert file exists
    string ChangelogFile => RootDirectory / "CHANGELOG.md";
    string NuGetReleaseNotes => GetNuGetReleaseNotes(ChangelogFile, (this as IHazGitRepository)?.GitRepository);
}