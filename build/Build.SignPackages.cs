// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fuke.Common;
using Fuke.Common.Git;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Fuke.Components;

partial class Build : ISignPackages
{
    public IEnumerable<AbsolutePath> SignPathPackages => NuGetPackageFiles;

    public Target SignPackages => _ => _
        .Inherit<ISignPackages>()
        .OnlyWhenStatic(() => IsPublicRelease)
        .OnlyWhenStatic(() => EnvironmentInfo.IsWin);
}
