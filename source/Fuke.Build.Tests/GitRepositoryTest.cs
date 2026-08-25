// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Fuke.Common.Git;
using Xunit;

namespace Fuke.Common.Tests;

public class GitRepositoryTest
{
    [Theory]
    [InlineData("https://github.com/example-org", "github.com", "example-org")]
    [InlineData("https://github.com/example-org/", "github.com", "example-org")]
    [InlineData("https://github.com/KoiRealm/Fuke", "github.com", "KoiRealm/Fuke")]
    [InlineData("https://github.com/KoiRealm/Fuke.git", "github.com", "KoiRealm/Fuke")]
    [InlineData("https://user:pass@github.com/example-org/example-repo.git", "github.com", "example-org/example-repo")]
    [InlineData(" https://github.com/TdMxm/fuke.git", "github.com", "TdMxm/fuke")]
    [InlineData("git@git.test.org:test", "git.test.org", "test")]
    [InlineData("git@git.test.org/test", "git.test.org", "test")]
    [InlineData("git@git.test.org/test/", "git.test.org", "test")]
    [InlineData("git@git.test.org/test.git", "git.test.org", "test")]
    [InlineData("ssh://git@git.test.org/test.git", "git.test.org", "test")]
    [InlineData("ssh://git@git.test.org:1234/test.git", "git.test.org", "test")]
    [InlineData("ssh://git.test.org/test/test", "git.test.org", "test/test")]
    [InlineData("ssh://git.test.org:1234/test/test", "git.test.org", "test/test")]
    [InlineData("https://git.test.org:1234/test/test", "git.test.org", "test/test")]
    [InlineData("git://git.test.org:1234/test/test", "git.test.org", "test/test")]
    [InlineData("git://git.test.org/test/test", "git.test.org", "test/test")]
    public void FromUrlTest(string url, string endpoint, string identifier)
    {
        var repository = GitRepository.FromUrl(url);
        repository.Endpoint.Should().Be(endpoint);
        repository.Identifier.Should().Be(identifier);
    }

    [Theory]
    [InlineData("https://github.com/example-org", GitProtocol.Https)]
    [InlineData("git@git.test.org:test", GitProtocol.Ssh)]
    [InlineData("ssh://git.test.org:1234/test/test", GitProtocol.Ssh)]
    [InlineData("git://git.test.org:1234/test/test", GitProtocol.Ssh)]
    public void FromUrlProtocolTest(string url, GitProtocol protocol)
    {
        var repository = GitRepository.FromUrl(url);
        repository.Protocol.Should().Be(protocol);
    }

    [Fact]
    public void FromDirectoryTest()
    {
        var repository = GitRepository.FromLocalDirectory(Directory.GetCurrentDirectory()).NotNull();
        repository.Endpoint.Should().NotBeNullOrEmpty();
        repository.Identifier.Should().NotBeNullOrEmpty();
        repository.LocalDirectory.Should().NotBeNull();
        repository.Head.Should().NotBeNullOrEmpty();
        repository.Commit.Should().NotBeNullOrEmpty();
        repository.Tags.Should().NotBeNull();
    }
}
