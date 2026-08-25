// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.CI.AppVeyor;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AppVeyorSecretAttribute : Attribute
{
    public AppVeyorSecretAttribute(string parameter, string value)
    {
        Parameter = parameter;
        Value = value;
    }

    public string Parameter { get; }
    public string Value { get; }
}
