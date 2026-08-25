// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.GitHubActions.Configuration;

[PublicAPI]
public class GitHubActionsScheduledTrigger : GitHubActionsDetailedTrigger
{
    public string Cron { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("schedule:");
        using (writer.Indent())
        {
            writer.WriteLine($"- cron: '{Cron}'");
        }
    }
}
