// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;

namespace Fuke.Common.IO;

partial class AbsolutePathExtensions
{
    /// <summary>
    /// Returns the time the file or directory was last written to. For directories, the latest time for the whole content is returned.
    /// </summary>
    public static DateTime GetLastWriteTimeUtc(this AbsolutePath path)
    {
        Assert.True(Directory.Exists(path) || File.Exists(path));

        return path.DirectoryExists()
            ? path.ToDirectoryInfo()
                .GetFileSystemInfos("*", SearchOption.AllDirectories)
                .Max(x => x.LastWriteTimeUtc)
            : File.GetLastWriteTimeUtc(path);
    }
}
