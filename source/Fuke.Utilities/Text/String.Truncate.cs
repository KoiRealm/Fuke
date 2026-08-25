// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Utilities;

partial class StringExtensions
{
    public static string Truncate(this string str, int maxChars)
    {
        return str.Length <= maxChars ? str : str.Substring(0, maxChars) + "…";
    }
}
