// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Tools.MSBuild;

[PublicAPI]
public class MSBuildProject : DynamicObject
{
    internal MSBuildProject(
        bool isSdkProject,
        IReadOnlyDictionary<string, string> properties,
        ILookup<string, string> itemGroups)
    {
        IsSdkProject = isSdkProject;
        Properties = properties;
        ItemGroups = itemGroups;
    }

    public bool IsSdkProject { get; }
    public bool IsLegacyProject => !IsSdkProject;
    public IReadOnlyDictionary<string, string> Properties { get; }
    public ILookup<string, string> ItemGroups { get; }
}
