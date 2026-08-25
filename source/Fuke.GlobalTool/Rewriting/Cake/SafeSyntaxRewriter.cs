// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Fuke.Common;
using static Fuke.Common.ToolLocalization;

namespace Fuke.GlobalTool.Rewriting.Cake;

internal class SafeSyntaxRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode Visit(SyntaxNode node)
    {
        try
        {
            return base.Visit(node);
        }
        catch (Exception)
        {
            Host.Warning(L(
                $"Could not process code fragment '{node.ToFullString().Trim()}'; skipping it ...",
                $"无法处理代码片段“{node.ToFullString().Trim()}”，已跳过……"));
            return node;
        }
    }
}
