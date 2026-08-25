// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fuke.Common.Utilities;

public static class ObjectExtensions
{
    public static JObject ToJObject(this object obj, JsonSerializer serializer = null)
    {
        serializer ??= JsonSerializer.CreateDefault();
        return JObject.FromObject(obj, serializer);
    }
}
