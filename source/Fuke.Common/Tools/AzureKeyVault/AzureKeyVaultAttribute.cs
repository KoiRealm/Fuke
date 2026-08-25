// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Fuke.Common.Tools.AzureKeyVault
{
    /// <summary>Attribute to obtain the KeyVault defined by <see cref="AzureKeyVaultConfigurationAttribute"/> to retrieve multiple items.</summary>
    [PublicAPI]
    public class AzureKeyVaultAttribute : AzureKeyVaultAttributeBase
    {
        protected override object GetValue(AzureKeyVaultConfiguration configuration, MemberInfo member)
        {
            return AzureKeyVaultTasks.LoadVault(configuration);
        }
    }
}
