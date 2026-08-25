// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using Newtonsoft.Json.Linq;

namespace Fuke.Common.Utilities;

public static partial class JObjectExtensions
{
    public static JEnumerable<T> GetChildren<T>(this JObject jobject, string name)
        where T : JToken
    {
        return jobject.GetPropertyValue<JArray>(name).Children<T>();
    }

    public static JEnumerable<JObject> GetChildren(this JObject jobject, string name)
    {
        return jobject.GetChildren<JObject>(name);
    }
}
