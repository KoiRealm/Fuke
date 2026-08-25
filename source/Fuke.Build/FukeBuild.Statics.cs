// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.CI;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.Constants;

namespace Fuke.Common;

public abstract partial class FukeBuild
{
    static FukeBuild()
    {
        RootDirectory = GetRootDirectory();
        TemporaryDirectory = GetTemporaryDirectory(RootDirectory).CreateDirectory();

        BuildAssemblyFile = GetBuildAssemblyFile();
        BuildAssemblyDirectory = BuildAssemblyFile?.Parent;

        BuildProjectFile = GetBuildProjectFile(BuildAssemblyDirectory);
        BuildProjectDirectory = BuildProjectFile?.Parent;

        Verbosity = ParameterService.GetParameter<Verbosity?>(() => Verbosity) ?? Verbosity.Normal;
        Host = ParameterService.GetParameter(() => Host) ?? Host.Default;
        LoadedLocalProfiles = ParameterService.GetParameter(() => LoadedLocalProfiles) ?? new string[0];
    }

    /// <summary>
    /// Gets the full path to the root directory.
    /// </summary>
    [Parameter("构建执行期间的根目录。", Name = RootDirectoryParameterName)]
    public static AbsolutePath RootDirectory { get; }

    /// <summary>
    /// Gets the full path to the temporary directory <c>/.fuke/temp</c>.
    /// </summary>
    public static AbsolutePath TemporaryDirectory { get; }

    /// <summary>
    /// Gets the full path to the build assembly file.
    /// </summary>
    [CanBeNull]
    public static AbsolutePath BuildAssemblyFile { get; }

    /// <summary>
    /// Gets the full path to the build assembly directory.
    /// </summary>
    [CanBeNull]
    public static AbsolutePath BuildAssemblyDirectory { get; }

    /// <summary>
    /// Gets the full path to the build project directory, or <c>null</c>
    /// </summary>
    [CanBeNull]
    public static AbsolutePath BuildProjectDirectory { get; }

    /// <summary>
    /// Gets the full path to the build project file, or <c>null</c>
    /// </summary>
    [CanBeNull]
    public static AbsolutePath BuildProjectFile { get; }

    /// <summary>
    /// Gets the logging verbosity during build execution. Default is <see cref="Fuke.Common.Verbosity.Normal"/>.
    /// </summary>
    [Parameter("构建执行期间的日志详细程度。默认为“Normal”。")]
    public static Verbosity Verbosity
    {
        get => (Verbosity) Logging.Level;
        set => Logging.Level = (LogLevel) value;
    }

    /// <summary>
    /// Gets the host for execution. Default is <em>automatic</em>.
    /// </summary>
    [Parameter("执行主机。默认为“automatic”。")]
    public static Host Host { get; set; }

    [Parameter("指定要加载的配置文件。", Name = LoadedLocalProfilesParameterName)]
    public static string[] LoadedLocalProfiles { get; }

    public static bool IsLocalBuild => !IsServerBuild;
    public static bool IsServerBuild => Host is IBuildServer;

    private static AbsolutePath GetRootDirectory()
    {
        var parameterValue = ParameterService.GetParameter(() => RootDirectory);
        if (parameterValue != null)
            return parameterValue;

        if (ParameterService.GetParameter<bool>(() => RootDirectory))
            return EnvironmentInfo.WorkingDirectory;

        return TryGetRootDirectoryFrom(EnvironmentInfo.WorkingDirectory)
            .NotNull(new[]
                     {
                          $"从“{EnvironmentInfo.WorkingDirectory}”向上查找时未找到“{FukeDirectoryName}”目录/文件。",
                          "请创建用于标记根目录的目录/文件，或在调用时添加“--root [path]”。"
                     }.JoinNewLine());
    }

    [CanBeNull]
    private static AbsolutePath GetBuildAssemblyFile()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null || entryAssembly.GetTypes().All(x => !x.IsSubclassOf(typeof(FukeBuild))))
        {
            var assemblyName = entryAssembly?.GetName().Name;
            Assert.True(assemblyName == null ||
                        assemblyName.StartsWith("ReSharperTestRunner") ||
                        assemblyName == "testhost",
                $"Assembly name was {assemblyName.SingleQuote()}");
            return null;
        }

        var assemblyLocation = entryAssembly.Location;
        var invokedLocation = Environment.GetCommandLineArgs().First();
        Assert.True(assemblyLocation == string.Empty || assemblyLocation == invokedLocation);

        return assemblyLocation != string.Empty ? assemblyLocation : invokedLocation;
    }

    [CanBeNull]
    private static AbsolutePath GetBuildProjectFile([CanBeNull] AbsolutePath buildAssemblyDirectory)
    {
        if (buildAssemblyDirectory == null)
            return null;

        return new DirectoryInfo(buildAssemblyDirectory)
            .DescendantsAndSelf(x => x.Parent)
            .Select(x => x.GetFiles("*.csproj", SearchOption.TopDirectoryOnly)
                .SingleOrDefaultOrError($"在“{x}”中发现了多个项目文件。"))
            .FirstOrDefault(x => x != null)
            ?.FullName;
    }
}
