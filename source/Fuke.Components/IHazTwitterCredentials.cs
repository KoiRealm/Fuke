// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;

namespace Fuke.Components;

[PublicAPI]
[ParameterPrefix(Twitter)]
public interface IHazTwitterCredentials : IFukeBuild
{
    public const string Twitter = nameof(Twitter);

    [Parameter] [Secret] string ConsumerKey => TryGetValue(() => ConsumerKey);
    [Parameter] [Secret] string ConsumerSecret => TryGetValue(() => ConsumerSecret);
    [Parameter] [Secret] string AccessToken => TryGetValue(() => AccessToken);
    [Parameter] [Secret] string AccessTokenSecret => TryGetValue(() => AccessTokenSecret);
}