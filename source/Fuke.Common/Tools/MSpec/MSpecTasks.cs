// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Tooling;

namespace Fuke.Common.Tools.MSpec;

partial class MSpecTasks
{
    protected override string GetToolPath(ToolOptions options = null)
    {
        return NuGetToolPathResolver.GetPackageExecutable(
            PackageId,
            EnvironmentInfo.Is64Bit ? "mspec-clr4.exe" : "mspec-x86-clr4.exe");
    }
}
