// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities;

[PublicAPI]
[DebuggerNonUserCode]
[DebuggerStepThrough]
public static partial class TaskExtensions
{
    public static void WaitAll(this IEnumerable<Task> tasks)
    {
        var tasksArray = tasks.ToArray();
        Task.WaitAll(tasksArray);
    }

    public static IReadOnlyCollection<T> WaitAll<T>(this IEnumerable<Task<T>> tasks)
    {
        var tasksArray = tasks.ToArray();
        Task.WaitAll(tasksArray);
        return tasksArray.Select(x => x.Result).ToList();
    }
}
