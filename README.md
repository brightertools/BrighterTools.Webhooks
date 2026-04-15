# BrighterTools.Webhooks

`BrighterTools.Webhooks` is a storage-agnostic webhook queueing and delivery library for .NET applications.

The host application owns:
- subscription persistence
- delivery persistence
- scheduling and background execution
- tenant or owner rules
- domain event publishing
- endpoint governance and operational policy

The library provides:
- webhook queueing services
- payload wrapping
- retry orchestration
- delivery processing
- HTTP delivery execution
- DI registration helpers

## Package

```powershell
dotnet add package BrighterTools.Webhooks
```

See the package-level readme in [`src/BrighterTools.Webhooks/README.md`](src/BrighterTools.Webhooks/README.md) for the minimal registration shape.

## Repository Layout

- `src/BrighterTools.Webhooks`
  - reusable webhook queueing and delivery package
- `tests/BrighterTools.Webhooks.Tests`
  - package-level behavior tests for retry, payload creation, and queueing
- `docs`
  - public integration documentation

## Validation

```powershell
dotnet test .\BrighterTools.Webhooks.sln
```

## Documentation

- [`docs/README.md`](docs/README.md)
- [`docs/integration-guide.md`](docs/integration-guide.md)
