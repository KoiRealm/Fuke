// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityFinishBuildTrigger : TeamCityTrigger
{
    public TeamCityBuildType BuildType { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("finishBuildTrigger"))
        {
            writer.WriteLine($"buildType = {$"${{{BuildType.Id}.id}}".DoubleQuote()}");
        }
    }
}
