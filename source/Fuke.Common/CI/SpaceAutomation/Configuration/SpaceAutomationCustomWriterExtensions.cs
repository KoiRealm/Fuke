// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.SpaceAutomation.Configuration;

public static class SpaceAutomationCustomWriterExtensions
{
    public static IDisposable WriteBlock(this CustomFileWriter writer, string text)
    {
        return DelegateDisposable
            .CreateBracket(
                () => writer.WriteLine(string.IsNullOrWhiteSpace(text)
                    ? "{"
                    : $"{text} {{"),
                () => writer.WriteLine("}"))
            .CombineWith(writer.Indent());
    }

    public static void WriteArray(this CustomFileWriter writer, string property, string[] values)
    {
        if (!values?.Any() ?? true)
            return;

        if (values.Length <= 1)
        {
            writer.WriteLine($"{property} = {values.Single().DoubleQuote()}");
            return;
        }

        writer.WriteLine($"{property} = \"\"\"");
        using (writer.Indent())
        {
            foreach (var value in values)
                writer.WriteLine(value);
        }

        writer.WriteLine("\"\"\"");
    }
}
