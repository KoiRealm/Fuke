// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities.Collections;

[PublicAPI]
public static class LookupExtensions
{
    public static LookupTable<TKey, TValue> ToLookupTable<TKey, TValue>(this ILookup<TKey, TValue> lookup, IEqualityComparer<TKey> comparer)
    {
        return new LookupTable<TKey, TValue>(lookup, comparer);
    }
}
