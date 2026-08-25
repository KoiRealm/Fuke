// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.CI.GitHubActions.Configuration;

// https://github.com/actions/cache
[PublicAPI]
public class GitHubActionsCacheStep : GitHubActionsStep
{
    public string[] IncludePatterns { get; set; }
    public string[] ExcludePatterns { get; set; }
    public string[] KeyFiles { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- name: " + $"Cache: {IncludePatterns.JoinCommaSpace()}".SingleQuote());
        using (writer.Indent())
        {
            writer.WriteLine("uses: actions/cache@v5");
            writer.WriteLine("with:");
            using (writer.Indent())
            {
                writer.WriteLine("path: |");
                IncludePatterns.ForEach(x => writer.WriteLine($"  {x}"));
                ExcludePatterns.ForEach(x => writer.WriteLine($"  !{x}"));
                writer.WriteLine($"key: ${{{{ runner.os }}}}-${{{{ hashFiles({KeyFiles.Select(x => x.SingleQuote()).JoinCommaSpace()}) }}}}");
            }
        }
    }
}
