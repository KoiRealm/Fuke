// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using Fuke.Common.CI;

namespace Fuke.Common.Tests.CI;

public interface ITestConfigurationGenerator : IConfigurationGenerator
{
    StreamWriter Stream { set; }
}
