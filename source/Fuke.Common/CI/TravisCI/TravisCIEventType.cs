// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace Fuke.Common.CI.TravisCI;

[PublicAPI]
public enum TravisCIEventType
{
    push,
    pull_request,
    api,
    cron
}
