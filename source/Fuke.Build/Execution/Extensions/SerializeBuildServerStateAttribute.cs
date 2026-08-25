// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Execution;

namespace Fuke.Common.CI;

internal class SerializeBuildServerStateAttribute : BuildServerConfigurationGenerationAttributeBase, IOnBuildFinished
{
    public void OnBuildFinished()
    {
        GetGenerators(Build)
            // TODO: bool IsRunning
            .FirstOrDefault(x => x.HostType == Build.Host.GetType())
            ?.SerializeState();
    }
}