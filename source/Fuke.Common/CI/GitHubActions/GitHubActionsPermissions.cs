// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Tooling;

namespace Fuke.Common.CI.GitHubActions;

[PublicAPI]
public enum GitHubActionsPermissions
{
    [EnumValue("actions")] Actions,
    [EnumValue("checks")] Checks,
    [EnumValue("contents")] Contents,
    [EnumValue("deployments")] Deployments,
    [EnumValue("id-token")] IdToken,
    [EnumValue("issues")] Issues,
    [EnumValue("discussions")] Discussions,
    [EnumValue("packages")] Packages,
    [EnumValue("pages")] Pages,
    [EnumValue("pull-requests")] PullRequests,
    [EnumValue("repository-projects")] RepositoryProjects,
    [EnumValue("security-events")] SecurityEvents,
    [EnumValue("statuses")] Statuses,
}
