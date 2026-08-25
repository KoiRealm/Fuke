// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Git;

namespace Fuke.Components;

[PublicAPI]
public interface IHazGitRepository : IFukeBuild
{
    [GitRepository] [Required] GitRepository GitRepository => TryGetValue(() => GitRepository);
}
