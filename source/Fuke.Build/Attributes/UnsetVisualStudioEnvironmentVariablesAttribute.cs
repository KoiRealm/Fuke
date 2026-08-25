// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.Execution;

[PublicAPI]
public class UnsetVisualStudioEnvironmentVariablesAttribute : BuildExtensionAttributeBase, IOnBuildCreated
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        new[]
        {
            "MSBuildLoadMicrosoftTargetsReadOnly",
            "VisualStudioDir",
            "VisualStudioEdition",
            "VisualStudioVersion",
            "VSAPPIDDIR",
            "VSAPPIDNAME",
            "VSLANG",
            "VSLOGGER_UNIQUEID",
            "VSSKUEDITION"
        }.ForEach(x => Environment.SetEnvironmentVariable(x, value: null));
    }
}
