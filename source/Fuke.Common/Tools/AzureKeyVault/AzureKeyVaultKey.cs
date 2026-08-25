// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using Azure.Security.KeyVault.Keys;

namespace Fuke.Common.Tools.AzureKeyVault
{
    public class AzureKeyVaultKey
    {
        public JsonWebKey Key { get; internal set; }
        public string Secret { get; internal set; }
    }
}
