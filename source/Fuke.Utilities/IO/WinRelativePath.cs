// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using static Fuke.Common.IO.PathConstruction;

namespace Fuke.Common.IO;

/// <summary>
/// Represents a relative path with the Windows separator (backward slash).
/// </summary>
[PublicAPI]
[Serializable]
public class WinRelativePath : RelativePath
{
    protected WinRelativePath(string path, char? separator)
        : base(path, separator)
    {
    }

    public static explicit operator WinRelativePath([CanBeNull] string path)
    {
        return new WinRelativePath(NormalizePath(path, WinSeparator), WinSeparator);
    }
}
