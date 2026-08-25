// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Fuke.Common.Tooling;

namespace Fuke.Common.CI.AzurePipelines.Configuration;

// https://docs.microsoft.com/en-us/azure/devops/pipelines/release/caching
[PublicAPI]
public class AzurePipelinesCacheStep : AzurePipelinesStep
{
    public AzurePipelinesImage Image { get; set; }
    public string[] KeyFiles { get; set; }
    public string Path { get; set; }

    private string AdjustedPath =>
        Image.GetValue().StartsWithAnyOrdinalIgnoreCase("ubuntu", "macos")
            ? Path.Replace("~", "$(HOME)")
            : Path.Replace("~", "$(USERPROFILE)");

    private string Identifier => Path
        .Replace(".", "/")
        .Replace("~", "/")
        .Replace("/", "-")
        .Trim('-');

    public override void Write(CustomFileWriter writer)
    {
        using (writer.WriteBlock("- task: Cache@2"))
        {
            writer.WriteLine("displayName: " + $"Cache: {Identifier}".SingleQuote());
            using (writer.WriteBlock("inputs:"))
            {
                writer.WriteLine($"key: $(Agent.OS) | {Identifier} | {KeyFiles.JoinCommaSpace()}");
                writer.WriteLine($"restoreKeys: $(Agent.OS) | {Identifier}");
                writer.WriteLine($"path: {AdjustedPath}");
            }
        }
    }
}
