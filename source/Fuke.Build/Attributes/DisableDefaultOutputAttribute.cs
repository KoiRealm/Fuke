// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public class DisableDefaultOutputAttribute : Attribute
{
    public DisableDefaultOutputAttribute(params DefaultOutput[] disabledOutputs)
    {
        DisabledOutputs = disabledOutputs.Length > 0
            ? disabledOutputs
            : Enum.GetValues(typeof(DefaultOutput)).Cast<DefaultOutput>().ToArray();
    }

    public DefaultOutput[] DisabledOutputs { get; }

    public virtual bool IsApplicable(IFukeBuild build)
    {
        return true;
    }
}

public enum DefaultOutput
{
    Logo,
    TargetHeader,
    TargetCollapse,
    ErrorsAndWarnings,
    TargetOutcome,
    BuildOutcome,
    Timestamps
}
