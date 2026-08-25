// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Fuke.Common.ValueInjection;

namespace Fuke.Common.CI;

[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class CIAttribute : ValueInjectionAttributeBase
{
    public override object GetValue(MemberInfo member, object instance)
    {
        // TODO: allow with conversion?
        var memberType = member.GetMemberType();
        var instanceProperty = memberType.GetProperty(nameof(Host.Instance), ReflectionUtility.Static);
        Assert.True(instanceProperty != null, $"类型“{memberType}”不支持通过“{nameof(CIAttribute)}”注入");
        return instanceProperty.GetValue(obj: null);
    }
}
