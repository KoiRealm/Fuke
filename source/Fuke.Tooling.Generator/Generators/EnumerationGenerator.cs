// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Text;
using Fuke.CodeGeneration.Model;
using Fuke.CodeGeneration.Writers;

namespace Fuke.CodeGeneration.Generators;

public static class EnumerationGenerator
{
    public static void Run(Enumeration enumeration, ToolWriter toolWriter)
    {
        var values = enumeration.Values.ToArray();
        for (var i = 0; i + 1 < values.Length; i++)
            values[i] += ",";

        string GetIdentifier(string value)
            => value.Aggregate(
                new StringBuilder(!char.IsLetter(value[index: 0]) ? "_" : string.Empty),
                (sb, c) => sb.Append(char.IsLetterOrDigit(c) ? c : '_'),
                sb => sb.ToString());

        toolWriter
            .WriteLine($"#region {enumeration.Name}")
            .WriteSummary(enumeration)
            .WriteLine("[PublicAPI]")
            .WriteLine("[Serializable]")
            .WriteObsoleteAttributeWhenObsolete(enumeration)
            .WriteLine("[ExcludeFromCodeCoverage]")
            .WriteLine($"[TypeConverter(typeof(TypeConverter<{enumeration.Name}>))]")
            .WriteLine($"public partial class {enumeration.Name} : Enumeration")
            .WriteBlock(w => w
                .ForEach(enumeration.Values,
                    x => w.WriteLine(
                        $"public static {enumeration.Name} {GetIdentifier(x).EscapeProperty()} = ({enumeration.Name}) {x.DoubleQuote()};"))
                .WriteLine($"public static implicit operator {enumeration.Name}(string value)")
                .WriteBlock(w2 => w2
                    .WriteLine($"return new {enumeration.Name} {{ Value = value }};")))
            .WriteLine("#endregion");
    }
}
