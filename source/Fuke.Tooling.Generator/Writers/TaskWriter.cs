// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Fuke.CodeGeneration.Model;

namespace Fuke.CodeGeneration.Writers;

public class TaskWriter : IWriterWrapper
{
    public TaskWriter(Task task, ToolWriter toolWriter)
    {
        Task = task;
        Writer = toolWriter;
    }

    public Task Task { get; }
    public IWriter Writer { get; }
}
