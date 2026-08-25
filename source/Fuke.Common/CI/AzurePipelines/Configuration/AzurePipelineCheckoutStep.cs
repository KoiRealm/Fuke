// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using JetBrains.Annotations;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.AzurePipelines.Configuration;

// https://docs.microsoft.com/en-us/azure/devops/pipelines/repos/pipeline-options-for-git?view=azure-devops&tabs=yaml#checkout-submodules
[PublicAPI]
public class AzurePipelineCheckoutStep : AzurePipelinesStep
{
    public bool? InclueSubmodules { get; set; }
    public bool? IncludeLargeFileStorage { get; set; }
    public int? FetchDepth { get; set; }
    public bool? Clean { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("- checkout: self"))
        {
            if (IncludeLargeFileStorage.HasValue)
            {
                writer.WriteLine($"lfs: {IncludeLargeFileStorage.Value}".ToLower());
            }

            if (InclueSubmodules.HasValue)
            {
                writer.WriteLine($"submodules: {InclueSubmodules.Value}".ToLower());
            }

            if (FetchDepth.HasValue)
            {
                writer.WriteLine($"fetchDepth: {FetchDepth.Value}");
            }

            if (Clean.HasValue)
            {
                writer.WriteLine($"clean: {Clean.Value}".ToLower());
            }
        }
    }
}
