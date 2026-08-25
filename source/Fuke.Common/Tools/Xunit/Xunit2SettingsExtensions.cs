// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Fuke.Common.Tools.Xunit;

partial class Xunit2SettingsExtensions
{
    public static Xunit2Settings AddTargetAssemblies(this Xunit2Settings toolSettings, IEnumerable<string> assemblyFiles)
    {
        return assemblyFiles.Aggregate(
            toolSettings,
            (current, assembly) => current.AddTargetAssemblyWithConfigs(assembly, string.Empty));
    }

    public static Xunit2Settings AddTargetAssemblies(this Xunit2Settings toolSettings, params string[] assemblyFiles)
    {
        return toolSettings.AddTargetAssemblies(assemblyFiles.AsEnumerable());
    }
}
