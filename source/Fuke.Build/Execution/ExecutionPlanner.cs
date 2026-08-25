// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NuGet.Packaging;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

/// <summary>
/// Given the invoked target names, creates an execution plan under consideration of execution, ordering and trigger dependencies.
/// </summary>
internal static class ExecutionPlanner
{
    public static IReadOnlyCollection<ExecutableTarget> GetExecutionPlan(
        IReadOnlyCollection<ExecutableTarget> executableTargets,
        [CanBeNull] IReadOnlyCollection<string> invokedTargetNames)
    {
        var invokedTargets = invokedTargetNames?.Select(x => GetExecutableTarget(x, executableTargets)).ToList();
        invokedTargets?.ForEach(x => x.Invoked = true);

        // Repeat to create the plan with triggers taken into account until plan doesn't change
        IReadOnlyCollection<ExecutableTarget> executionPlan;
        IReadOnlyCollection<ExecutableTarget> additionallyTriggered;
        do
        {
            executionPlan = GetExecutionPlanInternal(executableTargets, invokedTargets);
            additionallyTriggered = executionPlan
                .SelectMany(x => x.Triggers)
                .Except(executionPlan)
                .Where(executableTargets.Contains).ToList();
            invokedTargets = executionPlan.Concat(additionallyTriggered).ToList();
        } while (additionallyTriggered.Count > 0);

        return executionPlan.ForEachLazy(x => x.Status = ExecutionStatus.Scheduled).ToList();
    }

    private static IReadOnlyCollection<ExecutableTarget> GetExecutionPlanInternal(
        IReadOnlyCollection<ExecutableTarget> executableTargets,
        ICollection<ExecutableTarget> invokedTargets)
    {
        var vertexDictionary = GetVertexDictionary(executableTargets);
        var graphAsList = vertexDictionary.Values.ToList();
        var scheduledTargets = new List<ExecutableTarget>();

        var scc = new StronglyConnectedComponentFinder<ExecutableTarget>();
        var cycles = scc.DetectCycle(graphAsList).Cycles().ToList();
        if (cycles.Count > 0)
        {
            // TODO: logging additional
            Assert.Fail(L("Circular dependencies exist between targets:", "目标之间存在循环依赖：")
                .Concat(cycles.Select(x => $" - {x.Select(y => y.Value.Name).JoinCommaSpace()}"))
                .JoinNewLine());
        }

        while (graphAsList.Any())
        {
            var independents = graphAsList.Where(x => !graphAsList.Any(y => y.Dependencies.Contains(x))).ToList();
            if (ParameterService.GetNamedArgument<bool>("strict") && independents.Count > 1)
            {
                // TODO: logging additional
                Assert.Fail(L("Target ordering is incomplete:", "目标定义顺序不完整：")
                    .Concat(independents.Select(x => $"  - {x.Value.Name}"))
                    .JoinNewLine());
            }

            var independent = independents.First();
            graphAsList.Remove(independent);

            var executableTarget = independent.Value;
            if (!(invokedTargets != null && invokedTargets.Contains(executableTarget)) &&
                !(invokedTargets == null && executableTarget.IsDefault) &&
                !scheduledTargets.SelectMany(x => x.ExecutionDependencies).Contains(executableTarget))
                continue;

            scheduledTargets.Add(executableTarget);
        }

        scheduledTargets.Reverse();

        return scheduledTargets;
    }

    private static IReadOnlyDictionary<ExecutableTarget, Vertex<ExecutableTarget>> GetVertexDictionary(
        IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        var vertexDictionary = executableTargets.ToDictionary(x => x, x => new Vertex<ExecutableTarget>(x));
        foreach (var (executable, vertex) in vertexDictionary)
            vertex.Dependencies.AddRange(executable.AllDependencies.Select(x => vertexDictionary.GetValueOrDefault(x)).WhereNotNull());

        return vertexDictionary;
    }

    private static ExecutableTarget GetExecutableTarget(
        string targetName,
        IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        targetName = targetName.Replace("-", string.Empty);
        var executableTarget = executableTargets.SingleOrDefault(x => x.Name.EqualsOrdinalIgnoreCase(targetName));
        if (executableTarget == null)
        {
            Assert.Fail(L(
                    $"Target {targetName.SingleQuote()} does not exist. Available targets:",
                    $"名为 {targetName.SingleQuote()} 的目标不存在。可用目标：")
                .Concat(executableTargets.Select(x => $"  - {x.Name}").OrderBy(x => x))
                .JoinNewLine());
        }

        return executableTarget;
    }
}
