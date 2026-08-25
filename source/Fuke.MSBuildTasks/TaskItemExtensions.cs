// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Microsoft.Build.Framework;

namespace Fuke.MSBuildTasks;

public static class TaskItemExtensions
{
    public static string GetMetadataOrNull(this ITaskItem taskItem, string metdataName)
    {
        return taskItem.MetadataNames.Cast<string>().Contains(metdataName)
            ? taskItem.GetMetadata(metdataName)
            : null;
    }
}
