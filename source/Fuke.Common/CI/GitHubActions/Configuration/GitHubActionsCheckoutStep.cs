// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.GitHubActions.Configuration;

[PublicAPI]
public class GitHubActionsCheckoutStep : GitHubActionsStep
{
    public GitHubActionsSubmodules? Submodules { get; set; }
    public bool? Lfs { get; set; }
    public uint? FetchDepth { get; set; }
    public bool? Progress { get; set; }
    public string Filter { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- uses: actions/checkout@v7");

        if (Submodules.HasValue || Lfs.HasValue || FetchDepth.HasValue || Progress.HasValue || !Filter.IsNullOrWhiteSpace())
        {
            using (writer.Indent())
            {
                writer.WriteLine("with:");
                using (writer.Indent())
                {
                    if (Submodules.HasValue)
                        writer.WriteLine($"submodules: {Submodules.ToString().ToLowerInvariant()}");
                    if(Lfs.HasValue)
                        writer.WriteLine($"lfs: {Lfs.ToString().ToLowerInvariant()}");
                    if (FetchDepth.HasValue)
                        writer.WriteLine($"fetch-depth: {FetchDepth}");
                    if (Progress.HasValue)
                        writer.WriteLine($"progress: {Progress.ToString().ToLowerInvariant()}");
                    if (!Filter.IsNullOrWhiteSpace())
                        writer.WriteLine($"filter: {Filter}");
                }
            }
        }
    }
}
