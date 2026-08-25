// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using FluentAssertions;
using Fuke.Common.Git;
using Fuke.Common.IO;
using Fuke.Common.Tools.GitHub;
using Xunit;

namespace Fuke.Common.Tests;

public class GitHubTasksTest
{
    private static AbsolutePath RootDirectory => Constants.TryGetRootDirectoryFrom(EnvironmentInfo.WorkingDirectory).NotNull();

    [Fact]
    public void GitHubRepositoryFromLocalDirectoryTest()
    {
        var repository = GitRepository.FromLocalDirectory(RootDirectory).NotNull();
        if (!repository.IsGitHubRepository())
            return;

        var rawUrl = $"https://raw.githubusercontent.com/{repository.Identifier}/{repository.Branch}";
        var blobUrl = $"https://github.com/{repository.Identifier}/blob/{repository.Branch}";
        var treeUrl = $"https://github.com/{repository.Identifier}/tree/{repository.Branch}";

        repository.GetGitHubDownloadUrl(RootDirectory / "LICENSE").Should().Be($"{rawUrl}/LICENSE");

        repository.GetGitHubBrowseUrl("LICENSE").Should().Be($"{blobUrl}/LICENSE");
        repository.GetGitHubBrowseUrl("source").Should().Be($"{treeUrl}/source");

        repository.GetGitHubBrowseUrl(RootDirectory / "LICENSE").Should().Be($"{blobUrl}/LICENSE");
        repository.GetGitHubBrowseUrl(RootDirectory / "source").Should().Be($"{treeUrl}/source");
        repository.GetGitHubBrowseUrl(RootDirectory / "source" / "Directory.Build.props").Should().Be($"{blobUrl}/source/Directory.Build.props");

        repository.GetGitHubBrowseUrl("directory", itemType: GitHubItemType.Directory).Should().Be($"{treeUrl}/directory");
        repository.GetGitHubBrowseUrl("dir/file", itemType: GitHubItemType.File).Should().Be($"{blobUrl}/dir/file");

        repository.GetGitHubBrowseUrl(branch: repository.Branch).Should().Be(treeUrl);
    }

    [Fact]
    public void GitHubRepositoryFromUrlTest()
    {
        var repository = GitRepository.FromUrl("https://github.com/KoiRealm/Fuke", "dev");

        repository.GetGitHubBrowseUrl("LICENSE", itemType: GitHubItemType.File).Should().Be($"{repository}/blob/dev/LICENSE");
        repository.GetGitHubBrowseUrl("source", itemType: GitHubItemType.Directory).Should().Be($"{repository}/tree/dev/source");
    }
}
