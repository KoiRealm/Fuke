// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Fuke.Common.Utilities.Collections;
using Xunit;

namespace Fuke.Common.Tests;

public class DictionaryExtensionsTest
{
    [Fact]
    public static void ToGeneric()
    {
        var sourceDictionary = new Dictionary<string, string> { { "key", "value" }, { "key2", "value2" } };
        IDictionary dict = sourceDictionary;
        dict.ToGeneric<string, string>().Should().Equal(sourceDictionary);
    }
}
