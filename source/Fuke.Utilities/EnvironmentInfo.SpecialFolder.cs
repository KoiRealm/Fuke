// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.IO;
using Fuke.Common.Utilities;

namespace Fuke.Common;

[PublicAPI]
public enum SpecialFolders
{
    ProgramFiles = Environment.SpecialFolder.ProgramFiles,
    ProgramFilesX86 = Environment.SpecialFolder.ProgramFilesX86,
    LocalApplicationData = Environment.SpecialFolder.LocalApplicationData,
    ApplicationData = Environment.SpecialFolder.ApplicationData,
    CommonApplicationData = Environment.SpecialFolder.CommonApplicationData,
    Windows = Environment.SpecialFolder.Windows,
    System = Environment.SpecialFolder.System,
    UserProfile = Environment.SpecialFolder.UserProfile
}

partial class EnvironmentInfo
{
    [CanBeNull]
    public static AbsolutePath SpecialFolder(SpecialFolders folder)
    {
        var path = Environment.GetFolderPath((Environment.SpecialFolder)folder);

        // https://github.com/KoiRealm/Fuke/pull/825#discussion_r848954724
        if (path.IsNullOrEmpty() && folder == SpecialFolders.UserProfile)
            path = Environment.GetEnvironmentVariable("USERPROFILE");

        return path;
    }
}
