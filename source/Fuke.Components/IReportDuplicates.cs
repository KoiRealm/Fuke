// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.IO;
using Fuke.Common.Tools.ReSharper;
using static Fuke.Common.Tools.ReSharper.ReSharperTasks;

namespace Fuke.Components;

[PublicAPI]
public interface IReportDuplicates : IHazReports, IHazSolution
{
    AbsolutePath DupFinderReportFile => ReportDirectory / "dupfinder.xml";

    Target ReportDuplicates => _ => _
        .TryAfter<ITest>()
        .Executes(() =>
        {
            ReSharperDupFinder(_ => _
                .SetSource(Solution)
                .SetOutputFile(DupFinderReportFile)
                .EnableShowText()
                .SetExcludeFiles(
                    "**/*.Generated.cs",
                    "**/obj/**",
                    "**/bin/**"));

            TeamCity.Instance?.ImportData(TeamCityImportType.DotNetDupFinder, DupFinderReportFile);
        });
}
