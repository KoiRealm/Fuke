# FUKE

FUKE 是一个用于构建和部署的 .NET 工具，用于自动化 .NET 以及其它常见技术栈项目的构建和部署流程。

[English](README.md) | 中文

## 安装方法

```powershell
dotnet tool install --global Fuke.GlobalTool
```

安装完成后可在任意目录运行检查运行状态：

```powershell
fuke :version
```

升级到后续版本时使用：

```powershell
dotnet tool update --global Fuke.GlobalTool --version <版本号>
```

## 全局设置

FUKE 默认显示英语并隐藏大型 ASCII 标志。在 Windows 上，全局设置保存在 `%APPDATA%/KoiRealm/Fuke/settings.json`。

```powershell
fuke :settings
fuke :settings --language Chinese
fuke :settings --language English
fuke :settings --show-logo true
fuke :settings --show-logo false
fuke :settings --reset
```

## 本地构建

需要安装 .NET SDK 10。

```powershell
dotnet restore fuke-common.slnx
dotnet build fuke-common.slnx --configuration Release --no-restore
dotnet test --solution fuke-common.slnx --configuration Release --no-build
```

要运行仓库自身的 FUKE 构建：

```powershell
./build.ps1 --help
```

使用 `--no-logo` 可简化构建结果，省略目标表格和装饰性总结。

## 杂谈

本项目的初版是基于 [NUKE Build](https://github.com/nuke-build/nuke) 10.1.0版本开发的，感谢 NUKE Build 团队一直以来的贡献。项目原本的名字叫FishNuke，由于原名称过于冗长难以输入和记忆，因此简化为FUKE以便于记忆和使用。
本项目与原项目之间是完全独立的项目，与原项目不存在官方隶属关系。
