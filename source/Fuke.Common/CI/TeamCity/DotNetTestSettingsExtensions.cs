// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.DotNet;

namespace Fuke.Common.CI.TeamCity;

[PublicAPI]
public static class DotNetTestSettingsExtensions
{
    public static DotNetTestSettings AddTeamCityLogger(this DotNetTestSettings toolSettings)
    {
        Assert.True(TeamCity.Instance != null);
        var teamcityPackage = NuGetPackageResolver
            .GetLocalInstalledPackage("TeamCity.Dotnet.Integration", NuGetToolPathResolver.NuGetPackagesConfigFile)
            .NotNull("teamcityPackage != null");
        var loggerPath = teamcityPackage.Directory / "build" / "_common" / "vstest15";
        Assert.DirectoryExists(loggerPath);
        return toolSettings
            .SetLoggers("teamcity")
            .SetTestAdapterPath(loggerPath);
    }
}
