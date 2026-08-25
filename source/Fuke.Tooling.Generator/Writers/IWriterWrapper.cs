// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.CodeGeneration.Writers;

public interface IWriterWrapper
{
    IWriter Writer { get; }
}
