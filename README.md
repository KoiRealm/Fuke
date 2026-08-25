# FUKE

FUKE 是一个用于构建和部署的 .NET 工具，用于自动化 .NET 以及其它常见技术栈项目的构建和部署流程。

## 安装方法

```powershell
dotnet tool install --global Fuke.GlobalTool --version 0.0.0.1
```

安装完成后可在任意目录运行检查运行状态：

```powershell
fuke :version
```

升级到后续版本时使用：

```powershell
dotnet tool update --global Fuke.GlobalTool --version <版本号>
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

## 项目致谢

本项目的初版是基于 [NUKE Build](https://github.com/nuke-build/nuke) 开发的，感谢 NUKE Build 团队一直以来的贡献。
