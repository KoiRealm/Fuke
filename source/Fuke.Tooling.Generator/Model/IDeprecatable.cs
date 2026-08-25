// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.CodeGeneration.Model;

public interface IDeprecatable
{
    [CanBeNull]
    string DeprecationMessage { get; }

    [CanBeNull]
    IDeprecatable Parent { get; }
}

public static class DeprecatableExtensions
{
    [Pure]
    public static bool IsDeprecated(this IDeprecatable deprecatable)
    {
        return deprecatable.DeprecationMessage != null || deprecatable.Parent != null && deprecatable.Parent.IsDeprecated();
    }

    [Pure]
    [CanBeNull]
    public static string GetDeprecationMessage(this IDeprecatable deprecatable)
    {
        var message = deprecatable.DeprecationMessage;
        if (!string.IsNullOrEmpty(message))
            return message;
        return deprecatable.Parent?.GetDeprecationMessage();
    }
}
