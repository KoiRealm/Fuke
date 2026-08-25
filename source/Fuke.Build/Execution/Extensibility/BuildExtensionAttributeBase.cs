// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Execution;

[PublicAPI]
public interface IBuildExtension
{
    float Priority { get; }
}

[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public abstract class BuildExtensionAttributeBase : Attribute, IBuildExtension
{
    public IFukeBuild Build { get; internal set; }
    public virtual float Priority { get; set; }
}

[PublicAPI]
public interface IOnBuildCreated : IBuildExtension
{
    void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets);
}

[PublicAPI]
public interface IOnBuildInitialized : IBuildExtension
{
    void OnBuildInitialized(
        IReadOnlyCollection<ExecutableTarget> executableTargets,
        IReadOnlyCollection<ExecutableTarget> executionPlan);
}

[PublicAPI]
public interface IOnTargetSummaryUpdated : IBuildExtension
{
    void OnTargetSummaryUpdated(IFukeBuild build, ExecutableTarget target);
}

[PublicAPI]
public interface IOnTargetSkipped : IBuildExtension
{
    void OnTargetSkipped(ExecutableTarget target);
}

[PublicAPI]
public interface IOnTargetRunning : IBuildExtension
{
    void OnTargetRunning(ExecutableTarget target);
}

[PublicAPI]
public interface IOnTargetSucceeded : IBuildExtension
{
    void OnTargetSucceeded(ExecutableTarget target);
}

[PublicAPI]
public interface IOnTargetFailed : IBuildExtension
{
    void OnTargetFailed(ExecutableTarget target);
}

[PublicAPI]
public interface IOnBuildFinished : IBuildExtension
{
    void OnBuildFinished();
}
