// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.CI.AppVeyor.Configuration;

[PublicAPI]
public class AppVeyorBranches : ConfigurationEntity
{
    public string[] Only { get; set; }
    public string[] Except { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        if (Only.Length > 0)
        {
            using (writer.WriteBlock("only:"))
            {
                Only.ForEach(x => writer.WriteLine($"- {x}"));
            }
        }

        if (Except.Length > 0)
        {
            using (writer.WriteBlock("except:"))
            {
                Except.ForEach(x => writer.WriteLine($"- {x}"));
            }
        }
    }
}
