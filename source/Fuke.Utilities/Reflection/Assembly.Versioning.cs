// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;

namespace Fuke.Common.Utilities;

public static class AssemblyExtensions
{
    public static string GetInformationalText(this Assembly assembly)
    {
        return $"version {assembly.GetVersionText()} ({EnvironmentInfo.Platform},{EnvironmentInfo.Framework})";
    }

    public static string GetVersionText(this Assembly assembly)
    {
        var informationalVersion = assembly.GetAssemblyInformationalVersion();
        var plusIndex = informationalVersion.IndexOf(value: '+');
        return plusIndex == -1 ? "LOCAL" : informationalVersion.Substring(startIndex: 0, length: plusIndex);
    }

    private static string GetAssemblyInformationalVersion(this Assembly assembly)
    {
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().NotNull().InformationalVersion;
    }
}
