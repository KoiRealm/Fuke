// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Fuke.Common.Tests;

public class EnvironmentInfoTest
{
    [Fact]
    public void TestPaths()
    {
        var paths = EnvironmentInfo.Paths;
        var separator = EnvironmentInfo.IsWin ? ';' : ':';
        var expected = Environment.GetEnvironmentVariable("PATH")
            .NotNull()
            .Split(separator)
            .Select(EnvironmentInfo.ExpandVariables);

        paths.Should().Equal(expected);
    }
}
