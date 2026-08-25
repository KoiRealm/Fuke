// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Serilog;

namespace Fuke.Common.ProjectModel;

/// <summary>
///     Injects an instance of <see cref="Solution"/>. The solution path is resolved in the following order:
///     <ul>
///         <li>From the constructor argument</li>
///         <li>From command-line arguments (e.g., <c>-[MemberName] path/to/solution.sln</c>)</li>
///         <li>From environment variables (e.g., <c>[MemberName]=path/to/solution.sln</c>)</li>
///         <li>From the <c>.fuke</c> configuration file</li>
///     </ul>
/// </summary>
/// <example>
///     <code>
/// [Solution("common.sln")] readonly Solution Solution;
///     </code>
/// </example>
[PublicAPI]
[UsedImplicitly(ImplicitUseKindFlags.Assign)]
public class SolutionAttribute(string relativePath)
    : ParameterAttribute(GetDescription(relativePath))
{
    private static string GetDescription(string relativePath)
    {
        return "要自动加载的解决方案文件路径。"
               + (relativePath != null ? $"默认为 {relativePath}。" : string.Empty);
    }

    public SolutionAttribute()
        : this(relativePath: null)
    {
    }

    public override bool List { get; set; }
    public bool GenerateProjects { get; set; }

    public override object GetValue(MemberInfo member, object instance)
    {
        var solutionFile = TryGetSolutionFileFromFukeFile() ??
                           GetSolutionFileFromParametersFile(member);
        var deserializer = typeof(SolutionModelExtensions).GetMethods()
            .Single(x => x.Name == nameof(SolutionModelExtensions.ReadSolution) && x.ContainsGenericParameters)
            .MakeGenericMethod(member.GetMemberType());
        return ((Solution)deserializer.Invoke(obj: null, [solutionFile])).NotNull();
    }

    // TODO: allow wildcard matching? [Solution("fuke-*.sln")] -- no globbing?
    // TODO: for just [Solution] without parameter being passed, do wildcard search?
    private AbsolutePath GetSolutionFileFromParametersFile(MemberInfo member)
    {
        return relativePath != null
            ? Build.RootDirectory / relativePath
            : ParameterService.GetParameter<AbsolutePath>(member).NotNull($"未给“{member.Name}”定义解决方案文件。");
    }

    private AbsolutePath TryGetSolutionFileFromFukeFile()
    {
        var fukeFile = Build.RootDirectory / Constants.FukeFileName;
        if (!fukeFile.Exists())
            return null;

        var solutionFileRelative = fukeFile.ReadAllLines().ElementAtOrDefault(0);
        Assert.True(solutionFileRelative != null && !solutionFileRelative.Contains(value: '\\'),
            $"{Constants.FukeFileName} 第一行必须使用 UNIX 分隔符指定解决方案路径");

        var solutionFile = Build.RootDirectory / solutionFileRelative;
        Assert.FileExists(solutionFile, $"通过 {Constants.FukeFileName} 指定的解决方案文件“{solutionFile}”不存在");

        return solutionFile;
    }
}
