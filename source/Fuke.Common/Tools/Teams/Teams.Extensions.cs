// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using Newtonsoft.Json;
using System;

namespace Fuke.Common.Tools.Teams;

public partial class TeamsMessage
{
    [JsonProperty("@type")]
    internal string Type => "MessageCard";
    [JsonProperty("@context")]
    internal string Context => "http://schema.org/extensions";
}
