// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.GitHubActions.Configuration;

public class GitHubActionsArtifactStep : GitHubActionsStep
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string Condition { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- name: " + $"Publish: {Name}".SingleQuote());
        writer.WriteLine("  uses: actions/upload-artifact@v5");

        using (writer.Indent())
        {
            if (!Condition.IsNullOrWhiteSpace())
            {
                writer.WriteLine($"if: {Condition}");
            }

            writer.WriteLine("with:");
            using (writer.Indent())
            {
                writer.WriteLine($"name: {Name}");
                writer.WriteLine($"path: {Path}");
            }
        }
    }
}
