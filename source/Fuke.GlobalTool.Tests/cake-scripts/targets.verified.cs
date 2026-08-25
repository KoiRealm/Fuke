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

    Target A => _ => _
        .Executes(() =>
    {
        System.Console.WriteLine();
    });


    Target B => _ => _
        .DependsOn(A)
        .DependentFor(A)
        .Executes(() =>
    {
        System.Console.WriteLine();
    });


    Target C_1 => _ => _
        .DependsOn(B)
        .OnlyWhenStatic(() => staticCondition)
        .OnlyWhenDynamic(() => dynamicCondition)
        .ProceedAfterFailure();
}