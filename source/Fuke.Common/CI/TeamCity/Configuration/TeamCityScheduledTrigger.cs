// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityScheduledTrigger : TeamCityTrigger
{
    public string[] BranchFilters { get; set; }
    public string[] TriggerRules { get; set; }
    public bool TriggerBuildAlways { get; set; }
    public bool WithPendingChangesOnly { get; set; }
    public bool EnableQueueOptimization { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("schedule"))
        {
            using (writer.WriteBlock("schedulingPolicy = daily"))
            {
                writer.WriteLine("hour = 3");
            }

            writer.WriteArray("branchFilter", BranchFilters);
            writer.WriteArray("triggerRules", TriggerRules);

            if (TriggerBuildAlways)
                writer.WriteLine("triggerBuild = always()");

            writer.WriteLine("withPendingChangesOnly = false");
            writer.WriteLine($"enableQueueOptimization = {EnableQueueOptimization.ToString().ToLowerInvariant()}");
            writer.WriteLine("param(\"cronExpression_min\", \"3\")");
        }
    }
}
