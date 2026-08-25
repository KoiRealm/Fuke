// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Fuke.CodeGeneration.Model;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class Enumeration : IDeprecatable
{
    [JsonIgnore]
    public Tool Tool { get; set; }

    [NotNull]
    [JsonIgnore]
    public IDeprecatable Parent => Tool;

    [JsonProperty(Required = Required.Always)]
    [RegularExpression(RegexPatterns.Name)]
    [Description("Name of the enumeration.")]
    public string Name { get; set; }

    [JsonProperty(Required = Required.Always)]
    [Description("The enumeration values.")]
    public List<string> Values { get; set; }

    [Description("Obsolete message. Enumeration is marked as obsolete when specified.")]
    public string DeprecationMessage { get; set; }
}
