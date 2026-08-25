// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;
using System.Text.RegularExpressions;

namespace Fuke.Common.Utilities;

public static partial class StringExtensions
{
    /// <summary>
    /// Returns the first index of a given regular expression.
    /// </summary>
    [Pure]
    public static int IndexOfRegex(this string text, [RegexPattern] string expression)
    {
        var regex = new Regex(expression, RegexOptions.Compiled);
        return regex.Match(text).Index;
    }
}
