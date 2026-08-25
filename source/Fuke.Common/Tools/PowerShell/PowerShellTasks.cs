// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using Fuke.Common.Tooling;

namespace Fuke.Common.Tools.PowerShell;

partial class PowerShellTasks
{
    protected override string GetToolPath(ToolOptions options = null)
    {
        return ToolPathResolver.GetPathExecutable(EnvironmentInfo.IsWin ? "powershell" : "pwsh");
    }
}
