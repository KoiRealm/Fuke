// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.CI.TeamCity;

[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum TeamCityStatus
{
    NORMAL,
    WARNING,
    ERROR,
    FAILURE
}
