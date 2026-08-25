// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.IO;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.IO;

partial class AbsolutePathExtensions
{
    /// <summary>
    /// Indicates whether the path ends with an extension.
    /// </summary>
    public static bool HasExtension(this AbsolutePath path, string extension, params string[] alternativeExtensions)
    {
        return path.ToString().EndsWithAnyOrdinalIgnoreCase(extension.Concat(alternativeExtensions));
    }

    /// <summary>
    /// Changes the extension of the path (with or without leading period).
    /// </summary>
    public static AbsolutePath WithExtension(this AbsolutePath path, string extension)
    {
        return path.Parent / Path.ChangeExtension(path.Name, extension);
    }
}
