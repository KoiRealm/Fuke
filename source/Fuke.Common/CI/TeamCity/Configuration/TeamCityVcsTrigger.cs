// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityVcsTrigger : TeamCityTrigger
{
    public string[] BranchFilters { get; set; }
    public string[] TriggerRules { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("vcs"))
        {
            writer.WriteArray("branchFilter", BranchFilters);
            writer.WriteArray("triggerRules", TriggerRules);
        }
    }
}
