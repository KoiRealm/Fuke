// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common;
using Fuke.Common.Git;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tools.GitHub;
using Fuke.Common.Utilities;
using Fuke.Utilities.Text.Yaml;
using static Fuke.Common.ControlFlow;
using static Fuke.Common.Tools.Git.GitTasks;

partial class Build
{
    [Parameter] readonly bool UseHttps;

    AbsolutePath GlobalSolution => RootDirectory / "fuke-global.sln";
    AbsolutePath ExternalRepositoriesDirectory => RootDirectory / "external";
    AbsolutePath ExternalRepositoriesFile => ExternalRepositoriesDirectory / "repositories.yml";

    IEnumerable<Fuke.Common.ProjectModel.Solution> ExternalSolutions
        => ExternalRepositories
            .Select(x => ExternalRepositoriesDirectory / x.GetGitHubName())
            .Select(x => x.GlobFiles("*.sln").Single())
            .Select(x => x.ReadSolution());

    IEnumerable<GitRepository> ExternalRepositories
        => ExternalRepositoriesFile.ReadYaml<string[]>().Select(x => GitRepository.FromUrl(x));

    Target CheckoutExternalRepositories => _ => _
        .Executes(() =>
        {
            foreach (var repository in ExternalRepositories)
            {
                var repositoryDirectory = ExternalRepositoriesDirectory / repository.GetGitHubName();
                var origin = UseHttps ? repository.HttpsUrl : repository.SshUrl;

                if (!Directory.Exists(repositoryDirectory))
                    Git($"clone {origin} {repositoryDirectory} --progress");
                else
                {
                    SuppressErrors(() => Git($"remote add origin {origin}", repositoryDirectory));
                    Git($"remote set-url origin {origin}", repositoryDirectory);
                }
            }
        });
}
