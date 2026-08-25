// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Linq;

namespace Fuke.Common.Tooling;

partial class OptionsExtensions
{
    public static T Apply<T>(this T options, Configure<T> configurator)
        where T : Options, new()
    {
        return configurator(options);
    }

    public static T[] Apply<T>(this T[] options, Configure<T> configurator)
        where T : Options, new()
    {
        return options.Select(x => configurator(x)).ToArray();
    }

    public static T Apply<T, TValue>(this T options, Configure<T, TValue> configurator, TValue value)
        where T : Options, new()
    {
        return configurator(options, value);
    }

    public static T[] Apply<T, TValue>(this T[] options, Configure<T, TValue> configurator, TValue value)
        where T : Options, new()
    {
        return options.Select(x => configurator(x, value)).ToArray();
    }
}
