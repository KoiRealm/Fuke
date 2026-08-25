// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using FluentAssertions;
using Fuke.Common.Utilities.Collections;
using Xunit;

namespace Fuke.Common.Tests;

public class EnumerableExtensionsTest
{
    [Fact]
    public void SingleOrDefaultOrError_ThrowsExceptionWithMessage()
    {
        var x = new[] { "a", "a" };
        Action a = () => x.SingleOrDefaultOrError("error");
        a.Should().Throw<InvalidOperationException>().WithMessage("error");
    }

    [Theory]
    [InlineData(new[] { "a", "b", "c" }, "a", "a")]
    [InlineData(new[] { "a", "b", "c" }, "d", null)]
    public void SingleOrDefaultOrError(string[] enumerable, string equalsWith, string expectedValue)
    {
        enumerable.SingleOrDefaultOrError(x => x.Equals(equalsWith), "error").Should().Be(expectedValue);
    }
}
