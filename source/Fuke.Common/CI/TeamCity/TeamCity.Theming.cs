// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using Fuke.Common.Execution;
using Fuke.Common.Execution.Theming;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TeamCity;

public partial class TeamCity
{
    internal override IHostTheme Theme => AnsiConsoleHostTheme.Default256AnsiColorTheme;
    internal override string OutputTemplate => Logging.StandardOutputTemplate;

    protected internal override IDisposable WriteBlock(string text)
    {
        return DelegateDisposable.CreateBracket(
            () => OpenBlock(text),
            () => CloseBlock(text));
    }

    protected internal override void ReportWarning(string text, string details = null)
    {
        WriteWarning(text);
    }

    protected internal override void ReportError(string text, string details = null)
    {
        WriteError(text, details);
    }

    protected internal override bool FilterMessage(string message)
    {
        if (!message.ContainsOrdinalIgnoreCase("##teamcity["))
            return false;

        Console.WriteLine(message);
        return true;
    }
}
