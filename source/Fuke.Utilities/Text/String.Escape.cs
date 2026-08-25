// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities;

public static partial class StringExtensions
{
    [Pure]
    public static string EscapeBraces([CanBeNull] this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        return str.NotNull().Replace("{", "{{").Replace("}", "}}");
    }
}
