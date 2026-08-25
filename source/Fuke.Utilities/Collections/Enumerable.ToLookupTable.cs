// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Fuke.Common.Utilities.Collections;

public static partial class EnumerableExtensions
{
    public static LookupTable<TKey, TValue> ToLookupTable<TItem, TKey, TValue>(
        this IEnumerable<TItem> enumerable,
        Func<TItem, TKey> keySelector,
        Func<TItem, TValue> valueSelector)
    {
        return new LookupTable<TKey, TValue>(enumerable.ToLookup(keySelector, valueSelector));
    }
}
