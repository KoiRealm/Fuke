// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity.Configuration;

[PublicAPI]
public class TeamCityKeyValueParameter : TeamCityParameter
{
    public TeamCityKeyValueParameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; set; }
    public string Value { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("text(");
        using (writer.Indent())
        {
            writer.WriteLine($"{Key.DoubleQuote()},");
            writer.WriteLine($"{Value.DoubleQuote()},");
            writer.WriteLine($"display = ParameterDisplay.{TeamCityParameterDisplay.Hidden.ToString().ToUpperInvariant()})");
        }
    }
}
