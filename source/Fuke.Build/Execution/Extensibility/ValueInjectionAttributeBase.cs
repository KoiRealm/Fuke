// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Serilog;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.ValueInjection;

[PublicAPI]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[MeansImplicitUse(ImplicitUseKindFlags.Assign)]
public abstract class ValueInjectionAttributeBase : Attribute
{
    public IFukeBuild Build { get; internal set; }

    [CanBeNull]
    public object TryGetValue(MemberInfo member, object instance)
    {
        try
        {
            return GetValue(member, instance);
        }
        catch (Exception exception)
        {
            if (!SuppressWarnings && !member.HasCustomAttribute<OptionalAttribute>())
                Log.Warning(exception.Unwrap(), L("Could not inject a value for {Member}", "无法为 {Member} 注入值"), member.GetDisplayName());

            return null;
        }
    }

    [CanBeNull]
    public abstract object GetValue(MemberInfo member, object instance);

    public virtual int Priority => 0;
    public virtual bool SuppressWarnings => false;

    [CanBeNull]
    protected T GetMemberValue<T>(string memberName, object instance)
    {
        var type = instance.GetType();
        var member = type
            .GetAllMembers(
                x => x.Name == memberName,
                bindingFlags: ReflectionUtility.All,
                allowAmbiguity: true,
                filterQuasiOverridden: true)
            .FirstOrDefault()
            .NotNull(L($"Member '{memberName}' was not found in '{type.Name}'.", $"在“{type.Name}”中找不到成员“{memberName}”"));
        Assert.True(typeof(T).IsAssignableFrom(member.GetMemberType()), L(
            $"Member '{type.Name}.{member.Name}' must be of type '{typeof(T).Name}'.",
            $"成员“{type.Name}.{member.Name}”必须是“{typeof(T).Name}”类型"));
        return member.GetValue<T>(instance);
    }

    [CanBeNull]
    protected T GetMemberValueOrNull<T>([CanBeNull] string memberName, object instance)
    {
        return memberName != null ? GetMemberValue<T>(memberName, instance) : default;
    }
}
