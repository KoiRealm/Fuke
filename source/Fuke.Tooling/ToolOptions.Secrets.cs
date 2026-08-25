// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Fuke.Common.Utilities;

namespace Fuke.Common.Tooling;

partial class ToolOptions
{
    internal partial IEnumerable<string> GetSecrets()
    {
        return (ProcessRedactedSecrets ?? [])
            .Concat(InternalOptions.Properties()
                .Select(x => (Token: x.Value, Property: _allProperties[x.Name]))
                .Select(x => (x.Token, x.Property, Attribute: x.Property.GetCustomAttribute<ArgumentAttribute>()))
                .Where(x => x.Attribute?.Secret ?? false)
                .Select(x =>
                {
                    Assert.True(x.Property.GetMemberType() == typeof(string));
                    return x.Token.Value<string>();
                }));
    }
}
