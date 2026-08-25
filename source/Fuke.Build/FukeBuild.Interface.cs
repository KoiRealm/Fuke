// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.ValueInjection;

namespace Fuke.Common;

public abstract partial class FukeBuild
{
    IReadOnlyCollection<ExecutableTarget> IFukeBuild.ExecutableTargets => ExecutableTargets;
    bool IFukeBuild.IsInterceptorExecution => IsInterceptorExecution;
    string[] IFukeBuild.LoadedLocalProfiles => LoadedLocalProfiles;
    bool IFukeBuild.IsOutputEnabled(DefaultOutput output) => IsOutputEnabled(output);

    AbsolutePath IFukeBuild.RootDirectory => RootDirectory;
    AbsolutePath IFukeBuild.TemporaryDirectory => TemporaryDirectory;
    AbsolutePath IFukeBuild.BuildAssemblyFile => BuildAssemblyFile;
    AbsolutePath IFukeBuild.BuildAssemblyDirectory => BuildAssemblyDirectory;
    AbsolutePath IFukeBuild.BuildProjectDirectory => BuildProjectDirectory;
    AbsolutePath IFukeBuild.BuildProjectFile => BuildProjectFile;
    Verbosity IFukeBuild.Verbosity => Verbosity;
    Host IFukeBuild.Host => Host;
    bool IFukeBuild.Plan => Plan;
    bool IFukeBuild.Help => Help;
    bool IFukeBuild.NoLogo => NoLogo;
    bool IFukeBuild.IsLocalBuild => IsLocalBuild;
    bool IFukeBuild.IsServerBuild => IsServerBuild;
    bool IFukeBuild.Continue => Continue;

    T IFukeBuild.TryGetValue<T>(Expression<Func<T>> parameterExpression)
    {
        return ValueInjectionUtility.TryGetValue(parameterExpression);
    }

    T IFukeBuild.TryGetValue<T>(Expression<Func<object>> parameterExpression)
    {
        return ValueInjectionUtility.TryGetValue<T>(parameterExpression);
    }
}
