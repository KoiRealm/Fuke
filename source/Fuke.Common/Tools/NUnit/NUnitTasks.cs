// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Common.Tools.NUnit;

[PublicAPI]
public class NUnitVerbosityMappingAttribute : VerbosityMappingAttribute
{
    public NUnitVerbosityMappingAttribute()
        : base(typeof(NUnitTraceLevel))
    {
        Quiet = nameof(NUnitTraceLevel.Off);
        Minimal = nameof(NUnitTraceLevel.Warning);
        Normal = nameof(NUnitTraceLevel.Info);
        Verbose = nameof(NUnitTraceLevel.Verbose);
    }
}
