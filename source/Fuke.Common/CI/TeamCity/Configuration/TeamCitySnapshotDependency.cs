// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCitySnapshotDependency : TeamCityDependency
{
    public TeamCityBuildType BuildType { get; set; }
    public TeamCityDependencyFailureAction FailureAction { get; set; }
    public TeamCityDependencyFailureAction CancelAction { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        static string FormatAction(TeamCityDependencyFailureAction action)
            => "FailureAction." +
               action.ToString().SplitCamelHumps().JoinUnderscore().ToUpperInvariant();

        using (writer.WriteBlock($"snapshot({BuildType.Id})"))
        {
            writer.WriteLine($"onDependencyFailure = {FormatAction(FailureAction)}");
            writer.WriteLine($"onDependencyCancel = {FormatAction(CancelAction)}");
        }
    }
}
