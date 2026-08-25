// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fuke.Common.Utilities;
using Fuke.Common.ValueInjection;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

internal class HandleHelpRequestsAttribute : BuildExtensionAttributeBase, IOnBuildInitialized
{
    public void OnBuildInitialized(
        IReadOnlyCollection<ExecutableTarget> executableTargets,
        IReadOnlyCollection<ExecutableTarget> executionPlan)
    {
        if (Build.Help || executionPlan.Count == 0)
        {
            Host.Debug(GetTargetsText());
            Host.Debug(GetParametersText());
            Environment.Exit(exitCode: 0);
        }
    }

    public string GetTargetsText()
    {
        var builder = new StringBuilder();

        var longestTargetName = Build.ExecutableTargets.Select(x => x.Name.Length).OrderByDescending(x => x).First();
        var padRightTargets = Math.Max(longestTargetName, val2: 20);
        builder.AppendLine(L("Targets (with their direct dependencies):", "目标（及其直接依赖）："));
        builder.AppendLine();
        foreach (var target in Build.ExecutableTargets.Where(x => x.Listed))
        {
            var dependencies = target.ExecutionDependencies.Count > 0
                ? $" -> {target.ExecutionDependencies.Select(x => x.Name).JoinCommaSpace()}"
                : string.Empty;
            var targetEntry = target.Name + (target.IsDefault ? L(" (default)", "（默认）") : string.Empty);
            builder.AppendLine($"  {targetEntry.PadRight(padRightTargets)}{dependencies}");
            if (!string.IsNullOrWhiteSpace(target.Description))
                builder.AppendLine($"    {target.Description}");
        }

        return builder.ToString();
    }

    public string GetParametersText()
    {
        var defaultTargets = Build.ExecutableTargets.Where(x => x.IsDefault).Select(x => x.Name).ToList();
        var builder = new StringBuilder();
        var outputWidth = Console.IsOutputRedirected ? 90 : Console.BufferWidth;

        var parameters = ValueInjectionUtility.GetParameterMembers(Build.GetType(), includeUnlisted: false);
        var padRightParameter = Math.Max(parameters.Max(x => ParameterService.GetParameterDashedName(x).Length), val2: 16);

        List<string> SplitLines(string text)
        {
            var words = new Queue<string>(text.Split(' ').ToList());
            var lines = new List<string> { string.Empty };
            foreach (var word in words)
            {
                var nextLength = padRightParameter + 6 + lines.Last().Length + word.Length;
                if (nextLength >= outputWidth || nextLength > 90)
                    lines.Add(string.Empty);

                lines[lines.Count - 1] = $"{lines.Last()} {word}";
            }

            return lines;
        }

        void PrintParameter(MemberInfo parameter)
        {
            var description = SplitLines(
                // TODO: remove
                ParameterService.GetParameterDescription(parameter)
                    ?.Replace("{default_target}", defaultTargets.Count > 0 ? defaultTargets.JoinCommaSpace() : L("<none>", "<无>"))
                    .TrimEnd(".").TrimEnd("。").Append(L(".", "。"))
                ?? L("<no description>", "<无说明>"));
            var parameterName = ParameterService.GetParameterDashedName(parameter);
            builder.AppendLine($"  --{parameterName.PadRight(padRightParameter)}  {description.First()}");
            foreach (var line in description.Skip(count: 1))
                builder.AppendLine($"{' '.Repeat(padRightParameter + 6)}{line}");
        }

        builder.AppendLine(L("Parameters:", "参数："));

        var customParameters = parameters.Where(x => x.DeclaringType != typeof(FukeBuild)).ToList();
        if (customParameters.Count > 0)
            builder.AppendLine();
        customParameters.ForEach(PrintParameter);

        builder.AppendLine();

        var inheritedParameters = parameters.Where(x => x.DeclaringType == typeof(FukeBuild)).ToList();
        inheritedParameters.ForEach(PrintParameter);

        return builder.ToString();
    }
}
