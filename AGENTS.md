# AGENTS.md — Alma.Fable.Storage

This repo ships Agent Skill for the `Alma.Fable.Storage` library. Compatible agents discover it automatically; see `.agents/skills/fable-storage/SKILL.md`.

## Project Purpose

Fable (F#→JavaScript) library for working with browser storages (LocalStorage, SessionStorage). Provides typed save/load operations with JSON serialization via Thoth.Json. Published as NuGet package `Alma.Fable.Storage`.

## Tech Stack

- **Language:** F# (.NET 10), Fable-compatible
- **Framework:** .NET SDK library for Fable projects
- **Package management:** Paket
- **Build system:** FAKE (F# Make) via `build.sh`
- **Linting:** fsharplint
- **CI/CD:** GitHub Actions
- **Key dependencies:** `FSharp.Core ~> 10.0`, `Fable.Core ~> 4`, `Fable.Browser.WebStorage ~> 1.3`, `Thoth.Json ~> 10.4`

## Commands

```bash
# Install dependencies
dotnet tool restore && dotnet paket install

# Build
./build.sh build

# Run tests
./build.sh -t tests

# Lint
dotnet fsharplint lint src/Alma.Fable.Storage/Alma.Fable.Storage.fsproj
```

## Project Structure

```
fable-storage/
├── src/
│   └── Alma.Fable.Storage/
│       ├── Alma.Fable.Storage.fsproj  # Main project (PackageId: Alma.Fable.Storage, v9.0.1)
│       ├── AssemblyInfo.fs            # Auto-generated
│       ├── LocalStorage.fs            # LocalStorage operations (save, load, loadWith)
│       └── paket.references
├── build/
│   ├── build.fsproj                   # FAKE build project
│   ├── Build.fs                       # Build entry point
│   └── AssemblyInfo.fs
├── build.sh                           # Build entry script
├── paket.dependencies                 # Top-level dependencies
├── fsharplint.json                    # Lint configuration
├── CHANGELOG.md                       # Release history
└── .github/workflows/
    ├── tests.yaml                     # Tests on PRs and nightly
    ├── pr-check.yaml                  # Fixup commit blocker, ShellCheck
    └── publish.yaml                   # NuGet publish on tags
```

## Architecture

Pure Fable library exposing:

- `LocalStorage.save` — saves data to browser LocalStorage as JSON
- `LocalStorage.load<'T>` — loads and deserializes typed data from LocalStorage
- `LocalStorage.loadWith` — loads using a custom Thoth.Json decoder

**Fable-only** — this library targets browser environments via Fable. Do not use .NET-only APIs.

## Build System (FAKE)

Standard library target chain: `Clean → AssemblyInfo → Build → Lint → Tests → Release → Publish`

## CI/CD

- **tests.yaml** — runs on PRs and nightly
- **pr-check.yaml** — blocks fixup commits, runs ShellCheck
- **publish.yaml** — publishes to NuGet on semver tags

## Release Process

1. Increment `<Version>` in `src/Alma.Fable.Storage/Alma.Fable.Storage.fsproj`
2. Update `CHANGELOG.md`
3. Commit, tag with version, push

## Conventions

- Source code under `src/Alma.Fable.Storage/`
- Uses Thoth.Json for serialization/deserialization
- Must remain Fable-compatible — no .NET-only APIs
- Compile order in `.fsproj` matters

## Pitfalls

- **No Docker** — pure library, browser-only via Fable
- **Fable-only** — cannot be used in server-side .NET code directly; targets browser JavaScript
- **No tests directory** — currently no test project exists
- **Source location** — source is in `src/Alma.Fable.Storage/`, not project root
- **Paket, not NuGet CLI** — use `dotnet paket install`
