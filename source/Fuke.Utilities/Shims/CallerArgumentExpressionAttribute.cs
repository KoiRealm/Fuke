// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

#if NETSTANDARD
using System;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

[PublicAPI]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CallerArgumentExpressionAttribute : Attribute
{
    public CallerArgumentExpressionAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
}
#endif
