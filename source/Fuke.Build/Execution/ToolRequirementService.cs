// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;
using Serilog;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

internal static class ToolRequirementService
{
    public static void EnsureToolRequirements(IFukeBuild build, IReadOnlyCollection<ExecutableTarget> executionPlan)
    {
        var requirements = build.GetType().GetCustomAttributes<RequiresAttribute>().Select(x => x.GetRequirement())
            .Concat(executionPlan.SelectMany(x => x.ToolRequirements)).ToList();

        InstallNuGetPackages(requirements.OfType<NuGetPackageRequirement>().ToList(), build);
        InstallNpmPackages(requirements.OfType<NpmPackageRequirement>().ToList(), build);
        InstallAptGetPackages(requirements.OfType<AptGetPackageRequirement>().ToList(), build);
    }

    private static void InstallNuGetPackages(IReadOnlyCollection<NuGetPackageRequirement> requirements, IFukeBuild build)
    {
        if (requirements.Count == 0)
            return;

        var projectFile = build.TemporaryDirectory / "nuget.csproj";
        NuGetToolPathResolver.NuGetPackagesConfigFile = projectFile;
        NuGetToolPathResolver.NuGetAssetsConfigFile = projectFile.Parent / "obj" / "project.assets.json";

        var packages = requirements.OrderBy(x => x.PackageId).ThenBy(x => x.Version).ToList();
        var groupedPackages = packages.GroupBy(x => x.PackageId, x => $"[{x.Version}]");

        var content = $"""
                <Project Sdk="Microsoft.NET.Sdk">

                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                  </PropertyGroup>

                  <Import Project="{build.BuildProjectFile}" />

                  <ItemGroup>
                {groupedPackages.Select(x => $"""    <PackageDownload Include="{x.Key}" Version="{x.JoinSemicolon()}" Exclude="@(PackageDownload)" />""").JoinNewLine()}
                  </ItemGroup>

                </Project>
                """;

        if (projectFile.Exists() && projectFile.ReadAllText().StartsWith(content))
            return;

        Log.Information(L("Installing NuGet packages ...", "正在安装 NuGet 包……"));
        packages.ForEach(x => Log.Verbose(L("Installing {Id} ({Version}) ...", "正在安装 {Id} ({Version})……"), x.PackageId, x.Version));

        projectFile.WriteAllText(content);
        var dotnet = ToolResolver.GetEnvironmentOrPathTool("dotnet");
        dotnet.Invoke($"restore", workingDirectory: projectFile.Parent, logInvocation: false, logOutput: false);
    }

    private static void InstallNpmPackages(IReadOnlyCollection<NpmPackageRequirement> requirements, IFukeBuild build)
    {
        if (requirements.Count == 0)
            return;

        var packageJsonFile = build.TemporaryDirectory / "package.json";
        NpmToolPathResolver.NpmPackageJsonFile = packageJsonFile;

        var packages = requirements.OrderBy(x => x.PackageId).ToList();

        var content = $$"""
                {
                  "dependencies": {
                {{packages.Select(x => $"""    "{x.PackageId}": "{x.Version}",""").JoinNewLine().TrimEnd(',')}}
                  }
                }
                """;

        if (packageJsonFile.Exists() && packageJsonFile.ReadAllText().StartsWith(content))
            return;

        Log.Information(L("Installing NPM packages ...", "正在安装 NPM 包……"));
        packages.ForEach(x => Log.Verbose(L("Installing {Id} ({Version}) ...", "正在安装 {Id} ({Version})……"), x.PackageId, x.Version));

        packageJsonFile.WriteAllText(content);
        var npm = ToolResolver.GetEnvironmentOrPathTool("npm");
        npm.Invoke("install", workingDirectory: packageJsonFile.Parent, logInvocation: false, logOutput: false);
    }

    private static void InstallAptGetPackages(IReadOnlyCollection<AptGetPackageRequirement> requirements, IFukeBuild build)
    {
        if (requirements.Count == 0)
            return;

        var packages = requirements.OrderBy(x => x.PackageId).ToList();
        Assert.True(EnvironmentInfo.IsLinux, L("AptGet is only available on Linux.", "AptGet 仅在 Linux 上可用"));

        var installScript = build.TemporaryDirectory / "apt-get.sh";

        var content = $"""
                apt-get update
                apt-get install -y \
                {packages.Select(x => $"  {x.PackageId} \\").JoinNewLine().TrimEnd("\\")}
                """;

        if (installScript.Exists() && installScript.ReadAllText().StartsWith(content))
            return;

        Log.Information(L("Installing AptGet packages ...", "正在安装 AptGet 包……"));
        packages.ForEach(x => Log.Verbose(L("Installing {Id} ...", "正在安装 {Id}……"), x.PackageId));

        installScript.WriteAllText(content);
        ProcessTasks.StartShell($"sudo {installScript}", logInvocation: false, logOutput: false).AssertZeroExitCode();
    }
}
