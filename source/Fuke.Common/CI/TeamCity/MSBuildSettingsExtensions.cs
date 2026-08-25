// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tools.MSBuild;

namespace Fuke.Common.CI.TeamCity;

[PublicAPI]
public static class MSBuildSettingsExtensions
{
    public static MSBuildSettings AddTeamCityLogger(this MSBuildSettings toolSettings)
    {
        var teamCity = TeamCity.Instance.NotNull("TeamCity.Instance != null");
        var teamCityLogger = teamCity.ConfigurationProperties["teamcity.dotnet.msbuild.extensions4.0"];
        return toolSettings
            .AddLoggers($"JetBrains.BuildServer.MSBuildLoggers.MSBuildLogger,{teamCityLogger}")
            .EnableNoConsoleLogger();
    }
}
