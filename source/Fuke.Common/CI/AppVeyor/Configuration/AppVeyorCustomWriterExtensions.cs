// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.AppVeyor.Configuration;

public static class AppVeyorCustomWriterExtensions
{
    public static IDisposable WriteBlock(this CustomFileWriter writer, string text)
    {
        return DelegateDisposable
            .CreateBracket(() => writer.WriteLine(text))
            .CombineWith(writer.Indent());
    }
}
