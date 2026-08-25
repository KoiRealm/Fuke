// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace Fuke.GlobalTool.Tests;

public static class VerifyTestsInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Environment.SetEnvironmentVariable("DiffEngine_Disabled", "true");
        Environment.SetEnvironmentVariable("Verify_DisableClipboard", "true");
        VerifyDiffPlex.Initialize();
    }
}
