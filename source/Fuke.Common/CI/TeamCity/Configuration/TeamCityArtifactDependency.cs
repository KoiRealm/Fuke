// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityArtifactDependency : TeamCityDependency
{
    public TeamCityBuildType BuildType { get; set; }
    public string[] ArtifactRules { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock($"artifacts({BuildType.Id})"))
        {
            writer.WriteArray("artifactRules", ArtifactRules);
        }
    }
}
