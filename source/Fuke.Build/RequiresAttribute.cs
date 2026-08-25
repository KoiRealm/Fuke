// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using Fuke.Common.Tooling;

namespace Fuke.Common;

public abstract class RequiresAttribute : Attribute
{
    public abstract ToolRequirement GetRequirement();
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
public class RequiresAttribute<T> : RequiresAttribute
    where T : IRequireTool
{
    public string Version { get; set; }

    public override ToolRequirement GetRequirement()
    {
        return typeof(T).GetCustomAttribute<ToolAttribute>().NotNull().GetRequirement(Version);
    }
}
