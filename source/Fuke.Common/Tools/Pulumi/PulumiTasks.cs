// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using Fuke.Common.Tooling;
using Serilog.Events;

namespace Fuke.Common.Tools.Pulumi;

[LogLevelPattern(LogEventLevel.Warning, "^warning:")]
partial class PulumiTasks;
