// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities.Net;

namespace Fuke.Common.Tools.Discord;

[PublicAPI]
public static class DiscordTasks
{
    public static void SendDiscordMessage(Configure<DiscordMessage> configurator, string webhook)
    {
        SendDiscordMessageAsync(configurator, webhook).Wait();
    }

    public static async Task SendDiscordMessageAsync(Configure<DiscordMessage> configurator, string webhook)
    {
        var message = configurator(new DiscordMessage());

        using var client = new HttpClient();

        var response = await client.CreateRequest(HttpMethod.Post, webhook)
            .WithJsonContent(message)
            .GetResponseAsync();

        response.AssertSuccessfulStatusCode();
    }
}
