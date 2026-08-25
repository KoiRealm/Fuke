// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Fuke.Common.Tooling;

[PublicAPI]
public struct Output
{
    public OutputType Type;
    public string Text;
}
