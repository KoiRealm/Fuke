// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

#if NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fuke.Common.Utilities.Collections;

public static partial class DictionaryExtensions
{
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return new ReadOnlyDictionary<TKey, TValue>(dictionary);
    }
}

#endif
