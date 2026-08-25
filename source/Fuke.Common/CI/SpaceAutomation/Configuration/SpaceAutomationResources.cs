// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.SpaceAutomation.Configuration;

[PublicAPI]
public class SpaceAutomationResources : ConfigurationEntity
{
    public string Cpu { get; set; }
    public string Memory { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        if (Cpu != null || Memory != null)
        {
            using (writer.WriteBlock($"resources"))
            {
                if (Cpu != null)
                    writer.WriteLine($"cpu = {Cpu}");
                if (Memory != null)
                    writer.WriteLine($"memory = {Memory}");
            }
        }
    }
}
