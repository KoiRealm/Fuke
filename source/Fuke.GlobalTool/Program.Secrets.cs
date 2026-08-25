// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Fuke.Common;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.Constants;
using static Fuke.Common.ToolLocalization;
using static Fuke.Common.Utilities.EncryptionUtility;

namespace Fuke.GlobalTool;

// TODO: unlock prompt
// TODO: environment variable name
// TODO: profile vs. environment
// TODO: fuke :profile <name>
partial class Program
{
    private static string SaveAndExit => L("<save and exit>", "<保存并退出>");
    private static string DiscardAndExit => L("<discard and exit>", "<放弃并退出>");
    private static string DeletePasswordAndExit => L("<delete password and exit>", "<删除密码并退出>");

    // ReSharper disable once CognitiveComplexity
    [UsedImplicitly]
    public static int Secrets(string[] args, [CanBeNull] AbsolutePath rootDirectory, [CanBeNull] AbsolutePath buildScript)
    {
        var secretParameters = CompletionUtility.GetItemsFromSchema(
                GetBuildSchemaFile(rootDirectory.NotNull(L("The root directory was not found.", "未找到根目录"))),
                filter: x => x.Value.TryGetProperty("default", out _))
            .Select(x => x.Key).ToList();
        if (secretParameters.Count == 0)
        {
            Host.Information(L(
                $"No parameters are marked with {nameof(SecretAttribute)}.",
                $"没有标记为 {nameof(SecretAttribute)} 的参数"));
            return 0;
        }

        var profile = args.SingleOrDefault();
        var parametersFile = profile == null
            ? GetDefaultParametersFile(rootDirectory)
            : GetParametersProfileFile(rootDirectory, profile);

        var generatedPassword = false;
        var credentialStoreName = GetCredentialStoreName(rootDirectory, profile);
        var password = CredentialStore.TryGetPassword(credentialStoreName);
        var fromCredentialStore = password != null;
        password ??= CredentialStore.CreateNewPassword(out generatedPassword);
        var existingSecrets = LoadSecrets(secretParameters, password, parametersFile);

        if (EnvironmentInfo.IsOsx && existingSecrets.Count == 0 && !fromCredentialStore)
        {
            if (generatedPassword || PromptForConfirmation(L(
                    $"Save the password to the keychain for '{rootDirectory}'?",
                    $"是否将密码保存到钥匙串？（关联目录：“{rootDirectory}”）")))
                CredentialStore.SavePassword(credentialStoreName, password);
        }

        var options = secretParameters
            .Concat(SaveAndExit, DiscardAndExit)
            .Concat(fromCredentialStore ? DeletePasswordAndExit : null).WhereNotNull().ToList();

        var addedSecrets = new Dictionary<string, string>();
        while (true)
        {
            var choice = PromptForChoice(
                L("Select a secret parameter to enter:", "请选择要输入的密钥参数："),
                options.Select(x => (x, addedSecrets.ContainsKey(x) || existingSecrets.ContainsKey(x) ? $"* {x}" : x)).ToArray());

            if (!choice.EqualsAnyOrdinalIgnoreCase(SaveAndExit, DiscardAndExit, DeletePasswordAndExit))
            {
                addedSecrets[choice] = PromptForSecret(choice);
            }
            else
            {
                if (choice == SaveAndExit)
                    SaveSecrets(addedSecrets, password, parametersFile);

                if (choice == DeletePasswordAndExit)
                    CredentialStore.DeletePassword(credentialStoreName);

                if (addedSecrets.Any())
                    Host.Information(L("Remember to clear the clipboard!", "请记得清空剪贴板！"));

                return 0;
            }
        }
    }

    private static Dictionary<string, string> LoadSecrets(IReadOnlyCollection<string> secretParameters, string password, AbsolutePath parametersFile)
    {
        var jobject = parametersFile.ReadJson();
        return jobject.Properties()
            .Where(x => secretParameters.Contains(x.Name))
            .ToDictionary(x => x.Name, x => Decrypt(x.Value.Value<string>(), password, x.Name));
    }

    private static void SaveSecrets(Dictionary<string, string> secrets, string password, AbsolutePath parametersFile)
    {
        parametersFile.UpdateJson(obj =>
        {
            foreach (var (name, secret) in secrets)
                obj[name] = Encrypt(secret, password);
        });
    }
}
