// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;

namespace Fuke.Common.CI;

public class PartitionAttribute : ParameterAttribute
{
    public PartitionAttribute(int total)
    {
        Total = total;
    }

    public int Total { get; }

    public override bool List => false;

    public override object GetValue(MemberInfo member, object instance)
    {
        var part = ParameterService.GetParameter<int?>(member);
        return part.HasValue
            ? new Partition { Part = part.Value, Total = Total }
            : Partition.Single;
    }
}