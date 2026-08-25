// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Utilities;

partial class ObjectExtensions
{
    public static TOutput Apply<TInput, TOutput>(this TInput input, Func<TInput, TOutput> transform)
    {
        return transform.Invoke(input);
    }
}
