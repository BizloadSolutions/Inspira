# Contributing to Inspira

## Guidelines
- Follow the project `.editorconfig` rules (4-space indent, file-scoped namespaces, nullable enabled).
- Use PascalCase for public types and members. Private fields: `_camelCase` with leading underscore and readonly where appropriate.
- Prefer `var` when the type is obvious.
- Keep using directives inside the namespace and sort `System` first.

## Branching and PRs
- Work on feature branches off `main`.
- Open a PR with a clear description and link any relevant issue.
- Include unit tests for behavior changes. Tests should be in `tests/Inspira.Tests`.

## Building locally
- Scaffolded with `dotnet` CLI. To open the solution in Visual Studio, use __File > Open > Project/Solution__ or open the `.sln` directly.
- Use __Solution Explorer__ to inspect projects.

## Code reviews
- Focus on architecture, naming, and test coverage.
- Suggest small incremental PRs.

## Standards
- Nullable reference types enabled.
- Implicit usings enabled.
- Expression-bodied members allowed when concise.
