// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.CI.AzurePipelines;
using Fuke.Common.CI.AzurePipelines.Configuration;
using Fuke.Common.Execution;
using Fuke.Common.Utilities.Collections;
using Fuke.Components;

[AzurePipelines(
    suffix: null,
    AzurePipelinesImage.UbuntuLatest,
    AzurePipelinesImage.WindowsLatest,
    AzurePipelinesImage.MacOsLatest,
    PullRequestsDisabled = true,
    InvokedTargets = new[] { nameof(ITest.Test), nameof(IPack.Pack) },
    NonEntryTargets = new[] { nameof(IRestore.Restore), nameof(DownloadLicenses), nameof(ICompile.Compile), nameof(InstallFonts), nameof(ReleaseImage) },
    ExcludedTargets = new[] { nameof(Clean), nameof(ISignPackages.SignPackages) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" })]
partial class Build
{
    public class AzurePipelinesAttribute : Fuke.Common.CI.AzurePipelines.AzurePipelinesAttribute
    {
        public AzurePipelinesAttribute(
            string suffix,
            AzurePipelinesImage image,
            params AzurePipelinesImage[] images)
            : base(suffix, image, images)
        {
        }

        protected override AzurePipelinesJob GetJob(
            ExecutableTarget executableTarget,
            LookupTable<ExecutableTarget, AzurePipelinesJob> jobs,
            IReadOnlyCollection<ExecutableTarget> relevantTargets,
            AzurePipelinesImage image)
        {
            var job = base.GetJob(executableTarget, jobs, relevantTargets, image);

            var symbol = CustomNames.GetValueOrDefault(job.Name);
            job.DisplayName = (job.Parallel == 0
                ? $"{symbol} {job.DisplayName}"
                : $"{symbol} {job.DisplayName} 🧩").Trim();
            return job;
        }
    }
}
