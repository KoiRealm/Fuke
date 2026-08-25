// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities.Net;

namespace Fuke.Common.Tools.Teams;

[PublicAPI]
public static class TeamsTasks
{
    public static void SendTeamsMessage(Configure<TeamsMessage> configurator, string webhook)
    {
        SendTeamsMessageAsync(configurator, webhook).Wait();
    }

    public static async Task SendTeamsMessageAsync(Configure<TeamsMessage> configurator, string webhook)
    {
        var message = configurator(new TeamsMessage());

        using var client = new HttpClient();

        var response = await client.CreateRequest(HttpMethod.Post, webhook)
            .WithJsonContent(message)
            .GetResponseAsync();

        var responseText = await response.GetBodyAsync();
        Assert.True(responseText == "1", $"'{responseText}' == '1'");
    }
}
