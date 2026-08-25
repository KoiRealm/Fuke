// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Execution;

namespace Fuke.Common.IO;

/// <summary>
/// Allows to configure the case-sensitivity used for globbing operations in <see cref="PathConstruction"/>.
/// </summary>
[PublicAPI]
public sealed class GlobbingOptionsAttribute : BuildExtensionAttributeBase, IOnBuildCreated
{
    private readonly GlobbingCaseSensitivity _caseSensitivity;

    public GlobbingOptionsAttribute(GlobbingCaseSensitivity caseSensitivity)
    {
        _caseSensitivity = caseSensitivity;
    }

    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        Globbing.GlobbingCaseSensitivity = _caseSensitivity;
    }
}
