// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityVcsRoot : ConfigurationEntity
{
    public string Id => "DslContext.settingsRoot";

    public override void Write(CustomFileWriter writer)
    {
    }
}
