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
public class LatestNuGetVersionAttribute : ValueInjectionAttributeBase
{
    private readonly string _packageId;

    public LatestNuGetVersionAttribute(string packageId)
    {
        _packageId = packageId;
    }

    public bool IncludePrerelease { get; set; }
    public bool IncludeUnlisted { get; set; }

    public override object GetValue(MemberInfo member, object instance)
    {
        var version = NuGetVersionResolver.GetLatestVersion(_packageId, IncludePrerelease, IncludeUnlisted).GetAwaiter().GetResult();
        return member.GetMemberType() == typeof(string)
            ? version
            : NuGetVersion.Parse(version);
    }
}
