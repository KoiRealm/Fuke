// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;

namespace Fuke.Common.Tools.Unity.Logging;

internal class MatchedBlock
{
    public BlockMatcher BlockMatcher { get; }
    public string Name { get; }
    public MatchType MatchType { get; }

    public MatchedBlock(BlockMatcher blockMatcher, string name, MatchType matchType)
    {
        BlockMatcher = blockMatcher;
        Name = name;
        MatchType = matchType;
    }

    public MatchType MatchesEnd(string message)
    {
        return BlockMatcher.MatchesEnd(message);
    }
}
