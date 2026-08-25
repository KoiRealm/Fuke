// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common;

/// <summary>
/// Set of constants shared between libraries and IDE extensions.
/// </summary>
[UsedImplicitly]
internal static class Constants
{
    internal const string FukeFileName = FukeDirectoryName;
    internal const string FukeDirectoryName = ".fuke";
    internal const string FukeCommonPackageId = nameof(Fuke) + "." + nameof(Common);
    internal const string BuildSchemaFileName = "build.schema.json";
    internal const string VisualStudioDebugFileName = $"{VisualStudioDebugParameterName}.log";

    internal const string TargetsSeparator = "+";
    internal const string RootDirectoryParameterName = "Root";
    internal const string InvokedTargetsParameterName = "Target";
    internal const string SkippedTargetsParameterName = "Skip";
    internal const string LoadedLocalProfilesParameterName = "Profile";

    public const string VisualStudioDebugParameterName = "visual-studio-debug";
    internal const string CompletionParameterName = "shell-completion";
    internal const string ParametersFilePrefix = "parameters";
    internal const string DefaultProfileName = "$default";

    internal const string GlobalToolVersionEnvironmentKey = "FUKE_GLOBAL_TOOL_VERSION";
    internal const string GlobalToolStartTimeEnvironmentKey = "FUKE_GLOBAL_TOOL_START_TIME";
    internal const string InterceptorEnvironmentKey = "FUKE_INTERNAL_INTERCEPTOR";

    internal static AbsolutePath GlobalTemporaryDirectory => Path.GetTempPath();
    internal static AbsolutePath GlobalFukeDirectory =>  EnvironmentInfo.SpecialFolder(SpecialFolders.UserProfile) / ".fuke";

    [CanBeNull]
    internal static AbsolutePath TryGetRootDirectoryFrom(AbsolutePath startDirectory, bool includeLegacy = true)
    {
        var rootDirectory = new DirectoryInfo(startDirectory)
            .DescendantsAndSelf(x => x.Parent)
            .FirstOrDefault(x => x.GetDirectories(FukeDirectoryName).Any() ||
                                 includeLegacy && x.GetFiles(FukeFileName).Any())
            ?.FullName;
        return rootDirectory != GlobalFukeDirectory.Parent ? (AbsolutePath) rootDirectory : null;
    }

    internal static bool IsLegacy(AbsolutePath rootDirectory)
    {
        return File.Exists(rootDirectory / FukeFileName);
    }

    internal static AbsolutePath GetFukeDirectory(AbsolutePath rootDirectory)
    {
        return rootDirectory / FukeDirectoryName;
    }

    internal static AbsolutePath GetTemporaryDirectory(AbsolutePath rootDirectory)
    {
        return !IsLegacy(rootDirectory)
            ? GetFukeDirectory(rootDirectory) / "temp"
            : rootDirectory / ".tmp";
    }

    internal static AbsolutePath GetCompletionFile(AbsolutePath rootDirectory)
    {
        var completionFileName = CompletionParameterName + ".yml";
        return File.Exists(rootDirectory / completionFileName)
            ? rootDirectory / completionFileName
            : GetTemporaryDirectory(rootDirectory) / completionFileName;
    }

    internal static AbsolutePath GetBuildAttemptFile(AbsolutePath rootDirectory)
    {
        return GetTemporaryDirectory(rootDirectory) / "build-attempt.log";
    }

    public static AbsolutePath GetVisualStudioDebugFile(AbsolutePath rootDirectory)
    {
        return GetTemporaryDirectory(rootDirectory) / $"{VisualStudioDebugParameterName}.log";
    }

    public static AbsolutePath GetReSharperSurrogateFile(AbsolutePath rootDirectory)
    {
        return GetTemporaryDirectory(rootDirectory) / "resharper-surrogate.log";
    }

    internal static AbsolutePath GetBuildSchemaFile(AbsolutePath rootDirectory)
    {
        return GetFukeDirectory(rootDirectory) / BuildSchemaFileName;
    }

    internal static AbsolutePath GetDefaultParametersFile(AbsolutePath rootDirectory)
    {
        return GetFukeDirectory(rootDirectory) / GetParametersFileName(DefaultProfileName);
    }

    internal static IEnumerable<AbsolutePath> GetParametersProfileFiles(AbsolutePath rootDirectory)
    {
        return new DirectoryInfo(GetFukeDirectory(rootDirectory)).GetFiles($"{ParametersFilePrefix}.*.json", SearchOption.TopDirectoryOnly)
            .Select(x => (AbsolutePath)x.FullName);
    }

    internal static AbsolutePath GetParametersProfileFile(AbsolutePath rootDirectory, string profile)
    {
        return GetFukeDirectory(rootDirectory) / GetParametersFileName(profile);
    }

    internal static string GetParametersFileName(string profile)
    {
        return profile == DefaultProfileName ? $"{ParametersFilePrefix}.json" : $"{ParametersFilePrefix}.{profile}.json";
    }

    public static IEnumerable<string> GetProfileNames(AbsolutePath rootDirectory)
    {
        return GetParametersProfileFiles(rootDirectory)
            .Select(x => x.ToString())
            .Select(Path.GetFileNameWithoutExtension)
            .Select(x => x.TrimStart(ParametersFilePrefix).TrimStart("."));
    }

    internal static string GetCredentialStoreName(AbsolutePath rootDirectory, [CanBeNull] string profile)
    {
        return $"FUKE: {rootDirectory} ({profile ?? DefaultProfileName})";
    }

    internal static string GetProfilePasswordParameterName(string profile)
    {
        return $"PARAMS_{profile.TrimStart(DefaultProfileName).ToUpperInvariant().Replace(".", "_")}_KEY".Replace("_", string.Empty);
    }
}
