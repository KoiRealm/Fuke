// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using Fuke.Common.Utilities;
using static Fuke.Common.ToolLocalization;

namespace Fuke.Common;

partial class FukeBuild
{
    internal void WriteLogo()
    {
        if (IsInterceptorExecution)
            return;

        if (GlobalToolSettingsStore.Current.ShowLogo && IsOutputEnabled(DefaultOutput.Logo))
            Host.WriteLogo();

        Host.Information($"{L("FUKE Execution Engine", "FUKE 执行引擎")} {typeof(FukeBuild).Assembly.GetInformationalText()}");
        Host.Information();
    }

    internal IDisposable WriteTarget(string target)
    {
        bool CanCollapse() =>
            Host.GetType().GetMethod(nameof(Host.WriteBlock), ReflectionUtility.Instance | BindingFlags.DeclaredOnly) != null;

        if (IsInterceptorExecution)
            return DelegateDisposable.CreateBracket();

        if (NoLogo)
            return DelegateDisposable.CreateBracket();

        if (IsOutputEnabled(DefaultOutput.TargetHeader) && !CanCollapse() ||
            IsOutputEnabled(DefaultOutput.TargetCollapse) && CanCollapse())
            return Host.WriteBlock(target);

        return DelegateDisposable.CreateBracket();
    }

    internal void WriteErrorsAndWarnings()
    {
        if (IsInterceptorExecution)
            return;

        if (!NoLogo && IsOutputEnabled(DefaultOutput.ErrorsAndWarnings))
            Host.WriteErrorsAndWarnings();
    }

    internal void WriteTargetOutcome()
    {
        if (IsInterceptorExecution)
            return;

        if (IsOutputEnabled(DefaultOutput.TargetOutcome))
            Host.WriteTargetOutcome(this);
    }

    internal void WriteBuildOutcome()
    {
        if (IsInterceptorExecution)
            return;

        if (IsOutputEnabled(DefaultOutput.BuildOutcome))
            Host.WriteBuildOutcome(this);
    }

    private bool IsOutputEnabled(DefaultOutput output)
    {
        return !GetType().GetCustomAttributes<DisableDefaultOutputAttribute>()
            .Where(x => x.IsApplicable(this))
            .Any(x => x.DisabledOutputs.Contains(output));
    }
}
