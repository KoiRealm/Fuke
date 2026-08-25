// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.CI.TeamCity.Configuration;
using Fuke.Common.Execution;
using Fuke.Common.Utilities.Collections;
using Fuke.Components;

[TeamCity(
    VcsTriggeredTargets =
        new[]
        {
            nameof(IPack.Pack),
            nameof(ITest.Test),
            nameof(IReportDuplicates.ReportDuplicates),
            nameof(IReportIssues.ReportIssues),
            nameof(IReportCoverage.ReportCoverage)
        },
    NonEntryTargets =
        new[]
        {
            nameof(IRestore.Restore),
            nameof(DownloadLicenses),
            nameof(ICompile.Compile),
            nameof(InstallFonts),
            nameof(ReleaseImage)
        },
    ExcludedTargets = new[] { nameof(Clean), nameof(ISignPackages.SignPackages) })]
partial class Build
{
    public class TeamCityAttribute : Fuke.Common.CI.TeamCity.TeamCityAttribute
    {
        protected override IEnumerable<TeamCityBuildType> GetBuildTypes(
            ExecutableTarget executableTarget,
            TeamCityVcsRoot vcsRoot,
            LookupTable<ExecutableTarget, TeamCityBuildType> buildTypes,
            IReadOnlyCollection<ExecutableTarget> relevantTargets)
        {
            return base.GetBuildTypes(executableTarget, vcsRoot, buildTypes, relevantTargets)
                .ForEachLazy(x =>
                {
                    var symbol = CustomNames.GetValueOrDefault(x.InvokedTargets.Last());
                    x.Name = (x.Partition == null
                        ? $"{symbol} {x.Name}"
                        : $"{symbol} {x.InvokedTargets.Last()} 🧩 {x.Partition}").Trim();
                });
        }
    }
}
