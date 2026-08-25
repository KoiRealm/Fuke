// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.IO;
using Fuke.Common.Tools.GitHub;
using Fuke.Common.Utilities.Collections;
using static Fuke.CodeGeneration.CodeGenerator;
using static Fuke.CodeGeneration.ReferenceUpdater;
using static Fuke.Common.Tools.Git.GitTasks;

partial class Build
{
    AbsolutePath SpecificationsDirectory => RootDirectory / "source" / "Fuke.Common" / "Tools";
    AbsolutePath ReferencesDirectory => BuildProjectDirectory / "references";

    Target References => _ => _
        .Requires(() => GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            ReferencesDirectory.CreateOrCleanDirectory();

            UpdateReferences(SpecificationsDirectory, ReferencesDirectory);
        });

    [UsedImplicitly]
    Target GenerateTools => _ => _
        .Executes(() =>
        {
            SpecificationsDirectory.GlobFiles("*/*.json").ForEach(x =>
                GenerateCode(
                    x,
                    namespaceProvider: x => $"Fuke.Common.Tools.{x.Name}",
                    sourceFileProvider: x => GitRepository.SetBranch(MainBranch).GetGitHubBrowseUrl(x.SpecificationFile)));
        });
}
