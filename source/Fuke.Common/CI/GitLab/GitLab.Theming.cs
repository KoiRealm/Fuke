// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using Fuke.Common.Execution.Theming;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.GitLab;

public partial class GitLab
{
    internal override IHostTheme Theme => AnsiConsoleHostTheme.Default256AnsiColorTheme;

    protected internal override IDisposable WriteBlock(string text)
    {
        return DelegateDisposable.CreateBracket(
            () => BeginSection(text),
            () => EndSection(text));
    }
}
