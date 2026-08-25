// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Runtime.Serialization;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common.Execution;

[Serializable]
internal class TargetExecutionException : Exception
{
    public TargetExecutionException(string targetName, Exception inner)
        : base(L($"Target '{targetName}' threw an exception.", $"目标“{targetName}”抛出了异常。"), inner)
    {
    }

    protected TargetExecutionException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}
