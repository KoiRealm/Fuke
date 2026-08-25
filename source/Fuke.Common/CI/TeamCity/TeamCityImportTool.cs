// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.CI.TeamCity;

[PublicAPI]
public enum TeamCityImportTool
{
    /// <summary>dotCover reports</summary>
    dotcover,

    /// <summary>PartCover reports</summary>
    partcover,

    /// <summary>NCover reports</summary>
    ncover,

    /// <summary>NCover3 reports</summary>
    ncover3
}
