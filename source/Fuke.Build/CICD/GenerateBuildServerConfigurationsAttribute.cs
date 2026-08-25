// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.Execution;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.CI.BuildServerConfigurationGeneration;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.CI;

public class GenerateBuildServerConfigurationsAttribute
    : BuildServerConfigurationGenerationAttributeBase, IOnBuildCreated
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        var configurationId = ParameterService.GetParameter<string>(ConfigurationParameterName);
        if (configurationId == null)
            return;

        Assert.NotNull(Build.RootDirectory);

        var generator = GetGenerators(Build)
            .Where(x => x.Id == configurationId)
            .SingleOrDefaultOrError(L(
                $"Multiple {nameof(IConfigurationGenerator)} instances with ID '{configurationId}' were found.",
                $"发现多个 ID 同为“{configurationId}”的 {nameof(IConfigurationGenerator)}。"))
            .NotNull("generator != null");

        generator.Generate(executableTargets);

        Environment.Exit(0);
    }
}
