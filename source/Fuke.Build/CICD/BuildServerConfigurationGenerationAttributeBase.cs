// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fuke.Common.Execution;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.CI;

public class BuildServerConfigurationGenerationAttributeBase : BuildExtensionAttributeBase
{
    protected static IEnumerable<IConfigurationGenerator> GetGenerators(IFukeBuild build)
    {
        return build.GetType().GetCustomAttributes<ConfigurationAttributeBase>()
            .ForEachLazy(x => x.Build = build);
    }
}
