// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Fuke.Utilities.Text.Yaml;
using Xunit;

namespace Fuke.Common.Tests;

public class SerializationTest
{
    [Fact]
    public void JsonTest()
    {
        var data = CreateData("Json");
        var content = data.ToJson();
        var copy = content.GetJson<Data>();

        copy.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void YamlTest()
    {
        var data = CreateData("Yaml");
        var content = data.ToYaml();
        var copy = content.GetYaml<Data>();

        copy.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void XmlTest()
    {
        var data = CreateData("Xml");
        var content = data.ToXml();
        var copy = content.GetXml<Data>();

        copy.Should().BeEquivalentTo(data);
    }

    private static Data CreateData(string name)
    {
        return new Data
               {
                   String = name,
                   Number = 5,
                   Boolean = true,
                   Nested = new Data
                            {
                                Boolean = false
                            }
               };
    }

    public class Data
    {
        public string String { get; set; }
        public int Number { get; set; }
        public bool Boolean { get; set; }

        public Data Nested { get; set; }
    }
}
