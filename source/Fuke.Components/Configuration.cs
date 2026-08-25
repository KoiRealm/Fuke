// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Components;

[PublicAPI]
[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    public static Configuration Debug = new() { Value = nameof(Debug) };
    public static Configuration Release = new() { Value = nameof(Release) };

    public static implicit operator string(Configuration configuration)
    {
        return configuration.Value;
    }
}
