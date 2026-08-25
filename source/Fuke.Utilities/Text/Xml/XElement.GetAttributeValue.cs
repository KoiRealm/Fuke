// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Xml.Linq;

namespace Fuke.Common.Utilities;

public static partial class XElementExtensions
{
    public static string GetAttributeValue(this XElement element, string name)
    {
        return element.Attribute(name).NotNull().Value;
    }
}
