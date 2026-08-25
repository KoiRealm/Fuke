// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.CI.TeamCity;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TeamCityTokenAttribute : Attribute
{
    public TeamCityTokenAttribute(string name, string guid)
    {
        Name = name;
        Guid = guid;
    }

    public string Name { get; }
    public string Guid { get; }
}
