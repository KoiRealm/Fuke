// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.CodeGeneration.Model;

namespace Fuke.CodeGeneration.Writers;

public class DataClassWriter : IWriterWrapper
{
    public DataClassWriter(DataClass dataClass, ToolWriter writer)
    {
        DataClass = dataClass;
        Writer = writer;
    }

    public DataClass DataClass { get; }
    public IWriter Writer { get; }
}
