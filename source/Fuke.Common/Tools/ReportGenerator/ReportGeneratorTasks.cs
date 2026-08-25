// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Common.Tools.ReportGenerator;

[PublicAPI]
public class ReportGeneratorVerbosityMappingAttribute : VerbosityMappingAttribute
{
    public ReportGeneratorVerbosityMappingAttribute()
        : base(typeof(ReportGeneratorVerbosity))
    {
        Quiet = nameof(ReportGeneratorVerbosity.Off);
        Minimal = nameof(ReportGeneratorVerbosity.Warning);
        Normal = nameof(ReportGeneratorVerbosity.Info);
        Verbose = nameof(ReportGeneratorVerbosity.Verbose);
    }
}
