// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;

namespace Fuke.Common.Utilities;

public static partial class StringExtensions
{
    [Pure]
    public static string Indent(this string text, int count)
    {
        return ' '.Repeat(count) + text;
    }
}
