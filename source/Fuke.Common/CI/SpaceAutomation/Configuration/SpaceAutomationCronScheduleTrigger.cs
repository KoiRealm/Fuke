// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.SpaceAutomation.Configuration;

[PublicAPI]
public class SpaceAutomationCronScheduleTrigger : SpaceAutomationTrigger
{
    public string CronExpression { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine($"schedule {{ cron({CronExpression.DoubleQuote()}) }}");
    }
}
