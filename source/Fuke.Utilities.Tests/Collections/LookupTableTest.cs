// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using FluentAssertions;
using Fuke.Common.Utilities.Collections;
using Xunit;

// ReSharper disable ArgumentsStyleLiteral

namespace Fuke.Common.Tests;

public class LookupTableTest
{
    [Fact]
    public void Test()
    {
        var lookupTable = new LookupTable<string, int>(StringComparer.OrdinalIgnoreCase);

        lookupTable.Should().BeEmpty();
        lookupTable["first"].Should().BeEmpty();

        lookupTable.Add("first", value: 2);
        lookupTable.Add("first", value: 3);
        lookupTable.Add("first", value: 4);
        lookupTable.Add("second", value: 5);
        lookupTable.Should().HaveCount(2);
        lookupTable["first"].Should().HaveCount(3);
        lookupTable["first"].Should().BeEquivalentTo(new[] { 2, 3, 4 });

        lookupTable.Remove("first", value: 3);
        lookupTable["first"].Should().HaveCount(2);
        lookupTable["first"].Should().BeEquivalentTo(new[] { 2, 4 });

        lookupTable.Remove("first");
        lookupTable["first"].Should().BeEmpty();
        lookupTable.Should().HaveCount(1);

        var copy = new LookupTable<string, int>(lookupTable, StringComparer.OrdinalIgnoreCase);
        lookupTable.Add("second", value: 6);
        copy["second"].Should().HaveCount(1);

        lookupTable.Clear();
        lookupTable.Should().BeEmpty();
        copy.Should().NotBeEmpty();
    }
}
