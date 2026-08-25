// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;

namespace Fuke.Common.Tools.OctoVersion;

partial class OctoVersionTasks
{
    protected override object GetResult<T>(ToolOptions options, IReadOnlyCollection<Output> output)
    {
        if (options is OctoVersionGetVersionSettings getVersion)
        {
            Assert.FileExists(getVersion.OutputJsonFile);
            try
            {
                var file = (AbsolutePath) getVersion.OutputJsonFile;
                return file.ReadJson<OctoVersionInfo>(new JsonSerializerSettings { ContractResolver = new AllWritableContractResolver() });
            }
            catch (Exception exception)
            {
                throw new Exception($"Cannot parse {nameof(OctoVersion)} output from {getVersion.OutputJsonFile.SingleQuote()}.", exception);
            }
        }

        return null;
    }
}

[PublicAPI]
public record OctoVersionInfo(
    int? Major,
    int? Minor,
    int? Patch,
    string PreReleaseTag,
    string PreReleaseTagWithDash,
    string BuildMetaData,
    string BuildMetadataWithPlus,
    string MajorMinorPatch,
    string NuGetCompatiblePreReleaseWithDash,
    string FullSemVer,
    string InformationalVersion,
    string NuGetVersion);
