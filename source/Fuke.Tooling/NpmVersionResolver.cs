// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fuke.Common.Tooling;

[PublicAPI]
public static class NpmVersionResolver
{
    private static readonly HttpClient s_client = new();

    [ItemCanBeNull]
    public static async Task<string> GetLatestVersion(string packageId)
    {
        try
        {
            var url = $"https://registry.npmjs.org/{packageId}";
            var jsonString = await s_client.GetStringAsync(url);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonString);
            return (jsonObject["dist-tags"]?["latest"]).NotNull().Value<string>();
        }
        catch
        {
            return null;
        }
    }
}
