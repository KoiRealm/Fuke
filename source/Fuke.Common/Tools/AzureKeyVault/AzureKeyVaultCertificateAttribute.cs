// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Fuke.Common.Tools.AzureKeyVault
{
    /// <summary> Attribute to obtain a certificates from from the Azure KeyVault defined by <see cref="AzureKeyVaultConfigurationAttribute"/>.</summary>
    [PublicAPI]
    public class AzureKeyVaultCertificateAttribute : AzureKeyVaultAttributeBase
    {
        private readonly string _certificateName;

        public AzureKeyVaultCertificateAttribute(string certificateName = null)
        {
            _certificateName = certificateName;
        }

        /// <summary>If set to true, the key of the certificate is also obtained.</summary>
        public bool IncludeKey { get; set; } = true;

        /// <summary>If set to true, the secret of the certificate is also obtained.</summary>
        public bool IncludeSecret { get; set; } = true;

        protected override object GetValue(AzureKeyVaultConfiguration configuration, MemberInfo member)
        {
            return AzureKeyVaultTasks.GetCertificateBundle(configuration, _certificateName, IncludeKey, IncludeSecret);
        }
    }
}
