// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;

namespace Fuke.Components;

[PublicAPI]
public interface IHazConfiguration : IFukeBuild
{
    [Parameter] Configuration Configuration => TryGetValue(() => Configuration) ??
                                               (IsLocalBuild ? Configuration.Debug : Configuration.Release);
}
