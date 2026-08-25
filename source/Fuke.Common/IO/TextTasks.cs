// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Fuke.Common.IO;

[PublicAPI]
public static class TextTasks
{
    public static UTF8Encoding UTF8NoBom => new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
}
