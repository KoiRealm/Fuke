// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Reflection;
using Fuke.Common.Utilities;

namespace Fuke.Common.Tooling;

partial class ToolOptions
{
    internal Action<OutputType, string> ProcessLogger { get; set; }

    internal partial Action<OutputType, string> GetLogger()
    {
        var commandAttribute = GetType().GetCustomAttribute<CommandAttribute>().NotNull();
        var toolInstance = commandAttribute.Type.CreateInstance<ToolTasks>();
        return toolInstance.GetLogger(this);
    }
}
