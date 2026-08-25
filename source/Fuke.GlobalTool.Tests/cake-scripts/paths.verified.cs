using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Fuke.Common;
using Fuke.Common.Execution;
using Fuke.Common.IO;
using Fuke.Common.ProjectModel;
using Fuke.Common.Tooling;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Tools.GitVersion;
using Fuke.Common.Tools.SignTool;
using Fuke.Common.Utilities.Collections;
using Fuke.Common;
using Fuke.Common.Tools.DotNet;
using Fuke.Common.Tools.MSBuild;
using Fuke.Common.Tools.SignTool;
using Fuke.Common.Tools.NuGet;
using Fuke.Common.IO;
using Fuke.Common.IO;
using Fuke.Common;
using static Fuke.Common.ControlFlow;
using static Fuke.Common.Tools.DotNet.DotNetTasks;
using static Fuke.Common.Tools.MSBuild.MSBuildTasks;
using static Fuke.Common.Tools.SignTool.SignToolTasks;
using static Fuke.Common.Tools.NuGet.NuGetTasks;
using static Fuke.Common.IO.TextTasks;
using static Fuke.Common.IO.XmlTasks;
using static Fuke.Common.EnvironmentInfo;

class Build : FukeBuild
{
    AbsolutePath LocalPackagesDir => RootDirectory / ".." / "LocalPackages";

    AbsolutePath SourceFolder => RootDirectory / "source";

    AbsolutePath PublishDir => RootDirectory / "publish";

    AbsolutePath SignToolPath => RootDirectory / "certificates" / "signtool.exe";

    private string Convert(AbsolutePath file)
    {
        file = (AbsolutePath)file;
        CopyFile(RootDirectory / projectFile/ $"{projectFile}.nuspec", "nuspec");
    }

    private void NoConvert()
    {
        var nodes = doc.SelectNodes("Project/PropertyGroup/RuntimeIdentifiers");
        var node = doc.SelectSingleNode("Project/PropertyGroup/RuntimeIdentifiers");
    }
}