// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Fuke.Common;
using Fuke.Common.IO;
using Xunit;

namespace Fuke.Common.Tests;

public abstract class FileSystemDependentTest
{
    public ITestOutputHelper TestOutputHelper { get; }
    public string TestName { get; }
    public AbsolutePath ExecutionDirectory { get; }
    public AbsolutePath TestProjectDirectory { get; }
    public AbsolutePath RootDirectory { get; }
    public AbsolutePath TestTempDirectory { get; }

    protected FileSystemDependentTest(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;

        TestName = TestContext.Current.TestMethod.NotNull("xUnit 未提供当前测试方法上下文").MethodName;

        ExecutionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).NotNull();
        RootDirectory = Constants.TryGetRootDirectoryFrom(EnvironmentInfo.WorkingDirectory);
        TestProjectDirectory = ExecutionDirectory.FindParentOrSelf(x => x.ContainsFile("*.csproj"));
        TestTempDirectory = ExecutionDirectory / "temp" / $"{GetType().Name}.{TestName}";

        TestTempDirectory.CreateOrCleanDirectory();
    }
}
