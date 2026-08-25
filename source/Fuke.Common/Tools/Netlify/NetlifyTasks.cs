// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities;

namespace Fuke.Common.Tools.Netlify;

partial class NetlifyTasks
{
    protected override object GetResult<T>(ToolOptions options, IReadOnlyCollection<Output> output)
    {
        if (options is NetlifySitesCreateSettings)
        {
            return output.EnsureOnlyStd().Select(x => x.Text)
                .Single(x => x.Contains("Site ID:"))
                .SplitSpace().Last();
        }

        return null;
    }
}
