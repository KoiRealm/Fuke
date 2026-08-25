// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.IO;
using JetBrains.Annotations;

namespace Fuke.Common.IO;

partial class AbsolutePathExtensions
{
    /// <summary>
    /// Creates the correlating <see cref="FileInfo"/>.
    /// </summary>
    [Pure]
    [ContractAnnotation("null => null; => notnull")]
    public static FileInfo ToFileInfo(this AbsolutePath path)
    {
        return path is not null ? new FileInfo(path) : null;
    }

    /// <summary>
    /// Creates the correlating <see cref="DirectoryInfo"/>.
    /// </summary>
    [Pure]
    [ContractAnnotation("null => null; => notnull")]
    public static DirectoryInfo ToDirectoryInfo(this AbsolutePath path)
    {
        return path is not null ? new DirectoryInfo(path) : null;
    }
}
