// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;
using Serilog;
using Serilog.Events;

namespace Fuke.Common.Tooling;

[Serializable]
public class ProcessException : Exception
{
    private static string FormatMessage(IProcess process)
    {
        const string indentation = "   ";

        var messageBuilder = new StringBuilder()
            .AppendLine($"Process '{Path.GetFileName(process.FileName)}' exited with code {process.ExitCode}.")
            .AppendLine($"{indentation}> {process.FileName.DoubleQuoteIfNeeded()} {process.Arguments}")
            .AppendLine($"{indentation}@ {process.WorkingDirectory}");

        var errorOutput = process.Output.Where(x => x.Type == OutputType.Err).ToList();
        if (errorOutput.Count > 0)
        {
            messageBuilder.AppendLine("Error output:");
            errorOutput.ForEach(x => messageBuilder.Append(indentation).AppendLine(x.Text));
        }
        else if (Log.IsEnabled(LogEventLevel.Verbose))
        {
            messageBuilder.AppendLine("Standard output:");
            process.Output.ForEach(x => messageBuilder.Append(indentation).AppendLine(x.Text));
        }

        return messageBuilder.ToString();
    }

    public ProcessException(IProcess process)
        : base(FormatMessage(process))
    {
        Process = process;
        ExitCode = process.ExitCode;
    }

    protected ProcessException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }

    internal IProcess Process { get; }

    public int ExitCode { get; }
}
