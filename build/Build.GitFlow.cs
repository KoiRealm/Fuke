// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Git;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.GitHub;
using Fuke.Components;
using NuGet.Versioning;
using Octokit;
using Serilog;
using static Fuke.Common.ChangeLog.ChangelogTasks;
using static Fuke.Common.Tools.Git.GitTasks;

partial class Build
{
    [Parameter] readonly bool AutoStash = true;
    string ReleaseVersion => FukeVersion;
    string MilestoneTitle => $"v{ReleaseVersion}";

    Target Milestone => _ => _
        .Unlisted()
        .OnlyWhenStatic(() => GitRepository.IsOnReleaseBranch() || GitRepository.IsOnHotfixBranch())
        .Executes(async () =>
        {
            var milestone = await GitRepository.GetGitHubMilestone(MilestoneTitle);
            if (milestone == null)
                return;

            Assert.True(milestone.OpenIssues == 0);
            Assert.True(milestone.ClosedIssues != 0);
            Assert.True(milestone.State == ItemState.Closed);
        });

    Target Changelog => _ => _
        .Unlisted()
        .DependsOn(Milestone)
        .OnlyWhenStatic(() => GitRepository.IsOnReleaseBranch() || GitRepository.IsOnHotfixBranch())
        .Executes(() =>
        {
            var changelogFile = From<IHazChangelog>().ChangelogFile;
            FinalizeChangelog(changelogFile, ReleaseVersion, GitRepository);
            Log.Information("请检查 CHANGELOG.md，然后按任意键继续……");
            System.Console.ReadKey();

            Git($"add {changelogFile}");
            Git($"commit -m \"chore: {Path.GetFileName(changelogFile)} for {ReleaseVersion}\"");
        });

    [UsedImplicitly]
    Target Release => _ => _
        .DependsOn(Changelog)
        .Requires(() => !GitRepository.IsOnReleaseBranch() || GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            if (!GitRepository.IsOnReleaseBranch())
                Checkout($"{ReleaseBranchPrefix}/{ReleaseVersion}", start: DevelopBranch);
            else
                FinishReleaseOrHotfix();
        });

    [UsedImplicitly]
    Target Hotfix => _ => _
        .DependsOn(Changelog)
        .Requires(() => !GitRepository.IsOnHotfixBranch() || GitHasCleanWorkingCopy())
        .Executes(() =>
        {
            var currentVersion = NuGetVersion.Parse(ReleaseVersion);
            Assert.False(currentVersion.IsPrerelease, $"不能从预发行版本创建 Hotfix：{ReleaseVersion}");
            var nextVersion = new NuGetVersion(
                currentVersion.Major,
                currentVersion.Minor,
                currentVersion.Patch + 1);

            if (!GitRepository.IsOnHotfixBranch())
                Checkout($"{HotfixBranchPrefix}/{nextVersion.ToNormalizedString()}", start: MainBranch);
            else
                FinishReleaseOrHotfix();
        });

    void FinishReleaseOrHotfix()
    {
        Git($"checkout {MainBranch}");
        Git($"merge --no-ff --no-edit {GitRepository.Branch}");
        var releaseTag = $"v{ReleaseVersion}";
        Git($"tag {releaseTag}");

        Git($"checkout {DevelopBranch}");
        Git($"merge --no-ff --no-edit {GitRepository.Branch}");

        Git($"branch -D {GitRepository.Branch}");

        Git($"push origin {MainBranch} {DevelopBranch} {releaseTag}");
    }

    void Checkout(string branch, string start)
    {
        var hasCleanWorkingCopy = GitHasCleanWorkingCopy();

        if (!hasCleanWorkingCopy && AutoStash)
            Git("stash");

        Git($"checkout -b {branch} {start}");

        if (!hasCleanWorkingCopy && AutoStash)
            Git("stash apply");
    }
}
