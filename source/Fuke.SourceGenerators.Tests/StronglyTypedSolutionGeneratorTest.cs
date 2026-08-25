// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Fuke.Common;
using Fuke.Common.ProjectModel;
using VerifyXunit;
using Xunit;

namespace Fuke.SourceGenerators.Tests;

public class StronglyTypedSolutionGeneratorTest
{
    [Fact]
    public Task Test()
    {
        var inputCompilation = CreateCompilation("""
                using Fuke.Common;
                using Fuke.Common.ProjectModel;
                partial class Build : FukeBuild
                {
                    [Solution(GenerateProjects = true)]
                    readonly Solution Solution;
                }
                """);

        var generator = new StronglyTypedSolutionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var result = driver.RunGenerators(inputCompilation, TestContext.Current.CancellationToken);
        return Verifier.Verify(result);
    }

    [Fact]
    public void TestDisabled()
    {
        var inputCompilation = CreateCompilation("""

                using Fuke.Common;
                using Fuke.Common.ProjectModel;

                partial class Build : FukeBuild
                {
                    [Solution(GenerateProjects = false)]
                    readonly Solution Solution;
                }
                """);

        var generator = new StronglyTypedSolutionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var result = driver.RunGenerators(inputCompilation, TestContext.Current.CancellationToken).GetRunResult();

        if (!result.Diagnostics.IsEmpty)
            throw new Exception(string.Join(Environment.NewLine, result.Diagnostics.Select(x => x.GetMessage())));
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void TestUnspecified()
    {
        var inputCompilation = CreateCompilation("""

                using Fuke.Common;
                using Fuke.Common.ProjectModel;

                partial class Build : FukeBuild
                {
                    [Solution]
                    readonly Solution Solution;
                }
                """);

        var generator = new StronglyTypedSolutionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var result = driver.RunGenerators(inputCompilation, TestContext.Current.CancellationToken).GetRunResult();

        if (!result.Diagnostics.IsEmpty)
            throw new Exception(string.Join(Environment.NewLine, result.Diagnostics.Select(x => x.GetMessage())));
        result.GeneratedTrees.Should().BeEmpty();
    }

    private static Compilation CreateCompilation(string source)
    {
        return CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            Basic.Reference.Assemblies.NetStandard20.References.All
                .Concat(new[] { typeof(FukeBuild), typeof(SolutionAttribute) }
                    .Select(x => MetadataReference.CreateFromFile(x.Assembly.Location))),
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
