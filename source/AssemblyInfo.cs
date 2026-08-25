// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fuke.Build")]
[assembly: InternalsVisibleTo("Fuke.Build.Shared")]
[assembly: InternalsVisibleTo("Fuke.Build.Tests")]
[assembly: InternalsVisibleTo("Fuke.Common")]
[assembly: InternalsVisibleTo("Fuke.Common.Tests")]
[assembly: InternalsVisibleTo("Fuke.GlobalTool")]
[assembly: InternalsVisibleTo("Fuke.GlobalTool.Tests")]
[assembly: InternalsVisibleTo("Fuke.ProjectModel.Tests")]
[assembly: InternalsVisibleTo("Fuke.SourceGenerators")]
[assembly: InternalsVisibleTo("Fuke.SolutionModel")]
[assembly: InternalsVisibleTo("Fuke.SolutionModel.Tests")]
[assembly: InternalsVisibleTo("Fuke.Tooling")]
[assembly: InternalsVisibleTo("Fuke.Tooling.Tests")]
[assembly: InternalsVisibleTo("Fuke.Utilities.IO.Globbing")]
[assembly: InternalsVisibleTo("Fuke.Utilities.Tests")]

// Extensions
[assembly: InternalsVisibleTo("Fuke.VisualStudio")]
[assembly: InternalsVisibleTo("ReSharper.Fuke")]
[assembly: InternalsVisibleTo("ReSharper.Fuke.Rider")]

// Functions
[assembly: InternalsVisibleTo("Fuke.Remote.Functions")]
[assembly: InternalsVisibleTo("Fuke.Website.Functions")]
