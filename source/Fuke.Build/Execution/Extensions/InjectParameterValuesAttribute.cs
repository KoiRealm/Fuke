// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.Execution;

namespace Fuke.Common.ValueInjection;

internal class InjectParameterValuesAttribute : BuildExtensionAttributeBase, IOnBuildCreated
{
    public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
    {
        ValueInjectionUtility.InjectValues(Build, (_, attribute) => attribute.GetType() == typeof(ParameterAttribute));
    }
}
