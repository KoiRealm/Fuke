// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Common.Utilities;

public static class CredentialStore
{
    public static void DeletePassword(string name)
    {
        switch (EnvironmentInfo.Platform)
        {
            case PlatformFamily.OSX:
                ProcessTasks.StartProcess(
                    Security,
                    $"delete-generic-password -a {EnvironmentInfo.Variables["LOGNAME"]} -s {name.DoubleQuoteIfNeeded()}",
                    logInvocation: false,
                    logOutput: false).AssertZeroExitCode();
                break;
            default:
                throw new NotSupportedException(EnvironmentInfo.Platform.ToString());
        }
    }

    public static void SavePassword(string name, string password)
    {
        switch (EnvironmentInfo.Platform)
        {
            case PlatformFamily.OSX:
                ProcessTasks.StartProcess(
                    Security,
                    $"add-generic-password -T \"\" -a {EnvironmentInfo.Variables["LOGNAME"]} -s {name.DoubleQuoteIfNeeded()} -w {password}",
                    logInvocation: false,
                    logOutput: false).AssertZeroExitCode();
                break;
            default:
                throw new NotSupportedException(EnvironmentInfo.Platform.ToString());
        }
    }

    [CanBeNull]
    public static string TryGetPassword(string name)
    {
        switch (EnvironmentInfo.Platform)
        {
            case PlatformFamily.OSX:
                var process = ProcessTasks.StartProcess(
                    Security,
                    $"find-generic-password -w -a {EnvironmentInfo.Variables["LOGNAME"]} -s {name.DoubleQuoteIfNeeded()}",
                    logInvocation: false,
                    logOutput: false);
                process.WaitForExit();
                return process.ExitCode == 0
                    ? process.Output.Single().Text
                    : null;
            default:
                return null;
        }
    }

    private static string Security => ToolPathResolver.GetPathExecutable("security");

    public static string GetPassword(string profile, string rootDirectory)
    {
        string PromptForPassword()
        {
            Host.Information($"请输入 {Constants.GetParametersFileName(profile)} 的密码：");
            return ConsoleUtility.ReadSecret();
        }

        var credentialStoreName = Constants.GetCredentialStoreName(rootDirectory, profile);
        var passwordParameterName = Constants.GetProfilePasswordParameterName(profile);
        return TryGetPassword(credentialStoreName) ??
               ParameterService.GetParameter<string>(passwordParameterName) ??
               PromptForPassword();
    }

    public static string CreateNewPassword(out bool generated)
    {
        while (true)
        {
            Host.Information(
                EnvironmentInfo.IsOsx
                    ? "请输入至少 10 个字符的密码（留空将自动生成并保存到 macOS 钥匙串）："
                    : "请输入至少 10 个字符的密码：");

            var password = ConsoleUtility.ReadSecret();
            if (password.IsNullOrEmpty() && EnvironmentInfo.IsOsx)
            {
                generated = true;
                return EncryptionUtility.GetGeneratedPassword();
            }

            if (!password.IsNullOrEmpty() && password.Length >= 10)
            {
                generated = false;
                return password;
            }
        }
    }
}
