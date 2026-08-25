// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System.Threading;
using JetBrains.Annotations;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using Fuke.Common.IO;
using Fuke.Common.Utilities;

namespace Fuke.Common.ProjectModel;

public static class SolutionModelExtensions
{
    public static Solution ReadSolution([NotNull] this AbsolutePath path)
    {
        return path.ReadSolution<Solution>();
    }

    public static Solution ReadSolution<T>([NotNull] this AbsolutePath path)
        where T : Solution
    {
        var serializer = SolutionSerializers.GetSerializerByMoniker(path).NotNull();
        var model = AsyncHelper.RunSync(() => serializer.OpenAsync(path, CancellationToken.None));
        return typeof(T).CreateInstance<T>(model, path);
    }
}
