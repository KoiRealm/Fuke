// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common;
using Fuke.Common.Execution;

[DisableDefaultOutput<Terminal>(
    DefaultOutput.Timestamps,
    DefaultOutput.TargetHeader,
    DefaultOutput.ErrorsAndWarnings,
    DefaultOutput.TargetOutcome,
    DefaultOutput.BuildOutcome)]
partial class Build
{
}

public class DisableDefaultOutputAttribute<T> : DisableDefaultOutputAttribute
    where T : Host
{
    public DisableDefaultOutputAttribute(params DefaultOutput[] disabledOutputs)
        : base(disabledOutputs)
    {
    }

    public override bool IsApplicable(IFukeBuild build) => build.Host is T;
}
