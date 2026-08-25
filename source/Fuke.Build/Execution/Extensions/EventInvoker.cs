// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Fuke.Common.Execution;

internal class EventInvoker : BuildExtensionAttributeBase,
    IOnBuildCreated,
    IOnBuildInitialized,
    IOnTargetRunning,
    IOnTargetSkipped,
    IOnTargetSucceeded,
    IOnTargetFailed,
    IOnBuildFinished
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        ((FukeBuild)Build).OnBuildCreated();
    }

    public void OnBuildInitialized(
        IReadOnlyCollection<ExecutableTarget> executableTargets,
        IReadOnlyCollection<ExecutableTarget> executionPlan)
    {
        ((FukeBuild)Build).OnBuildInitialized();
    }

    public void OnTargetRunning(ExecutableTarget target)
    {
        ((FukeBuild)Build).OnTargetRunning(target.Name);
    }

    public void OnTargetSkipped(ExecutableTarget target)
    {
        ((FukeBuild)Build).OnTargetSkipped(target.Name);
    }

    public void OnTargetSucceeded(ExecutableTarget target)
    {
        ((FukeBuild)Build).OnTargetSucceeded(target.Name);
    }

    public void OnTargetFailed(ExecutableTarget target)
    {
        ((FukeBuild)Build).OnTargetFailed(target.Name);
    }

    public void OnBuildFinished()
    {
        ((FukeBuild)Build).OnBuildFinished();
    }
}
