// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

internal class TargetDefinition : ITargetDefinition
{
    private readonly Stack<PropertyInfo> _baseMembers;

    public TargetDefinition(PropertyInfo target, IFukeBuild build, Stack<PropertyInfo> baseMembers)
    {
        Target = target;
        Build = build;
        _baseMembers = baseMembers;
    }

    public PropertyInfo Target { get; }
    public string Name => Target.GetDisplayShortName();
    public IFukeBuild Build { get; }

    internal Func<bool> Intercept { get; set; }

    internal string Description { get; set; }
    internal List<(string Text, Func<bool> Delegate)> DynamicConditions { get; } = new();
    internal List<(string Text, Func<bool> Delegate)> StaticConditions { get; } = new();
    internal List<LambdaExpression> DelegateRequirements { get; } = new();
    internal List<ToolRequirement> ToolRequirements { get; } = new();
    internal List<Delegate> DependsOnTargets { get; } = new();
    internal List<Delegate> DependentForTargets { get; } = new();
    internal List<Action> Actions { get; } = new();
    internal DependencyBehavior DependencyBehavior { get; private set; }
    internal bool IsProceedAfterFailure { get; private set; }
    internal bool IsAssuredAfterFailure { get; private set; }
    internal bool IsInternal { get; private set; }
    internal List<Delegate> BeforeTargets { get; } = new();
    internal List<Delegate> AfterTargets { get; } = new();
    internal List<Delegate> TriggersTargets { get; } = new();
    internal List<Delegate> TriggeredByTargets { get; } = new();
    internal int? PartitionSize { get; private set; }
    internal List<string> ArtifactProducts { get; } = new();
    internal LookupTable<Target, string[]> ArtifactDependencies { get; } = new();

    ITargetDefinition ITargetDefinition.Description(string description)
    {
        Description = description;
        return this;
    }

    public ITargetDefinition Executes(params Action[] actions)
    {
        Actions.AddRange(actions);
        return this;
    }

    public ITargetDefinition Executes<T>(Func<T> action)
    {
        return Executes(new Action(() => action()));
    }

    public ITargetDefinition Executes(Func<Task> action)
    {
        return Executes(() => action().GetAwaiter().GetResult());
    }

    public ITargetDefinition DependsOn(params Target[] targets)
    {
        DependsOnTargets.AddRange(targets);
        return this;
    }

    public ITargetDefinition DependsOn<T>(params Func<T, Target>[] targets)
    {
        return DependsOn(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition TryDependsOn<T>(params Func<T, Target>[] targets)
    {
        return Build is T ? DependsOn(targets) : this;
    }

    public ITargetDefinition DependentFor(params Target[] targets)
    {
        DependentForTargets.AddRange(targets);
        return this;
    }

    public ITargetDefinition DependentFor<T>(params Func<T, Target>[] targets)
    {
        return DependentFor(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition TryDependentFor<T>(params Func<T, Target>[] targets)
    {
        return Build is T ? DependentFor(targets) : this;
    }

    public ITargetDefinition OnlyWhenDynamic(Func<bool> condition, string conditionExpression = null)
    {
        DynamicConditions.Add((conditionExpression.NotNull().TrimWhitespaces(), condition));
        return this;
    }

    public ITargetDefinition OnlyWhenStatic(Func<bool> condition, string conditionExpression = null)
    {
        StaticConditions.Add((conditionExpression.NotNull().TrimWhitespaces(), condition));
        return this;
    }

    public ITargetDefinition Requires<T>(Expression<Func<T>> parameterRequirement, params Expression<Func<T>>[] parameterRequirements)
        where T : class
    {
        DelegateRequirements.AddRange(parameterRequirement.Concat(parameterRequirements));
        return this;
    }

    public ITargetDefinition Requires<T>(Expression<Func<T?>> parameterRequirement, params Expression<Func<T?>>[] parameterRequirements)
        where T : struct
    {
        DelegateRequirements.AddRange(parameterRequirement.Concat(parameterRequirements));
        return this;
    }

    public ITargetDefinition Requires(Expression<Func<bool>> requirement, params Expression<Func<bool>>[] requirements)
    {
        DelegateRequirements.AddRange(requirement.Concat(requirements));
        return this;
    }

    public ITargetDefinition Requires<T>()
        where T : IRequireTool
    {
        ToolRequirements.Add(typeof(T).GetCustomAttribute<ToolAttribute>().NotNull().GetRequirement());
        return this;
    }

    public ITargetDefinition Requires<T>(string version)
        where T : IRequireToolWithVersion
    {
        ToolRequirements.Add(typeof(T).GetCustomAttribute<ToolAttribute>().NotNull().GetRequirement(version));
        return this;
    }

    public ITargetDefinition Requires(Expression<Func<Tool>> tool, params Expression<Func<Tool>>[] tools)
    {
        var requirements = tool.Concat(tools)
            .Select(x => x.GetMemberInfo())
            .Select(x => x.GetCustomAttribute<ToolInjectionAttributeBase>().NotNull().GetRequirement(x))
            .WhereNotNull();
        ToolRequirements.AddRange(requirements);
        return this;
    }

    public ITargetDefinition WhenSkipped(DependencyBehavior dependencyBehavior)
    {
        DependencyBehavior = dependencyBehavior;
        return this;
    }

    public ITargetDefinition Before(params Target[] targets)
    {
        BeforeTargets.AddRange(targets);
        return this;
    }

    public ITargetDefinition Before<T>(params Func<T, Target>[] targets)
    {
        return Before(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition TryBefore<T>(params Func<T, Target>[] targets)
    {
        return Build is T ? Before(targets) : this;
    }

    public ITargetDefinition After(params Target[] targets)
    {
        AfterTargets.AddRange(targets);
        return this;
    }

    public ITargetDefinition After<T>(params Func<T, Target>[] targets)
    {
        return After(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition TryAfter<T>(params Func<T, Target>[] targets)
    {
        return Build is T ? After(targets) : this;
    }

    public ITargetDefinition Triggers(params Target[] targets)
    {
        TriggersTargets.AddRange(targets);
        return this;
    }

    public ITargetDefinition Triggers<T>(params Func<T, Target>[] targets)
    {
        return Triggers(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition TryTriggers<T>(params Func<T, Target>[] targets)
    {
        return Build is T ? Triggers(targets) : this;
    }

    public ITargetDefinition TriggeredBy(params Target[] targets)
    {
        TriggeredByTargets.AddRange(targets);
        return this;
    }

    public ITargetDefinition TriggeredBy<T>(params Func<T, Target>[] targets)
    {
        return TriggeredBy(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition TryTriggeredBy<T>(params Func<T, Target>[] targets)
    {
        return Build is T ? TriggeredBy(targets) : this;
    }

    public ITargetDefinition AssuredAfterFailure()
    {
        IsAssuredAfterFailure = true;
        return this;
    }

    public ITargetDefinition ProceedAfterFailure()
    {
        IsProceedAfterFailure = true;
        return this;
    }

    public ITargetDefinition Unlisted()
    {
        IsInternal = true;
        return this;
    }

    public ITargetDefinition Base()
    {
        Assert.True(_baseMembers.Count > 0,
            L(
                $"Target '{Target.DeclaringType}.{Target.Name}' has no base members. Use {nameof(Inherit)}<T> to inherit a default interface implementation.",
                $"目标“{Target.DeclaringType}.{Target.Name}”没有任何基成员。若要继承接口默认实现，请改用 {nameof(Inherit)}<T>。"));
        Inherit(_baseMembers.Pop().GetValueNonVirtual<Target>(Build));
        return this;
    }

    public ITargetDefinition Inherit(params Target[] targets)
    {
        targets.ForEach(x => x.Invoke(this));
        return this;
    }

    public ITargetDefinition Inherit<T>(params Expression<Func<T, Target>>[] targets)
    {
        var properties = targets.Length > 0
            ? targets.Select(x => x.GetMemberInfo())
            : new[] { GetSingleTargetProperty<T>(L("shorthand inheritance", "简写继承")) };
        Inherit(properties.Select(x => x.GetValueNonVirtual<Target>(Build)).ToArray());
        return this;
    }

    public ITargetDefinition Produces(params string[] artifacts)
    {
        ArtifactProducts.AddRange(artifacts);
        return this;
    }

    public ITargetDefinition Consumes(params Target[] targets)
    {
        targets.ForEach(x => Consumes(x));
        return this;
    }

    public ITargetDefinition Consumes<T>(params Func<T, Target>[] targets)
    {
        return Consumes(GetTargetsOrShorthand<T>(targets.Select(x => x((T) Build)).ToArray()));
    }

    public ITargetDefinition Consumes(Target target, params string[] artifacts)
    {
        ArtifactDependencies.Add(target, artifacts);
        return this;
    }

    public ITargetDefinition Consumes<T>(Func<T, Target> target, params string[] artifacts)
    {
        return Consumes(target.Invoke((T) Build), artifacts);
    }

    public ITargetDefinition Consumes<T>(params string[] artifacts)
    {
        return Consumes(GetTargetsOrShorthand<T>().Single(), artifacts);
    }

    public ITargetDefinition DependsOnContext<T>()
        where T : IFukeBuild
    {
        Assert.True(Build is T, $"'{Build.GetType().Name}' must implement context '{typeof(T).Name}'");
        var setup = (Setup) GetSingleTargetProperty<T>(L("context dependency (missing setup)", "上下文依赖（缺少 setup）"), targetType: typeof(Setup)).GetValue(Build);
        var cleanup = (Cleanup) GetSingleTargetProperty<T>(L("context dependency (missing cleanup)", "上下文依赖（缺少 cleanup）"), targetType: typeof(Cleanup)).GetValue(Build);
        DependsOnTargets.Add(setup);
        BeforeTargets.Add(cleanup);
        return this;
    }

    public ITargetDefinition Partition(int size)
    {
        Assert.True(size > 1);
        PartitionSize = size;
        return this;
    }

    private Target[] GetTargetsOrShorthand<T>(params Target[] targets)
    {
        return targets.Length > 0
            ? targets
            : new[] { (Target) GetSingleTargetProperty<T>(L("shorthand dependency", "简写依赖")).GetValue(Build) };
    }

    private PropertyInfo GetSingleTargetProperty<T>(string kind, Type targetType = null)
    {
        var interfaceTargets = typeof(T).GetProperties(ReflectionUtility.Instance)
            .Where(x => x.PropertyType == (targetType ?? typeof(Target))).ToList();
        if (interfaceTargets.Count != 1)
        {
            Assert.Fail(L(
                    $"Target '{Target.DeclaringType}.{Target.Name}' cannot use {kind} on component '{typeof(T).Name}'.",
                    $"目标“{Target.DeclaringType}.{Target.Name}”无法在组件“{typeof(T).Name}”上使用{kind}。")
                .Concat(new[]
                {
                    interfaceTargets.Count > 1
                        ? L("Too many related targets:", "相关目标过多：")
                        : L("No related target exists.", "没有相关目标。")
                })
                .Concat(interfaceTargets.Select(x => $"  - {x.Name}")).JoinNewLine());
        }

        return interfaceTargets.Single();
    }
}
