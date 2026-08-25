// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI;

public abstract class ConfigurationEntity
{
    public abstract void Write(CustomFileWriter writer);
}
