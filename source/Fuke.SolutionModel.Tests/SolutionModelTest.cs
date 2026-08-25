// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using FluentAssertions;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Utilities;
using Xunit;

namespace Fuke.Common.Tests;

public class SolutionModelTest
{
    private static AbsolutePath RootDirectory => Constants.TryGetRootDirectoryFrom(EnvironmentInfo.WorkingDirectory).NotNull();

    private static AbsolutePath SolutionFile => RootDirectory / "fuke-common.slnx";

    [Fact]
    public void SolutionTest()
    {
        var solution = SolutionFile.ReadSolution();

        solution.SolutionFolders.Select(x => x.Name).Should().BeEquivalentTo("misc");

        var buildProject = solution.AllProjects.SingleOrDefault(x => x.Name == "_build");
        buildProject.Should().NotBeNull();

        // solution.SaveAs(solution.Path + ".bak");
    }

    [Fact]
    public void SolutionGetProjectsTest()
    {
        var solution = SolutionFile.ReadSolution();

        solution.GetAllProjects("*.Tests").Should().HaveCountGreaterOrEqualTo(2);
    }
}
