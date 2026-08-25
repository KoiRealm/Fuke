// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NuGet.Versioning;
using Fuke.Common.Utilities;
using Fuke.Common.ValueInjection;

namespace Fuke.Common.Tooling;

[PublicAPI]
public class LatestNpmVersionAttribute : ValueInjectionAttributeBase
{
    private readonly string _packageId;

    public LatestNpmVersionAttribute(string packageId)
    {
        _packageId = packageId;
    }

    public override object GetValue(MemberInfo member, object instance)
    {
        var version = NpmVersionResolver.GetLatestVersion(_packageId).GetAwaiter().GetResult();
        return member.GetMemberType() == typeof(string)
            ? version
            : SemanticVersion.Parse(version);
    }
}
