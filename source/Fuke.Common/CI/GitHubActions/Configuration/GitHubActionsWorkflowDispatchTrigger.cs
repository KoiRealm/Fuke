// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Linq;
using JetBrains.Annotations;
using Fuke.Common.Utilities;
using Fuke.Common.Utilities.Collections;

namespace Fuke.Common.CI.GitHubActions.Configuration;

[PublicAPI]
public class GitHubActionsWorkflowDispatchTrigger : GitHubActionsDetailedTrigger
{
    public string[] OptionalInputs { get; set; }
    public string[] RequiredInputs { get; set; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("workflow_dispatch:");
        using (writer.Indent())
        {
            writer.WriteLine("inputs:");
            using (writer.Indent())
            {
                void WriteInput(string input, bool required)
                {
                    writer.WriteLine($"{input}:");
                    using (writer.Indent())
                    {
                        writer.WriteLine($"description: {input.SplitCamelHumpsWithKnownWords().JoinSpace().DoubleQuote()}");
                        writer.WriteLine($"required: {required.ToString().ToLowerInvariant()}");
                    }
                }

                OptionalInputs.ForEach(x => WriteInput(x, required: false));
                RequiredInputs.ForEach(x => WriteInput(x, required: true));
            }
        }
    }
}
