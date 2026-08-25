// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Build.Execution.Extensions;
using Fuke.Common.CI;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;
using Fuke.Common.ValueInjection;
using static Fuke.Common.Constants;

// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Fuke.Common;

/// <summary>
/// Base class for build definitions. Derived types must declare <c>static int Main</c> which calls
/// <see cref="Execute{T}"/> for the exit code.
/// </summary>
/// <example>
/// <code>
/// class DefaultBuild : FukeBuild
/// {
///     public static int Main () => Execute&lt;DefaultBuild&gt;(x => x.Compile);
///
///     Target Clean =&gt; _ =&gt; _
///         .Executes(() =&gt;
///         {
///             EnsureCleanDirectory(OutputDirectory);
///         });
///
///     Target Compile =&gt; _ =&gt; _
///         .DependsOn(Clean)
///         .Executes(() =&gt;
///         {
///             MSBuild(SolutionFile);
///         });
/// }
/// </code>
/// </example>
[PublicAPI]
// Before logo
[ArgumentsFromParametersFile(Priority = 150)]
[HandleReSharperSurrogateArguments(Priority = 150)]
[InjectParameterValues(Priority = 100)]
[HandleShellCompletion(Priority = 75)]
[GenerateBuildServerConfigurations(Priority = 50)]
[InvokeBuildServerConfigurationGeneration(Priority = 45)]
[UnsetVisualStudioEnvironmentVariables]
// [SaveBuildProfile(Priority = 30)]
// [LoadBuildProfiles(Priority = 25)]
// After logo
[HandlePlanRequests(Priority = 10)]
[HandleHelpRequests(Priority = 5)]
[HandleVisualStudioDebugging]
[InjectNonParameterValues(Priority = -100)]
// After finish
[UpdateNotification(Priority = 10)]
[SerializeBuildServerState]
public abstract partial class FukeBuild : IFukeBuild
{
    /// <summary>
    /// Executes the build. The provided expression defines the <em>default</em> target that is invoked,
    /// if no targets have been specified via command-line arguments.
    /// </summary>
    protected static int Execute<T>(params Expression<Func<T, Target>>[] defaultTargetExpressions)
        where T : FukeBuild, new()
    {
        return BuildManager.Execute(defaultTargetExpressions);
    }

    internal IReadOnlyCollection<ExecutableTarget> ExecutableTargets { get; set; }

    public IReadOnlyCollection<ExecutableTarget> ExecutionPlan { get; set; }

    /// <summary>
    /// Gets the list of targets that were invoked.
    /// </summary>
    [Parameter("List of targets to invoke. Defaults to '{default_target}'.",
        DescriptionChinese = "要调用的目标列表。默认为“{default_target}”。",
        Name = InvokedTargetsParameterName,
        Separator = TargetsSeparator)]
    public IReadOnlyCollection<ExecutableTarget> InvokedTargets => ExecutionPlan.Where(x => x.Invoked).ToList();

    /// <summary>
    /// Gets the list of targets that are skipped.
    /// </summary>
    [Parameter("List of targets to skip. An empty list skips all dependencies.",
        DescriptionChinese = "要跳过的目标列表。空列表将跳过所有依赖项。",
        Name = SkippedTargetsParameterName,
        Separator = TargetsSeparator)]
    public IReadOnlyCollection<ExecutableTarget> SkippedTargets => ExecutionPlan.Where(x => x.Status == ExecutionStatus.Skipped).ToList();

    /// <summary>
    /// Gets the list of targets that are scheduled.
    /// </summary>
    public IReadOnlyCollection<ExecutableTarget> ScheduledTargets => ExecutionPlan.Where(x => x.Status == ExecutionStatus.Scheduled).ToList();

    /// <summary>
    /// Gets the list of targets that are running.
    /// </summary>
    public IReadOnlyCollection<ExecutableTarget> RunningTargets => ExecutionPlan.Where(x => x.Status == ExecutionStatus.Running).ToList();

    /// <summary>
    /// Gets the list of targets that were aborted.
    /// </summary>
    public IReadOnlyCollection<ExecutableTarget> AbortedTargets => ExecutionPlan.Where(x => x.Status == ExecutionStatus.Aborted).ToList();

    /// <summary>
    /// Gets the list of targets that have failed.
    /// </summary>
    public IReadOnlyCollection<ExecutableTarget> FailedTargets => ExecutionPlan.Where(x => x.Status == ExecutionStatus.Failed).ToList();

    /// <summary>
    /// Gets the list of targets that have succeeded.
    /// </summary>
    public IReadOnlyCollection<ExecutableTarget> SucceededTargets => ExecutionPlan.Where(x => x.Status == ExecutionStatus.Succeeded).ToList();

    /// <summary>
    /// Gets the list of targets that have been finished (failed or succeeded).
    /// </summary>
    public IReadOnlyCollection<ExecutableTarget> FinishedTargets => FailedTargets.Concat(SucceededTargets).ToList();

    /// <summary>
    /// Gets a value whether to show the execution plan (HTML).
    /// </summary>
    [Parameter("Show the execution plan as HTML.", DescriptionChinese = "显示执行计划（HTML）。")]
    public bool Plan { get; }

    /// <summary>
    /// Gets a value whether to show the help text for this build assembly.
    /// </summary>
    [Parameter("Show help for this build assembly.", DescriptionChinese = "显示此构建程序集的帮助文本。")]
    public bool Help { get; }

    /// <summary>
    /// Gets a value whether to display the FUKE logo.
    /// </summary>
    [Parameter("Suppress the FUKE header and ASCII logo.", DescriptionChinese = "禁止显示 FUKE 标题与 ASCII 标志。")]
    public bool NoLogo { get; set; }

    /// <summary>
    /// Gets a value whether a previous failed build should be continued.
    /// </summary>
    [Parameter("Continue the previous failed build.", DescriptionChinese = "继续上一次失败的构建。")]
    public bool Continue { get; internal set; }

    [Parameter("Partition used on CI.", DescriptionChinese = "CI 上使用的分区。", List = false)]
    public Partition Partition { get; internal set; } = Partition.Single;

    [CanBeNull]
    protected internal virtual string NuGetPackagesConfigFile =>
        BuildProjectDirectory != null
            ? NuGetPackageResolver.GetPackagesConfigFile(BuildProjectDirectory)
            : null;

    [CanBeNull]
    protected internal virtual string NuGetAssetsConfigFile =>
        BuildProjectDirectory != null
            ? BuildProjectDirectory / "obj" / "project.assets.json"
            : null;

    [CanBeNull]
    protected internal virtual AbsolutePath NpmPackageJsonFile =>
        BuildProjectDirectory != null
            ? BuildProjectDirectory / "package.json"
            : null;

    [CanBeNull]
    protected internal virtual string EmbeddedPackagesDirectory =>
        BuildProjectFile == null
            ? Assembly.GetEntryAssembly().NotNull().Location != string.Empty
                ? BuildAssemblyDirectory
                : GlobalFukeDirectory / "packages"
            : null;

    internal IEnumerable<string> TargetNames => ExecutableTargetFactory.GetTargetProperties(GetType()).Select(x => x.GetDisplayShortName());
    internal IEnumerable<string> HostNames => Host.AvailableTypes.Select(x => x.Name);

    public bool IsSucceeding => !IsFailing;

    public bool IsFailing => ExecutionPlan.Any(x => x.Status is
        ExecutionStatus.Failed or
        ExecutionStatus.Aborted or
        ExecutionStatus.NotRun);

    public bool IsFinished => !ScheduledTargets.Concat(RunningTargets).Any();

    /// <summary>
    /// Gets or sets the build exit code.
    /// When set to <value>null</value> (default), <see cref="Execute{T}"/> will return a <em>0</em> exit code on build success; or a <em>-1</em> exit code on build failure.
    /// When set to a non-null value, <see cref="Execute{T}"/> will return the value of <see cref="ExitCode"/>.
    /// </summary>
    public int? ExitCode { get; set; }

    private bool IsInterceptorExecution => Environment.GetEnvironmentVariable(InterceptorEnvironmentKey) == "1";

    public void ReportSummary(Configure<Dictionary<string, string>> configurator = null)
    {
        var target = ExecutionPlan.Single(x => x.Status == ExecutionStatus.Running);
        ReportSummary(target, configurator);
    }

    internal void ReportSummary(ExecutableTarget target, Configure<Dictionary<string, string>> configurator)
    {
        target.SummaryInformation = configurator.InvokeSafe(new Dictionary<string, string>()).ToDictionary(x => x.Key, x => x.Value);
        ExecuteExtension<IOnTargetSummaryUpdated>(x => x.OnTargetSummaryUpdated(this, target));
    }
}
