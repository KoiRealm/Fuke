// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fuke.Common.Utilities;
using static Fuke.Common.Constants;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

internal class UpdateNotificationAttribute : BuildExtensionAttributeBase, IOnBuildCreated, IOnBuildFinished
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        if (Build.IsLocalBuild && ShouldNotify)
        {
            Notify();
            Host.Information(L("Press any key to skip the update and continue ...", "按任意键跳过更新并继续……"));
            Console.ReadKey();
        }
    }

    public void OnBuildFinished()
    {
        if (Build.IsServerBuild && ShouldNotify)
            Notify();
    }

    private bool ShouldNotify => !Directory.Exists(GetFukeDirectory(Build.RootDirectory)) &&
                                 !Build.IsInterceptorExecution;

    private static void Notify()
    {
        Host.Warning(
            new[]
            {
                L("--- Updating from version 5.1.0 or newer is recommended ---", "--- 建议从 5.1.0 版本开始更新 ---"),
                L("1. Update the global tool", "1. 更新全局工具"),
                "   dotnet tool update Fuke.GlobalTool -g",
                L("2. Update the build project", "2. 更新构建项目"),
                "   fuke :update",
                L("3. Confirm updates to configuration files and build scripts", "3. 确认更新配置文件和构建脚本"),
                L("   (Other update items are optional)", "   （其他更新项可选）"),
                string.Empty
            }.JoinNewLine());
    }
}
