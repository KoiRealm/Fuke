// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.CI.GitHubActions.Configuration;

[PublicAPI]
public class GitHubActionsRunStep : GitHubActionsStep
{
    public string BuildCmdPath { get; set; }
    public string[] InvokedTargets { get; set; }
    public Dictionary<string, string> Imports { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- name: " + $"Run: {InvokedTargets.JoinCommaSpace()}".SingleQuote());
        writer.WriteLine($"  run: ./{BuildCmdPath} {InvokedTargets.JoinSpace()}");

        if (Imports.Count > 0)
        {
            using (writer.Indent())
            {
                writer.WriteLine("env:");
                using (writer.Indent())
                {
                    Imports.ForEach(x => writer.WriteLine($"{x.Key}: {x.Value}"));
                }
            }
        }
    }
}
