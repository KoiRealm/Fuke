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
    private void Convert()
    {
        DotNetBuild(_ => _
            .SetProjectFile(RootDirectory / "source")
            .SetConfiguration(configuration)
            .SetProcessArgumentConfigurator(args => args.Add($"/p:Version={nugetVersion}")));
        DotNetTest(_ => _
            .SetProjectFile(testProjectFile)
            .SetConfiguration(configuration)
            .SetNoBuild(true));
        DotNetPack(_ => _
            .SetProjectFile(octopusClientFolder)
            .SetProcessArgumentConfigurator(args =>
        {
            args.Add($"/p:Version={nugetVersion}");
            args.Add("/p:NuspecFile=file.nuspec");
            return args;
        })
            .SetConfiguration(configuration)
            .SetOutputDirectory(artifactsDir)
            .SetNoBuild(true)
            .SetIncludeSymbols(false)
            .SetVerbosity(DotNetVerbosity.Normal));
        SignTool(_ => _
            .SetFiles(files)
            .SetProcessToolPath(RootDirectory / "certificates" / "signtool.exe")
            .SetTimeStampUri(new Uri("http://rfc3161timestamp.globalsign.com/advanced"))
            .SetTimeStampDigestAlgorithm(SignToolDigestAlgorithm.Sha256)
            .SetCertPath(signingCertificatePath)
            .SetPassword(signingCertificatePassword));
    }
}