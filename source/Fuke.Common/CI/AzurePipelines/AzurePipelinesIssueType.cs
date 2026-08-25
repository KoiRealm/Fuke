// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.CI.AzurePipelines;

[PublicAPI]
public enum AzurePipelinesIssueType
{
    Warning,
    Error
}
