// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using Fuke.Common.Utilities;

namespace Fuke.Common.CI.TravisCI;

public partial class TravisCI
{
    protected internal override IDisposable WriteBlock(string text)
    {
        return DelegateDisposable.CreateBracket(
            () => Console.WriteLine($"travis_fold:start:{text}"),
            () => Console.WriteLine($"travis_fold:end:{text}"));
    }
}
