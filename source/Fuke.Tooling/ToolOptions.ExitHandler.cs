// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;

namespace Fuke.Common.Tooling;

partial class ToolOptions
{
    internal Func<ToolOptions, IProcess, object> ProcessExitHandler { get; set; }
}
