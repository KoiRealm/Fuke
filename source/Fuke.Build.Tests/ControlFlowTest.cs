// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using FluentAssertions;
using Xunit;

// ReSharper disable ArgumentsStyleLiteral

namespace Fuke.Common.Tests;

public class ControlFlowTest
{
    [Fact]
    public void Test()
    {
        var executions = 0;

        void OnSecondExecution()
        {
            executions++;
            if (executions != 2)
                throw new Exception(executions.ToString());
        }

        ControlFlow.ExecuteWithRetry(OnSecondExecution);
        executions.Should().Be(2);
    }
}
