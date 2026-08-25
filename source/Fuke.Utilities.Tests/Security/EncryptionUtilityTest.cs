// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using FluentAssertions;
using Fuke.Common.Utilities;
using Xunit;

namespace Fuke.Common.Tests;

public class EncryptionUtilityTest
{
    private const string Password = "correct horse battery staple";
    private const string ClearText = "FUKE-兼容性测试";
    private const string CipherText = "v1:Ak7ghky98e8wmy2x6vqkOMkIepNiEgH1Z9XQmgYBraM=";

    [Fact]
    public void TestVersion1Compatibility()
    {
        EncryptionUtility.Encrypt(ClearText, Password).Should().Be(CipherText);
        EncryptionUtility.Decrypt(CipherText, Password, "测试密钥").Should().Be(ClearText);
    }
}
