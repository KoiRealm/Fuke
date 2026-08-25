// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities;

[PublicAPI]
[DebuggerNonUserCode]
[DebuggerStepThrough]
public static class Lazy
{
    /// <summary>
    /// Creates a <see cref="Lazy{T}"/> from a delegate.
    /// </summary>
    public static Lazy<T> Create<T>(Func<T> provider)
    {
        return new Lazy<T>(provider);
    }
}
