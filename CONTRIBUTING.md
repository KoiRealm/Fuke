# Contributing to FUKE

English | [中文](CONTRIBUTING.zh-CN.md)

FUKE is an independently maintained build automation project. By contributing, you agree to follow the [Code of Conduct](CODE_OF_CONDUCT.md) and license your contribution under this project's MIT License.

## Reporting an issue

Before opening an issue:

- Reproduce it against the current FUKE code and include the .NET SDK, operating system, and FUKE version.
- Provide minimal reproduction steps, the complete error text, and relevant build logs. Do not replace logs with screenshots.
- Confirm that the problem is not caused by an external CLI tool; when necessary, run that tool directly for comparison.
- Search existing issues and `CHANGELOG.md`.

## Submitting a change

- Create a focused feature branch from the current development branch.
- Follow the existing code style and add tests for behavioral changes.
- Do not introduce silent degradation, implicit fallback behavior, or swallowed exceptions. States that cannot be handled correctly must fail explicitly.
- Use `Fuke.*`, `fuke`, `.fuke`, and `FUKE_` for public identifiers. Do not introduce conflicting package IDs, commands, or environment variables.
- Do not remove or weaken the license and origin notices in the root `LICENSE` and `README.md` files.

Run the following commands from the repository root before submitting:

```powershell
dotnet restore fuke-common.slnx
dotnet build fuke-common.slnx --configuration Release --no-restore
dotnet test fuke-common.slnx --configuration Release --no-build
```

When modifying a CLI tool wrapper, update its JSON specification and generated code, then validate the change with the `GenerateTools` target.
