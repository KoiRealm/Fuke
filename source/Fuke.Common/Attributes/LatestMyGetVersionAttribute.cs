// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Fuke.Common.IO;
using Fuke.Common.Utilities;
using Fuke.Common.ValueInjection;

namespace Fuke.Common.Tooling;

[PublicAPI]
public class LatestMyGetVersionAttribute : ValueInjectionAttributeBase
{
    private readonly string _feed;
    private readonly string _package;

    public LatestMyGetVersionAttribute(string feed, string package)
    {
        _feed = feed;
        _package = package;
    }

    public override object GetValue(MemberInfo member, object instance)
    {
        var content = HttpTasks.HttpDownloadString($"https://www.myget.org/RSS/{_feed}");
        return XmlTasks.XmlPeekFromString(content, ".//title")
            // TODO: regex?
            .First(x => x.Contains($"/{_package} "))
            .Split('(').Last()
            .Split(')').First()
            .TrimStart("version ");
    }
}
