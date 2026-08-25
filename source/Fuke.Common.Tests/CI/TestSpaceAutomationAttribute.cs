// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using Fuke.Common.CI.SpaceAutomation;

namespace Fuke.Common.Tests.CI;

public class TestSpaceAutomationAttribute : SpaceAutomationAttribute, ITestConfigurationGenerator
{
    public TestSpaceAutomationAttribute(string jobName, string image)
        : base(jobName, image)
    {
    }

    public StreamWriter Stream { get; set; }

    protected override StreamWriter CreateStream()
    {
        return Stream;
    }
}
