// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Common.CI.GitHubActions;

[PublicAPI]
public enum GitHubActionsTrigger
{
    [EnumValue("push")] Push,
    [EnumValue("pull_request")] PullRequest,
    [EnumValue("workflow_dispatch")] WorkflowDispatch
}
