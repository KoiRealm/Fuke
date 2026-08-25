// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Tools.GitVersion;

namespace Fuke.Components;

[PublicAPI]
public interface IHazGitVersion : IFukeBuild
{
    [GitVersion(NoFetch = true, Framework = "net8.0")]
    [Required]
    GitVersion Versioning => TryGetValue(() => Versioning);
}
