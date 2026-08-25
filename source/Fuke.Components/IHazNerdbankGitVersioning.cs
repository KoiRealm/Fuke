// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Tools.NerdbankGitVersioning;

namespace Fuke.Components;

[PublicAPI]
public interface IHazNerdbankGitVersioning : IFukeBuild
{
    [NerdbankGitVersioning] [Required] NerdbankGitVersioning Versioning => TryGetValue(() => Versioning);
}
