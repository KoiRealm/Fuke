// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Tooling;
using Serilog.Events;

namespace Fuke.Common.Tools.Docker;

[LogErrorAsStandard]
[LogLevelPattern(LogEventLevel.Warning, "^WARNING!")]
partial class DockerTasks;
