// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.Execution;
using Fuke.Common.IO;

namespace Fuke.Common.CI;

public interface IConfigurationGenerator
{
    string Id { get; }
    string DisplayName { get; }
    string HostName { get; }

    bool AutoGenerate { get; }
    Type HostType { get; }
    IEnumerable<AbsolutePath> GeneratedFiles { get; }

    void Generate(IReadOnlyCollection<ExecutableTarget> executableTargets);
    void SerializeState();
}