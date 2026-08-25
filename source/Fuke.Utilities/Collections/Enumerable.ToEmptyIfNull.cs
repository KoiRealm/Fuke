// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Collections.Generic;

namespace Fuke.Common.Utilities.Collections;

public static partial class EnumerableExtensions
{
    public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> enumerable)
    {
        return enumerable ?? [];
    }

    public static T[] ToEmptyIfNull<T>(this T[] array)
    {
        return array ?? [];
    }

    public static IList<T> ToEmptyIfNull<T>(this IList<T> list)
    {
        return list ?? [];
    }
}
