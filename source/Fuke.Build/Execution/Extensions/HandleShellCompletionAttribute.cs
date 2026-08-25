// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.CI;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using static Fuke.Common.Constants;

namespace Fuke.Common.Execution;

internal class HandleShellCompletionAttribute : BuildExtensionAttributeBase, IOnBuildCreated
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        if (BuildServerConfigurationGeneration.IsActive)
            return;

        if (IsLegacy(Build.RootDirectory))
        {
            Host.Error(
                new[]
                {
                    "不再支持旧式 .fuke 配置。",
                    "请运行以下命令，转换为新式 .fuke 目录：",
                    "   fuke :update"
                }.JoinNewLine());
            Environment.Exit(exitCode: -1);
        }
        else if (Build.BuildProjectFile != null)
        {
            var buildSchema = SchemaUtility.GetJsonString(Build);
            var buildSchemaFile = GetBuildSchemaFile(Build.RootDirectory);
            buildSchemaFile.WriteAllText(buildSchema);

            var parametersFile = GetDefaultParametersFile(Build.RootDirectory);
            if (!parametersFile.Exists())
            {
                parametersFile.WriteAllText($$"""
                    {
                      "$schema": "./{{BuildSchemaFileName}}"
                    }
                    """);
            }
        }
        else if (ParameterService.GetPositionalArgument<string>(0) == ":complete")
        {
            var schema = SchemaUtility.GetJsonDocument(Build);
            var profileNames = GetProfileNames(Build.RootDirectory);
            var completionItems = CompletionUtility.GetItemsFromSchema(schema, profileNames);

            var words = EnvironmentInfo.CommandLineArguments.Skip(2).JoinSpace();
            var relevantCompletionItems = CompletionUtility.GetRelevantItems(words, completionItems);
            foreach (var item in relevantCompletionItems)
                Console.WriteLine(item);

            Environment.Exit(exitCode: 0);
        }

        if (ParameterService.GetParameter<bool>(CompletionParameterName))
            Environment.Exit(exitCode: 0);
    }
}
