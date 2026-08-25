// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.IO;
using static Fuke.Common.ToolLocalization;

namespace Fuke.GlobalTool;

partial class Program
{
    [UsedImplicitly]
    public static int Settings(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        if (args.SequenceEqual(new[] { "--reset" }, StringComparer.OrdinalIgnoreCase))
        {
            GlobalToolSettingsStore.Reset();
            PrintInfo();
            Host.Information(L("Global settings were reset to their defaults.", "全局设置已重置为默认值。"));
            PrintGlobalSettings();
            return 0;
        }

        var current = GlobalToolSettingsStore.Current;
        var updated = new GlobalToolSettings
        {
            Language = current.Language,
            ShowLogo = current.ShowLogo
        };

        for (var index = 0; index < args.Length; index++)
        {
            var option = args[index];
            switch (option.ToLowerInvariant())
            {
                case "--language":
                    updated.Language = ParseLanguage(GetOptionValue(args, ref index, option));
                    break;
                case "--show-logo":
                    var showLogo = GetOptionValue(args, ref index, option);
                    if (!bool.TryParse(showLogo, out var parsedShowLogo))
                    {
                        throw new ArgumentException(
                            L(
                                $"Option '{option}' only accepts 'true' or 'false', but received '{showLogo}'.",
                                $"选项“{option}”仅接受“true”或“false”，实际收到“{showLogo}”。"));
                    }

                    updated.ShowLogo = parsedShowLogo;
                    break;
                default:
                    throw new ArgumentException(
                        L(
                            $"Unknown global settings option '{option}'.",
                            $"未知的全局设置选项“{option}”。"));
            }
        }

        if (args.Length > 0)
        {
            GlobalToolSettingsStore.Save(updated);
            Host.Information(L("Global settings were saved.", "全局设置已保存。"));
        }

        PrintInfo();
        PrintGlobalSettings();
        return 0;
    }

    private static string GetOptionValue(string[] args, ref int index, string option)
    {
        index++;
        if (index >= args.Length)
        {
            throw new ArgumentException(
                L(
                    $"Option '{option}' requires a value.",
                    $"选项“{option}”需要提供值。"));
        }

        return args[index];
    }

    private static ToolLanguage ParseLanguage(string value)
    {
        return value.ToLowerInvariant() switch
        {
            "english" or "en" or "en-us" => ToolLanguage.English,
            "chinese" or "zh" or "zh-cn" => ToolLanguage.Chinese,
            _ => throw new ArgumentException(
                L(
                    $"Unsupported language '{value}'. Supported values are 'English' and 'Chinese'.",
                    $"不支持语言“{value}”。支持的值为“English”和“Chinese”。"))
        };
    }

    private static void PrintGlobalSettings()
    {
        var settings = GlobalToolSettingsStore.Current;
        var language = settings.Language switch
        {
            ToolLanguage.English => L("English", "英语"),
            ToolLanguage.Chinese => L("Chinese", "中文"),
            var value => throw new NotSupportedException($"Unsupported tool language '{value}'.")
        };
        var logo = settings.ShowLogo
            ? L("Enabled", "已启用")
            : L("Disabled", "已禁用");

        Host.Information(L("Global settings:", "全局设置："));
        Host.Information($"  {L("Language", "显示语言")}: {language}");
        Host.Information($"  {L("FUKE ASCII logo", "FUKE ASCII 标志")}: {logo}");
        Host.Information($"  {L("Settings file", "设置文件")}: {GlobalToolSettingsStore.SettingsFile}");
        Host.Information();
        Host.Information(L("Usage:", "用法："));
        Host.Information("  fuke :settings --language English|Chinese");
        Host.Information("  fuke :settings --show-logo true|false");
        Host.Information("  fuke :settings --reset");
    }
}
