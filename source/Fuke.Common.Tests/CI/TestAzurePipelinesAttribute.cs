// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using Fuke.Common.CI.AzurePipelines;

namespace Fuke.Common.Tests.CI;

public class TestAzurePipelinesAttribute : AzurePipelinesAttribute, ITestConfigurationGenerator
{
    public TestAzurePipelinesAttribute(AzurePipelinesImage image, params AzurePipelinesImage[] images)
        : base(image, images)
    {
    }

    public StreamWriter Stream { get; set; }

    protected override StreamWriter CreateStream()
    {
        return Stream;
    }
}
