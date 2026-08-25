// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Execution;

public enum ExecutionStatus
{
    None,
    Scheduled,
    NotRun,
    Skipped,
    Succeeded,
    Failed,
    Running,
    Aborted,
    Collective
}
