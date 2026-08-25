// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.AzurePipelines.Configuration;

[PublicAPI]
public class AzurePipelinesPublishStep : AzurePipelinesStep
{
    public string ArtifactName { get; set; }
    public string PathToPublish { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("- task: PublishBuildArtifacts@1"))
        {
            writer.WriteLine("displayName: " + $"Publish: {ArtifactName}".SingleQuote());
            using (writer.WriteBlock("inputs:"))
            {
                writer.WriteLine($"artifactName: {ArtifactName}");
                writer.WriteLine($"pathToPublish: {PathToPublish.SingleQuote()}");
            }
        }
    }
}
