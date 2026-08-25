# FUKE 贡献指南

FUKE 是独立维护的构建自动化项目。提交代码即表示你同意遵守 [行为准则](CODE_OF_CONDUCT.md)，并以本项目的 MIT 许可证发布贡献。

## 提交问题

在创建问题前，请先：

- 确认问题可以在当前 FUKE 代码上复现，并说明所用的 .NET SDK、操作系统和 FUKE 版本。
- 附上最小复现步骤、完整错误文本和必要的构建日志；日志不要以截图代替。
- 确认问题不是外部 CLI 工具本身导致；必要时直接运行该工具命令作对照。
- 搜索当前仓库的已有问题和 `CHANGELOG.md`。

## 提交代码

- 从当前维护分支创建功能分支。
- 保持变更范围聚焦，遵循现有代码风格，对行为变更补充测试。
- 不要引入静默降级、隐式回退或为了“先跑起来”而吞掉异常的逻辑。无法正确处理的状态应该明确报错。
- 公开标识统一使用 `Fuke.*`、`fuke`、`.fuke` 和 `FUKE_`，不要新增冲突的包 ID、命令或环境变量。
- 不要删除或弱化根目录 `LICENSE` 和 `README.md` 中的许可证与来源说明。

提交前请在仓库根目录运行：

```powershell
dotnet restore fuke-common.slnx
dotnet build fuke-common.slnx --configuration Release --no-restore
dotnet test fuke-common.slnx --configuration Release --no-build
```

如果修改了 CLI 工具包装器，请同时更新对应 JSON 规格和生成代码，并运行 `GenerateTools` 目标验证。
