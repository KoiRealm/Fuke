// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;

namespace Fuke.Common.CI;

public interface IBuildServer
{
    [CanBeNull]
    string Branch { get; }

    [CanBeNull]
    string Commit { get; }
}
