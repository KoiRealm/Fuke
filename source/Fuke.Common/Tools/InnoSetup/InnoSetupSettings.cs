// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Tools.InnoSetup;

public partial class InnoSetupSettings
{
    private static string GetInnoSetupBool(bool? value)
    {
        return value switch
        {
            null => null,
            true => "+",
            false => "-"
        };
    }

    private string GetOutput()
    {
        return GetInnoSetupBool(Output);
    }
}
