// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

[PublicAPI]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ExcludeAssemblyFromCodeCoverageAttribute : Attribute
{
}
