// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.CI;

public static class BuildServerConfigurationGeneration
{
    public static bool IsActive { get; } = ParameterService.GetParameter<string>(ConfigurationParameterName) != null;

    public const string ConfigurationParameterName = "generate-configuration";
}
