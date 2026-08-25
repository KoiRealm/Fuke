// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Reflection;

namespace Fuke.Common.Tools.CorFlags;

partial class CorFlagsSettings
{
    string FormatBoolean(bool? value, PropertyInfo property)
        => value switch
        {
            true => "+",
            false => "-",
            null => null
        };
}
