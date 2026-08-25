// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities.Collections;

public static partial class DictionaryExtensions
{
    public static IDictionary<TKey, TValue> SetKeyValue<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        [CanBeNull] TValue value = default)
    {
        dictionary[key] = value;
        return dictionary;
    }
}
