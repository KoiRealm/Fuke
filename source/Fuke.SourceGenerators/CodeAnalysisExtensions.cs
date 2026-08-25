// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Fuke.Common.Utilities.Collections;

namespace Fuke.SourceGenerators;

public static class CodeAnalysisExtensions
{
    public static IEnumerable<INamespaceSymbol> GetAllNamespaces(this INamespaceSymbol namespaceSymbol)
    {
        return namespaceSymbol.DescendantsAndSelf(x => x.GetNamespaceMembers());
    }

    public static IEnumerable<ITypeSymbol> GetAllTypes(this INamespaceSymbol namespaceSymbol)
    {
        return namespaceSymbol
            .GetAllNamespaces()
            .SelectMany(x => x.GetTypeMembers())
            .SelectMany(x => x.DescendantsAndSelf(x => x.GetTypeMembers()));
    }

    public static string GetFullName(this INamespaceOrTypeSymbol namespaceOrTypeSymbol)
    {
        return namespaceOrTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public static AttributeData GetAttributeData(this ISymbol symbol, string attributeFullName)
    {
        return symbol
            .GetAttributes()
            .SingleOrDefault(x => x.AttributeClass.GetFullName() == attributeFullName);
    }
}
