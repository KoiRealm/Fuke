// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityBuildTypeVcsRoot : ConfigurationEntity
{
    public TeamCityVcsRoot Root { get; set; }
    public bool ShowDependenciesChanges { get; set; }
    public bool CleanCheckoutDirectory { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("vcs"))
        {
            writer.WriteLine($"root({Root.Id})");
            if (CleanCheckoutDirectory)
                writer.WriteLine("cleanCheckout = true");
            if (ShowDependenciesChanges)
                writer.WriteLine("showDependenciesChanges = true");
        }
    }
}
