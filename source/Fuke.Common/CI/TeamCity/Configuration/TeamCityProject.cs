// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityProject : ConfigurationEntity
{
    public string Description { get; set; }
    public TeamCityParameter[] Parameters { get; set; }
    public TeamCityVcsRoot VcsRoot { get; set; }
    public TeamCityBuildType[] BuildTypes { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("project"))
        {
            if (Description != null)
                writer.WriteLine($"description = {Description}");

            foreach (var buildType in BuildTypes)
                writer.WriteLine($"buildType({buildType.Id})");
            writer.WriteLine();

            writer.WriteLine($"buildTypesOrder = arrayListOf({BuildTypes.Select(x => x.Id).JoinCommaSpace()})");
            writer.WriteLine();

            if (Parameters.Any())
            {
                using (writer.WriteBlock("params"))
                {
                    foreach (var parameter in Parameters)
                        parameter.Write(writer);
                }
            }
        }
    }
}
