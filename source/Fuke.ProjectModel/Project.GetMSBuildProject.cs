// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

namespace Fuke.Common.ProjectModel;

public static partial class ProjectExtensions
{
    /// <summary>
    /// Loads the project through the <a href="https://github.com/dotnet/msbuild">Microsoft Build Engine</a>.
    /// </summary>
    public static Microsoft.Build.Evaluation.Project GetMSBuildProject(
        this Project project,
        string configuration = null,
        string targetFramework = null)
    {
        return ProjectModelTasks.ParseProject(project.Path, configuration, targetFramework);
    }
}
