// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.CI;

[AttributeUsage(AttributeTargets.Property)]
public class NoConvertAttribute : Attribute
{
}
