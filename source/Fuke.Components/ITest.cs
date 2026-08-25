// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.CI.AzurePipelines;
using Fuke.Common.CI.TeamCity;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.Coverlet;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.Tools.DotNet.DotNetTasks;

namespace Fuke.Components;

[PublicAPI]
public interface ITest : ICompile, IHazArtifacts
{
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";

    int TestDegreeOfParallelism => 1;

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            try
            {
                DotNetTest(_ => _
                        .Apply(TestSettingsBase)
                        .Apply(TestSettings)
                        .CombineWith(TestProjects, (_, v) => _
                            .Apply(TestProjectSettingsBase, v)
                            .Apply(TestProjectSettings, v)),
                    completeOnFailure: true,
                    degreeOfParallelism: TestDegreeOfParallelism);
            }
            finally
            {
                ReportTestResults();
                ReportTestCount();
            }
        });

    void ReportTestResults()
    {
        TestResultDirectory.GlobFiles("*.trx").ForEach(x =>
            AzurePipelines.Instance?.PublishTestResults(
                type: AzurePipelinesTestResultsType.VSTest,
                title: $"{Path.GetFileNameWithoutExtension(x)} ({AzurePipelines.Instance.StageDisplayName})",
                files: new string[] { x }));
    }

    void ReportTestCount()
    {
        IEnumerable<string> GetOutcomes(AbsolutePath file)
            => XmlTasks.XmlPeek(
                file,
                "/xn:TestRun/xn:Results/xn:UnitTestResult/@outcome",
                ("xn", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"));

        var resultFiles = TestResultDirectory.GlobFiles("*.trx");
        var outcomes = resultFiles.SelectMany(GetOutcomes).ToList();
        var passedTests = outcomes.Count(x => x == "Passed");
        var failedTests = outcomes.Count(x => x == "Failed");
        var skippedTests = outcomes.Count(x => x == "NotExecuted");

        ReportSummary(_ => _
            .When(failedTests > 0, _ => _
                .AddPair("Failed", failedTests.ToString()))
            .AddPair("Passed", passedTests.ToString())
            .When(skippedTests > 0, _ => _
                .AddPair("Skipped", skippedTests.ToString())));
    }

    sealed Configure<DotNetTestSettings> TestSettingsBase => _ => _
        .SetConfiguration(Configuration)
        .SetNoBuild(SucceededTargets.Contains(Compile))
        .ResetVerbosity()
        .SetResultsDirectory(TestResultDirectory)
        .When(InvokedTargets.Contains((this as IReportCoverage)?.ReportCoverage) || IsServerBuild, _ => _
            .EnableCollectCoverage()
            .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
            .SetExcludeByFile("*.Generated.cs")
            .When(TeamCity.Instance is not null, _ => _
                .SetCoverletOutputFormat($"\\\"{CoverletOutputFormat.cobertura},{CoverletOutputFormat.teamcity}\\\""))
            .When(IsServerBuild, _ => _
                .EnableUseSourceLink()));

    sealed Configure<DotNetTestSettings, Project> TestProjectSettingsBase => (_, v) => _
        .SetProjectFile(v)
        .EnableReportTrx()
        .SetReportTrxFilename($"{v.Name}.trx")
        .When(InvokedTargets.Contains((this as IReportCoverage)?.ReportCoverage) || IsServerBuild, _ => _
            .SetCoverletOutput(TestResultDirectory / $"{v.Name}.xml"));

    Configure<DotNetTestSettings> TestSettings => _ => _;
    Configure<DotNetTestSettings, Project> TestProjectSettings => (_, v) => _;

    IEnumerable<Project> TestProjects { get; }
}
