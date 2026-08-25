# FUKE

FUKE is a .NET build and deployment tool designed to automate build and deployment workflows for .NET projects and other commonly used technology stacks.

English | [中文](README.zh-CN.md)

## Installation

```powershell
dotnet tool install --global Fuke.GlobalTool --version 0.1.0
```

After installation, verify that FUKE is available from any directory:

```powershell
fuke :version
```

To upgrade to a later version, run:

```powershell
dotnet tool update --global Fuke.GlobalTool --version <version>
```

## Building Locally

.NET SDK 10 is required.

```powershell
dotnet restore fuke-common.slnx
dotnet build fuke-common.slnx --configuration Release --no-restore
dotnet test --solution fuke-common.slnx --configuration Release --no-build
```

To run the repository's own FUKE build:

```powershell
./build.ps1 --help
```

## Background

The initial version of this project was developed based on [NUKE Build](https://github.com/nuke-build/nuke) 10.1.0. We thank the NUKE Build team for their long-standing contributions. The project was originally named FishNuke, but the name was shortened to FUKE to make it easier to remember and type. FUKE is a fully independent project and has no official affiliation with the original project.
