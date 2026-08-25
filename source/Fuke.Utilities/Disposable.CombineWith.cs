// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Utilities;

public static class DisposableExtensions
{
    /// <summary>
    /// Combines an existing <see cref="IDisposable"/> with another setup and cleanup delegate.
    /// </summary>
    public static IDisposable CombineWith(this IDisposable disposable, [InstantHandle] Action setup = null, [InstantHandle] Action cleanup = null)
    {
        return DelegateDisposable.CreateBracket(
            setup,
            () =>
            {
                cleanup?.Invoke();
                disposable.Dispose();
            });
    }

    /// <summary>
    /// Combines an existing <see cref="IDisposable"/> with another <see cref="IDisposable"/>.
    /// </summary>
    public static IDisposable CombineWith(this IDisposable disposable, IDisposable otherDisposable)
    {
        return disposable.CombineWith(cleanup: otherDisposable.Dispose);
    }
}
