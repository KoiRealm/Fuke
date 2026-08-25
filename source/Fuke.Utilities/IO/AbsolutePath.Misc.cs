// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;

namespace Fuke.Common.IO;

partial class AbsolutePathExtensions
{
    public static bool IsDotDirectory(this AbsolutePath path)
    {
        return path.DirectoryExists() && path.Name.StartsWith(".");
    }
}
