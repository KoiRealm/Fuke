// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fuke.Common.Tools.Unity.Logging;

internal class LineMatcher
{
    public string RegexPattern { get; }
    public LogLevel LogLevel { get; }

    public LineMatcher(string regexPattern, LogLevel logLevel)
    {
        RegexPattern = regexPattern;
        LogLevel = logLevel;
    }

    public bool Matches(string message)
    {
        return Regex.IsMatch(message, RegexPattern);
    }
}
