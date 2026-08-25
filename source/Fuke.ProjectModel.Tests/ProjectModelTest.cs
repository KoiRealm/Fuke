// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Xunit;

namespace Fuke.Common.Tests;

public class ProjectModelTest
{
    private static AbsolutePath RootDirectory => Constants.TryGetRootDirectoryFrom(EnvironmentInfo.WorkingDirectory).NotNull();

    private static AbsolutePath SolutionFile => RootDirectory / "fuke-common.slnx";

    [Fact]
    public void ProjectTest()
    {
        var solution = SolutionFile.ReadSolution();
        var project = solution.Projects.Single(x => x.Name == "Fuke.ProjectModel");

        var action = new Action(() => project.GetMSBuildProject());
        action.Should().NotThrow();

        project.GetTargetFrameworks().Should().Equal("net8.0", "net9.0", "net10.0");
        project.HasPackageReference("Microsoft.Build.Locator").Should().BeTrue();
        project.GetPackageReferenceVersion("Microsoft.Build.Locator").Should().Be("1.7.8");
        project.GetPackageReferenceVersion("Microsoft.Build").Should().Be("18.9.6");
    }

    [Fact]
    public void MSBuildProjectTest()
    {
        var solution = SolutionFile.ReadSolution();
        var project = solution.Projects.Single(x => x.Name == "Fuke.ProjectModel");

        ProjectModelTasks.Initialize();
        GetMicrosoftBuildPackageVersion(project).Should().Be("17.11.48");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string GetMicrosoftBuildPackageVersion(Project project)
    {
        var msbuildProject = project.GetMSBuildProject(targetFramework: "net8.0");
        var package = msbuildProject.GetItems("PackageVersion").FirstOrDefault(x => x.EvaluatedInclude == "Microsoft.Build");
        package.Should().NotBeNull();
        return package.GetMetadataValue("Version");
    }
}
