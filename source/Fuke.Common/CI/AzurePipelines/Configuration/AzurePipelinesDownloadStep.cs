// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.AzurePipelines.Configuration;

[PublicAPI]
public class AzurePipelinesDownloadStep : AzurePipelinesStep
{
    public string ArtifactName { get; set; }
    public string DownloadPath { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("- task: DownloadBuildArtifacts@0"))
        {
            // writer.WriteLine("displayName: Download Artifacts");
            using (writer.WriteBlock("inputs:"))
            {
                writer.WriteLine($"artifactName: {ArtifactName}");
                writer.WriteLine($"downloadPath: {DownloadPath.SingleQuote()}");
            }
        }
    }
}
