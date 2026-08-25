// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Fuke.Common.Tools.MSBuild;
using Fuke.Common.Tools.OpenCover;
using Fuke.Common.Tools.Xunit;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.CorFlags;
using Fuke.Common.Tools.Discord;
using Fuke.Common.Tools.Docker;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Tools.Kubernetes;
using Fuke.Common.Utilities;
using VerifyXunit;
using Xunit;

namespace Fuke.Common.Tests;

public class SettingsTest
{
    private static AbsolutePath RootDirectory => Constants.TryGetRootDirectoryFrom(Directory.GetCurrentDirectory()).NotNull();

    [Fact]
    public void TestCommon()
    {
        var settings = new DotNetRunSettings()
            .SetProcessToolPath("/path/to/dotnet")
            .SetProcessEnvironmentVariable("key", "value")
            .SetProcessExecutionTimeout(TimeSpan.FromMilliseconds(1_000));

        settings.ProcessToolPath.Should().Be("/path/to/dotnet");
        settings.ProcessEnvironmentVariables.Should().ContainSingle(x => x.Key == "key" && x.Value == "value");
        settings.ProcessExecutionTimeout.Should().Be(1_000);
    }

    [Fact]
    public void TestMSBuild()
    {
        var projectFile = RootDirectory / "source" / "Fuke.Common" / "Fuke.Common.csproj";
        var solutionFile = RootDirectory / "fuke-common.sln";

        Assert(new MSBuildSettings()
                .SetProjectFile(projectFile)
                .SetTargetPlatform(MSBuildTargetPlatform.MSIL)
                .SetConfiguration("Release")
                .DisableNodeReuse()
                .EnableNoLogo(),
            $"{projectFile.ToString().DoubleQuoteIfNeeded()} /p:Platform=AnyCPU /p:Configuration=Release /nodeReuse:false /nologo");

        Assert(new MSBuildSettings()
                .SetProjectFile(solutionFile)
                .SetTargetPlatform(MSBuildTargetPlatform.MSIL)
                .EnableNodeReuse()
                .DisableNoLogo()
                .ToggleRunCodeAnalysis(),
            $"{solutionFile.ToString().DoubleQuoteIfNeeded()} /p:Platform=\"Any CPU\" /nodeReuse:true /p:RunCodeAnalysis=true");
    }

    [Fact]
    public void TestXunit2()
    {
        Assert(new Xunit2Settings()
                .AddTargetAssemblies("A.csproj")
                .AddTargetAssemblyWithConfigs("B.csproj", "D.config", "new folder\\E.config"),
            "A.csproj  B.csproj D.config B.csproj \"new folder\\E.config\"");

        Assert(new Xunit2Settings()
                .AddResultReport(Xunit2ResultFormat.HTML, "new folder\\data.html")
                .AddResultReport(Xunit2ResultFormat.Xml, "new_folder\\data.xml"),
            "-HTML \"new folder\\data.html\" -Xml new_folder\\data.xml");

        Assert(new Xunit2Settings()
                .EnableFailSkips()
                .EnableDiagnostics()
                .AddResultReport(Xunit2ResultFormat.NUnit, "new folder\\nunit.xml"),
            "-failskips -diagnostics -NUnit \"new folder\\nunit.xml\"");
    }

    [Fact]
    public void TestOpenCover()
    {
        var projectFile = RootDirectory / "source" / "Fuke.Common" / "Fuke.Common.csproj";

        Assert(new OpenCoverSettings()
                .SetTargetPath(projectFile)
                .SetTargetArguments("-diagnostics -HTML \"new folder\\data.xml\"")
                .AddFilters("+[*]*", "-[xunit.*]*", "-[NUnit.*]*"),
            $"-target:{projectFile.ToString().DoubleQuoteIfNeeded()} -targetargs:\"-diagnostics -HTML \\\"new folder\\data.xml\\\"\" -filter:\"+[*]* -[xunit.*]* -[NUnit.*]*\"");
    }

    [Fact]
    public void TestCorFlags()
    {
        Assert(new CorFlagsSettings()
                .SetAssembly("assembly")
                .EnablePrefer32Bit()
                .DisableILOnly(),
            "assembly -32BITPREF+ -ILONLY-");
    }

    [Fact]
    public void TestDotNet()
    {
        Assert(new DotNetTestSettings()
                .SetProjectFile("test-project.csproj")
                .AddRunSetting("key", "value")
                .AddRunSetting("foo", "bar")
                .EnableNoLogo(),
            "test --project test-project.csproj --nologo -- key=value foo=bar");

        Assert(new DotNetRunSettings()
                .AddApplicationArguments("arg1")
                .AddApplicationArguments("arg2")
                .SetProperty("foo", "bar")
                .SetInformationalVersion("version"),
            "run --property:foo=bar --property:InformationalVersion=version -- arg1 arg2");

        Assert(new DotNetPackSettings()
                .SetAuthors("a", "b"),
            "pack --property:Authors=a,b");
    }

    [Fact]
    public void TestDotNet_Delegate()
    {
        Assert(new DotNetBuildSettings()
                .SetNoWarns(1337),
            "build --property:NoWarn=1337");

        Assert(new DotNetBuildSettings()
                .SetNoWarns(1337, 555)
                .RemoveNoWarns(555),
            "build --property:NoWarn=1337");

        Assert(new DotNetBuildSettings()
                .AddNoWarns(1337),
            "build --property:NoWarn=1337");

        Assert(new DotNetBuildSettings()
                .AddNoWarns(1337)
                .SetNoWarns(555),
            "build --property:NoWarn=555");
    }

    [Fact]
    public void TestDotNet_Empty()
    {
        Assert(new DotNetBuildSettings()
                .SetFramework(null)
                .SetConfiguration("")
                .SetProperty("foo1", null)
                .SetProperty("foo2", ""),
            "build");
    }

    [Fact]
    public void TestDocker()
    {
        Assert(new DockerAttachSettings()
                .SetDetachKeys("detach-keys")
                .SetContainer("container")
                .SetLogLevel(DockerLogLevel.debug),
            "attach --detach-keys detach-keys --log-level debug container");
    }

    [Fact]
    public Task TestDiscord()
    {
        var result = new DiscordMessage()
            .SetNonce("nonce")
            .SetChannelId("channel-id")
            .AddEmbeds(_ => _
                .SetType(DiscordEmbedType.article)
                .SetAuthor(_ => _
                    .SetName("author-name")))
            .ToJson(Options.JsonSerializerSettings);

        return Verifier.Verify(result);
    }

    [Fact]
    public void TestKubernetes()
    {
        Assert(new KubernetesExecSettings()
                .SetContainer("container")
                .SetCommand("command")
                .SetArguments("arg1", "arg2")
                .SetCluster("cluster"),
            "exec --container=container --cluster=cluster -- command arg1 arg2");
    }

    private static void Assert<T>(T options, string expected)
        where T : ToolOptions, new()
    {
        var arguments = options.GetArguments().JoinSpace();
        arguments.Should().Be(expected);
    }
}
