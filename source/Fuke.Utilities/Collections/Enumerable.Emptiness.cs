// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Collections.Generic;
using System.Linq;

namespace Fuke.Common.Utilities.Collections;

public static partial class EnumerableExtensions
{
    /// <summary>
    /// Indicates whether the collection is empty.
    /// </summary>
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }

    /// <summary>
    /// Indicates whether the collection is not empty.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        return enumerable == null || enumerable.IsEmpty();
    }
}
