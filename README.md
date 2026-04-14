
# HARD.CORE

Hard.Core is a modular and extensible framework designed to facilitate rapid development of enterprise-grade applications. It provides a set of reusable components, libraries, and tools that streamline common development tasks and promote best practices.

## Current repository status

This README was regenerated from the current source tree on **2026-03-14**.

### Projects currently in `HARD.CORE.sln`

- `HARD.CORE.OBJ` (`net8.0`)
- `HARD.CORE.DAT` (`net8.0`)
- `HARD.CORE.NEG` (`net8.0`)
- `HARD.CORE.API` (`net8.0`)

> `HARD.CORE.SER` and `HARD.CORE.WEB` are no longer part of the active solution.

## Architecture

Layered dependency flow:

`HARD.CORE.API -> HARD.CORE.NEG -> HARD.CORE.DAT -> HARD.CORE.OBJ`

`HARD.CORE.API` also references `HARD.CORE.OBJ` directly.

## Main components

### `HARD.CORE.API`

- ASP.NET Core Web API (`net8.0`)
- API versioning via URL segment (`v1`, `v2`)
- JWT authentication
- Swagger/OpenAPI
- CORS policy allowing any origin/header/method
- Telerik Reporting packages configured

Controllers currently present:

- `V1`: `Auth`, `Cliente`, `Config`, `Cryptographer`, `Empresa`, `Menu`, `Perfil`, `Usuario`
- `V2`: `Auth`, `Usuario`
- `RPT`: report/viewer controllers exist in source

### `HARD.CORE.NEG`

Business rules and application services:

- Business classes: `AuthB`, `ClienteB`, `CryptographerSHA512B`, `EmpresaB`, `MenuB`, `PerfilB`, `UsuarioB`
- Service classes (folder `Services/`) consumed by API controllers

### `HARD.CORE.DAT`

Data access layer:

- `HardCoreDbContext` with EF Core SQL Server provider
- Repository-style classes: `UsuarioDA`, `PerfilDA`, `ClienteDA`, `MenuDA`, `EmpresaDA`
- Migrations folder included

### `HARD.CORE.OBJ`

Shared models and base objects:

- `Usuario`, `Perfil`, `Cliente`, `Empresa`, `Menu`, `WebResult`, filters, enums, and common model classes

### `HARD.CORE.NEG.Tests`

Unit tests (not included in solution file, but present in repository):

- xUnit + Moq
- Current target framework: `net10.0`
- Service test classes: `UsuarioServiceTests`, `ClienteServiceTests`, `EmpresaServiceTests`, `PerfilServiceTests`, `MenuServiceTests`, `ConfigServiceTests`, `CryptographerServiceTests`

## Repository structure

```text
hard.core/
├── HARD.CORE.sln
├── Jenkinsfile
├── HARD.CORE.API/
├── HARD.CORE.NEG/
├── HARD.CORE.DAT/
├── HARD.CORE.OBJ/
├── HARD.CORE.NEG.Tests/
├── UNIT_TESTS_SUMMARY.md
└── LICENSE
```

## Prerequisites

- .NET SDK 8.0 (build/run solution projects)
- .NET SDK 10.0 (run current test project as configured)
- SQL Server instance for data access

## Configuration

Primary app configuration file:

- `HARD.CORE.API/appsettings.json`

Important keys expected by the API startup and DI code:

- `Jwt:Key`
- `ConnectionStrings:DefaultConnection`

> Recommended: keep secrets out of source control and use environment variables / secret management.

## Build

Build the solution:

```bash
dotnet build HARD.CORE.sln
```

Build API only:

```bash
dotnet build HARD.CORE.API/HARD.CORE.API.csproj
```

## Run

Run API locally:

```bash
dotnet run --project HARD.CORE.API/HARD.CORE.API.csproj
```

Swagger UI is enabled by the API pipeline.

## Test

Run business/service tests:

```bash
dotnet test HARD.CORE.NEG.Tests/HARD.CORE.NEG.Tests.csproj
```

## CI/CD

Current `Jenkinsfile` pipeline:

1. Checkout from GitHub (`main`)
2. Publish `HARD.CORE.API` for `linux-x64`
3. Deploy via SSH + `rsync`
4. Restart and verify systemd service `HARD.CORE.API`

## License

See `LICENSE`.
