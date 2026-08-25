// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.CI.AzurePipelines;

// https://docs.microsoft.com/en-us/azure/devops/pipelines/release/caching?view=azure-devops
public static class AzurePipelinesCachePaths
{
    public const string Fuke = ".fuke/temp";
    public const string NuGet = "~/.nuget/packages";
    public const string Npm = "~/.npm";
    public const string Gradle = "~/.gradle";
    public const string Docker = "~/docker";
}
