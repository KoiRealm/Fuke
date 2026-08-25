// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Fuke.Common.Tools.AzureKeyVault
{
    /// <summary>Attribute to obtain a key from from the Azure KeyVault defined by <see cref="AzureKeyVaultConfigurationAttribute"/>.</summary>
    [PublicAPI]
    public class AzureKeyVaultKeyAttribute : AzureKeyVaultAttributeBase
    {
        private readonly string _keyName;

        public AzureKeyVaultKeyAttribute(string keyName = null)
        {
            _keyName = keyName;
        }

        protected override object GetValue(AzureKeyVaultConfiguration configuration, MemberInfo member)
        {
            return AzureKeyVaultTasks.GetKeyBundle(configuration, _keyName ?? member.Name);
        }
    }
}
