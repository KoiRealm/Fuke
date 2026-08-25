// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities;

public static partial class StringExtensions
{
    /// <summary>
    /// Prepends a string to another string.
    /// </summary>
    [Pure]
    public static string Prepend(this string str, string prependText)
    {
        return prependText + str;
    }

    /// <summary>
    /// Appends a string to another string.
    /// </summary>
    [Pure]
    public static string Append(this string str, string appendText)
    {
        return str + appendText;
    }
}
