// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Collections.Generic;
using Fuke.Components;

partial class Build
{
    static Dictionary<string, string> CustomNames =
        new()
        {
            { nameof(ICompile.Compile), "⚙️" },
            { nameof(ITest.Test), "🚦" },
            { nameof(IPack.Pack), "📦" },
            { nameof(IReportCoverage.ReportCoverage), "📊" },
            { nameof(IReportDuplicates.ReportDuplicates), "🎭" },
            { nameof(IReportIssues.ReportIssues), "💣" },
            { nameof(ISignPackages.SignPackages), "🔑" },
            { nameof(IPublish.Publish), "🚚" },
            { nameof(Announce), "🗣" }
        };
}
