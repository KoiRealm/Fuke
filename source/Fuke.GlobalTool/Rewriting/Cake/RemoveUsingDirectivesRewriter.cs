// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fuke.GlobalTool.Rewriting.Cake;

internal class RemoveUsingDirectivesRewriter : SafeSyntaxRewriter
{
    public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
    {
        return null;
    }
}
