// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Net;

namespace Fuke.Common.CI.GitHubActions;

public partial class GitHubActions
{
    public async Task CreateComment(int issue, string text)
    {
        await _httpClient.Value
            .CreateRequest(HttpMethod.Post, $"repos/{Repository}/issues/{issue}/comments")
            .WithJsonContent(new { body = text })
            .GetResponseAsync();
    }

    private JObject GetJobDetails(long runId)
    {
        var response = _httpClient.Value
            .CreateRequest(HttpMethod.Get, $"repos/{Repository}/actions/runs/{runId}/jobs")
            .GetResponse()
            .AssertSuccessfulStatusCode();

        return response.GetBodyAsJson().GetAwaiter().GetResult()
            .GetChildren("jobs")
            .Single(x => x.GetPropertyStringValue("name") == Job);
    }

    private long GetJobId()
    {
        return GetJobDetails(RunId).GetPropertyValue<long>("id");
    }
}
