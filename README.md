
# RETAIL.BASE

RETAIL.BASE is a modular and extensible framework designed to facilitate rapid development of enterprise-grade applications. It provides a set of reusable components, libraries, and tools that streamline common development tasks and promote best practices.

## Current repository status

This README was regenerated from the current source tree on **2026-03-14**.

### Projects currently in `RETAIL.BASE.sln`

- `RETAIL.BASE.OBJ` (`net8.0`)
- `RETAIL.BASE.DAT` (`net8.0`)
- `RETAIL.BASE.NEG` (`net8.0`)
- `RETAIL.BASE.API` (`net8.0`)

> `RETAIL.BASE.SER` and `RETAIL.BASE.WEB` are no longer part of the active solution.

## Architecture

Layered dependency flow:

`RETAIL.BASE.API -> RETAIL.BASE.NEG -> RETAIL.BASE.DAT -> RETAIL.BASE.OBJ`

`RETAIL.BASE.API` also references `RETAIL.BASE.OBJ` directly.

## Main components

### `RETAIL.BASE.API`

- ASP.NET Core Web API (`net8.0`)
- API versioning via URL segment (`v1`, `v2`)
- JWT authentication
- Swagger/OpenAPI
- CORS policy allowing any origin/header/method
- Telerik Reporting packages configured

Controllers currently present:

- `V1`: `Auth`, `Customer`, `Config`, `Cryptographer`, `Company`, `MenuItem`, `Role`, `User`
- `V2`: `Auth`, `User`
- `RPT`: report/viewer controllers exist in source

### `RETAIL.BASE.NEG`

Business rules and application services:

- Business classes: `AuthB`, `CustomerB`, `CryptographerSHA512B`, `CompanyB`, `MenuItemB`, `RoleB`, `UserB`
- Service classes (folder `Services/`) consumed by API controllers

### `RETAIL.BASE.DAT`

Data access layer:

- `RETAIL_BASEDbContext` with EF Core SQL Server provider
- Repository-style classes: `UserDA`, `RoleDA`, `CustomerDA`, `MenuItemDA`, `CompanyDA`
- Migrations folder included

### `RETAIL.BASE.OBJ`

Shared models and base objects:

- `User`, `Role`, `Customer`, `Company`, `MenuItem`, `WebResult`, filters, enums, and common model classes

### `RETAIL.BASE.NEG.Tests`

Unit tests (not included in solution file, but present in repository):

- xUnit + Moq
- Current target framework: `net10.0`
- Service test classes: `UserServiceTests`, `CustomerServiceTests`, `CompanyServiceTests`, `RoleServiceTests`, `MenuItemServiceTests`, `ConfigServiceTests`, `CryptographerServiceTests`

## Repository structure

```text
RETAIL.BASE/
├── RETAIL.BASE.sln
├── Jenkinsfile
├── RETAIL.BASE.API/
├── RETAIL.BASE.NEG/
├── RETAIL.BASE.DAT/
├── RETAIL.BASE.OBJ/
├── RETAIL.BASE.NEG.Tests/
├── UNIT_TESTS_SUMMARY.md
└── LICENSE
```

## Prerequisites

- .NET SDK 8.0 (build/run solution projects)
- .NET SDK 10.0 (run current test project as configured)
- SQL Server instance for data access

## Configuration

Primary app configuration file:

- `RETAIL.BASE.API/appsettings.json`

Important keys expected by the API startup and DI code:

- `Jwt:Key`
- `ConnectionStrings:DefaultConnection`

> Recommended: keep secrets out of source control and use environment variables / secret management.

## Build

Build the solution:

```bash
dotnet build RETAIL.BASE.sln
```

Build API only:

```bash
dotnet build RETAIL.BASE.API/RETAIL.BASE.API.csproj
```

## Run

Run API locally:

```bash
dotnet run --project RETAIL.BASE.API/RETAIL.BASE.API.csproj
```

Swagger UI is enabled by the API pipeline.

## Test

Run business/service tests:

```bash
dotnet test RETAIL.BASE.NEG.Tests/RETAIL.BASE.NEG.Tests.csproj
```

## CI/CD

Current `Jenkinsfile` pipeline:

1. Checkout from GitHub (`main`)
2. Publish `RETAIL.BASE.API` for `linux-x64`
3. Deploy via SSH + `rsync`
4. Restart and verify systemd service `RETAIL.BASE.API`

## License

See `LICENSE`.
