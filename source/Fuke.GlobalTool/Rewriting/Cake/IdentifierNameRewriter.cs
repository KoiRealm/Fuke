// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Fuke.Common;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Tools.MSBuild;

namespace Fuke.GlobalTool.Rewriting.Cake;

internal class IdentifierNameRewriter : SafeSyntaxRewriter
{
    private static Dictionary<string, string> Replacements =>
        new()
        {
            ["DotNetCoreVerbosity"] = nameof(DotNetVerbosity),
            ["MSBuildToolVersion"] = nameof(MSBuildToolsVersion),
            ["PlatformTarget"] = nameof(MSBuildTargetPlatform),
            ["IsRunningOnUnix"] = nameof(EnvironmentInfo.IsUnix),
            ["IsRunningOnWindows"] = nameof(EnvironmentInfo.IsWin),
            ["EnvironmentVariable"] = "GetVariable<string>",
        };

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        return Replacements.TryGetValue(node.Identifier.Text, out var replacement)
            ? node.WithIdentifier(SyntaxFactory.Identifier(replacement))
            : node;
    }
}
