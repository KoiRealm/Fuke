// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.CI.AzurePipelines;

[PublicAPI]
public enum AzurePipelinesRepositoryType
{
    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/azure/devops/repos/git/overview?view=azure-devops">Azure DevOps Git repository</a>.
    /// </summary>
    AzureRepos,

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/azure/devops/repos/tfvc/overview?view=azure-devops">Team Foundation Version Control</a>.
    /// </summary>
    TfsVersionControl,

    /// <summary>
    /// Git repository hosted on an external server.
    /// </summary>
    Git,

    /// <summary>
    /// Git repository hosted on GitHub.
    /// </summary>
    GitHub,

    /// <summary>
    /// Subversion.
    /// </summary>
    Svn,

    TfsGit
}
