// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Fuke.Common;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;
using Fuke.GlobalTool.Rewriting.Cake;
using static Fuke.Common.Constants;
using static Fuke.Common.EnvironmentInfo;
using static Fuke.Common.ToolLocalization;

namespace Fuke.GlobalTool;

partial class Program
{
    public const string CAKE_FILE_PATTERN = "*.cake";

    [UsedImplicitly]
    public static int CakeConvert(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        PrintInfo();
        Logging.Configure();
        ProjectModelTasks.Initialize();

        Host.Warning(
            new[]
            {
                L(".cake conversion uses syntax rewriting and requires manual review.", ".cake 文件转换通过语法重写实现，结果需要人工检查。"),
                L("Compilation errors may remain after conversion. The following elements are currently supported:", "转换后可能仍有编译错误；当前已支持以下元素："),
                L("  - Target definitions", "  - 目标定义"),
                L("  - Default target", "  - 默认目标"),
                L("  - Parameter declarations", "  - 参数声明"),
                L("  - Absolute paths", "  - 绝对路径"),
                L("  - Glob patterns", "  - Glob 匹配模式"),
                L("  - Tool invocations (dotnet CLI, SignTool)", "  - 工具调用（dotnet CLI、SignTool）"),
                L("  - Add-in and tool references", "  - 插件与工具引用"),
            }.JoinNewLine());

        Host.Debug();
        if (!PromptForConfirmation(L("Continue?", "是否继续？")))
            return 0;
        Host.Debug();

        if (buildScript == null &&
            PromptForConfirmation(L(
                "Create a FUKE project first for more complete conversion results?",
                "是否先创建 FUKE 项目以获得更完整的转换结果？")))
        {
            Setup(args, rootDirectory: null, buildScript: null);
        }

        var buildScriptFile = WorkingDirectory / CurrentBuildScriptName;
        var buildProjectFile = buildScriptFile.Exists()
            ? GetConfiguration(buildScriptFile, evaluate: true)
                .GetValueOrDefault(BUILD_PROJECT_FILE, defaultValue: null)
            : null;

        foreach (var cakeFile in GetCakeFiles())
        {
            var outputFile = cakeFile.Parent / cakeFile.NameWithoutExtension.Capitalize() + ".cs";
            var content = GetCakeConvertedContent(cakeFile.ReadAllText());
            outputFile.WriteAllText(content);
        }

        if (buildProjectFile != null)
        {
            var packages = GetCakeFiles().SelectMany(x => GetCakePackages(x.ReadAllText()));
            foreach (var package in packages)
                AddOrReplacePackage(package.Id, package.Version, package.Type, buildProjectFile);
        }

        return 0;
    }

    [UsedImplicitly]
    public static int CakeClean(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        var cakeFiles = GetCakeFiles().ToList();
        Host.Information(L("The following .cake files were found:", "找到以下 .cake 文件："));
        cakeFiles.ForEach(x => Host.Debug($"  - {x}"));

        if (PromptForConfirmation(L("Delete them?", "是否删除？")))
            cakeFiles.ForEach(x => x.DeleteFile());

        return 0;
    }

    private static IEnumerable<AbsolutePath> GetCakeFiles()
    {
        return (TryGetRootDirectoryFrom(WorkingDirectory) ?? WorkingDirectory).GlobFiles($"**/{CAKE_FILE_PATTERN}");
    }

    internal static string GetCakeConvertedContent(string content)
    {
        var options = new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, SourceCodeKind.Script);
        var syntaxTree = CSharpSyntaxTree.ParseText(content, options);
        return new CSharpSyntaxRewriter[]
               {
                   new RemoveUsingDirectivesRewriter(),
                   new RenameFieldIdentifierRewriter(),
                   new ParameterRewriter(),
                   new AbsolutePathRewriter(),
                   new RegularFieldRewriter(),
                   new TargetDefinitionRewriter(),
                   new InvocationRewriter(),
                   new MemberAccessRewriter(),
                   new IdentifierNameRewriter(),
                   new ToolInvocationRewriter(),
                   new ClassRewriter(),
                   new FormattingRewriter()
               }.Aggregate(syntaxTree.GetRoot(), (root, rewriter) => rewriter.Visit(root.NormalizeWhitespace(elasticTrivia: true)))
            .ToFullString();
    }

    internal static IEnumerable<(string Type, string Id, string Version)> GetCakePackages(string content)
    {
        IEnumerable<(string Type, string Id, string Version)> GetPackages(
            string packageType,
            [RegexPattern] string regexPattern)
        {
            var regex = new Regex(regexPattern);
            foreach (Match match in regex.Matches(content))
            {
                var packageId = match.Groups["packageId"].Value;
                var packageVersion = match.Groups["version"].Value;
                if (packageVersion.IsNullOrEmpty())
                    packageVersion = AsyncHelper.RunSync(() => NuGetVersionResolver.GetLatestVersion(packageId, includePrereleases: false));
                yield return new(packageType, packageId, packageVersion);
            }
        }

        return GetPackages(PACKAGE_TYPE_DOWNLOAD, @"#tool ""nuget:\?package=(?'packageId'[\w\d\.]+)(&version=(?'version'[\w\d\.]+))?S*""")
            .Concat(GetPackages(PACKAGE_TYPE_REFERENCE, @"#addin ""nuget:\?package=(?'packageId'[\w\d\.]+)(&version=(?'version'[\w\d\.]+))?S*"""))
            .Where(x => !x.Id.ContainsOrdinalIgnoreCase("Cake"));
    }
}
