// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fuke.Common.Utilities.Collections;

public static partial class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> AddDictionary<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        Dictionary<TKey, TValue> otherDictionary)
    {
        foreach (var (key, value) in otherDictionary)
            dictionary.AddPair(key, value);
        return dictionary;
    }

    public static Dictionary<TKey, TValue> AddDictionary<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        ReadOnlyDictionary<TKey, TValue> otherDictionary)
    {
        foreach (var (key, value) in otherDictionary)
            dictionary.AddPair(key, value);
        return dictionary;
    }
}
