// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using Fuke.Common.CI.AppVeyor;
using Fuke.Components;

[AppVeyor(
    suffix: null,
    AppVeyorImage.VisualStudio2022,
    BranchesOnly = new[] { MasterBranch, $"/{ReleaseBranchPrefix}\\/*/" },
    SkipTags = true,
    InvokedTargets = new[] { nameof(ITest.Test), nameof(IPack.Pack) },
    Secrets = new string[0])]
[AppVeyor(
    suffix: "continuous",
    AppVeyorImage.VisualStudioLatest,
    AppVeyorImage.UbuntuLatest,
    AppVeyorImage.MacOsLatest,
    BranchesExcept = new[] { MasterBranch, $"/{ReleaseBranchPrefix}\\/*/" },
    SkipTags = true,
    InvokedTargets = new[] { nameof(ITest.Test), nameof(IPack.Pack) },
    Secrets = new string[0])]
partial class Build
{
}
