// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Tools.AzureKeyVault
{
    public class AzureKeyVaultCertificate
    {
        public byte[] Cer { get; internal set; }
        public byte[] X509Thumbprint { get; internal set; }
        public AzureKeyVaultKey Key { get; internal set; }
        public string Secret { get; internal set; }
    }
}
