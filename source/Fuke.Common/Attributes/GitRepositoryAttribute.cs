// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.ValueInjection;

namespace Fuke.Common.Git;

/// <summary>
/// Injects an instance of <see cref="GitRepository"/> based on the local repository.
/// </summary>
[PublicAPI]
[UsedImplicitly(ImplicitUseKindFlags.Default)]
public class GitRepositoryAttribute : ValueInjectionAttributeBase
{
    public override object GetValue(MemberInfo member, object instance)
    {
        return GitRepository.FromLocalDirectory(Build.RootDirectory);
    }
}
