// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Net.Http;

namespace Fuke.Common.Utilities.Net;

public static partial class HttpClientExtensions
{
    /// <summary>
    /// Creates an HTTP request.
    /// </summary>
    public static HttpRequestBuilder CreateRequest(this HttpClient client, HttpMethod method, string relativeUri)
    {
        return new HttpRequestBuilder(client, new HttpRequestMessage(method, relativeUri));
    }

    /// <summary>
    /// Creates an HTTP request.
    /// </summary>
    public static HttpRequestBuilder CreateRequest(this HttpClient client, HttpMethod method, string baseAddress, string relativeUri)
    {
        return new HttpRequestBuilder(client, new HttpRequestMessage(method, new Uri(new Uri(baseAddress), relativeUri)));
    }
}

public class HttpRequestBuilder
{
    public HttpRequestBuilder(HttpClient client, HttpRequestMessage request)
    {
        Client = client;
        Request = request;
    }

    public HttpClient Client { get; }
    public HttpRequestMessage Request { get; }
}
