// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Fuke.Common.Utilities;

namespace Fuke.Common.Tests;

public static class HostInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        FukeBuild.Host = new SilentHost();
    }

    private class SilentHost : Host
    {
        protected internal override IDisposable WriteBlock(string text)
        {
            return DelegateDisposable.CreateBracket();
        }
    }
}
