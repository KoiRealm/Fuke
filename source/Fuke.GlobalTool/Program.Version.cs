// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;
using Fuke.Common.IO;

namespace Fuke.GlobalTool;

partial class Program
{
    [UsedImplicitly]
    public static int Version(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        PrintInfo();
        return 0;
    }
}
