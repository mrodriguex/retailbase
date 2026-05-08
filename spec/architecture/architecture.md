# Architecture Specification

## 1. System Architecture Overview

RETAIL.BASE is a **layered monolith** built on ASP.NET Core 8. All layers reside in the same Git repository and are composed at startup via the built-in Dependency Injection container. There is no microservice split. Real-time communication is handled by an embedded **SignalR** hub.

```
┌─────────────────────────────────────┐
│          RETAIL.BASE.API            │  ← HTTP entry-point (REST + SignalR)
│  Controllers / Hubs / Helpers       │
└────────────────┬────────────────────┘
                 │ depends on
┌────────────────▼────────────────────┐
│          RETAIL.BASE.NEG            │  ← Business logic
│  Services / Interfaces / Helpers    │
└────────────────┬────────────────────┘
                 │ depends on
┌────────────────▼────────────────────┐
│          RETAIL.BASE.DAT            │  ← Data access
│  DbContext / Repositories           │
│  EF Core Migrations                 │
└────────────────┬────────────────────┘
                 │ depends on
┌────────────────▼────────────────────┐
│          RETAIL.BASE.OBJ            │  ← Shared objects
│  Entities / Models / Enums          │
└─────────────────────────────────────┘
```

---

## 2. Project / Module Structure

### RETAIL.BASE.OBJ
Shared class library referenced by all other projects.
- **Entities**: `Base`, `Brand`, `Category`, `Company`, `Customer`, `MenuItem`, `Product`, `ProductPresentation`, `Role`, `User`
- **Models**: `ResultModel<T>`, `PagedResult<T>`, `LoginModel`
- **Filter**: `BaseFilter`
- **Enums**: `Entidad`, `Evento`, `LogEvento`, `TipoEvento`

### RETAIL.BASE.DAT
Data access class library.
- `RETAIL_BASEDbContext` — EF Core `DbContext` targeting PostgreSQL via Npgsql.
- `Repositories/` — One concrete repository per entity implementing `IRepositoryBase<TEntity, TFilter, TId>`.
- `Interfaces/IRepositoryBase<>` — Generic CRUD interface.
- `Migrations/` — EF Core migration history (two migrations: `InitialCreate`, `AddBrandCategoryProductPresentation`).

### RETAIL.BASE.NEG
Business logic class library.
- `Services/` — One service per domain entity implementing the corresponding interface.
- `Interfaces/` — `IServiceBase<>` generic interface plus entity-specific extensions.
- `Helpers/CryptographerSHA512B` — SHA-512 hashing implementation.

### RETAIL.BASE.API
ASP.NET Core 8 Web API application.
- `Program.cs` — Application bootstrap, DI registration, middleware pipeline.
- `Controllers/Base/BaseController` — Abstract base that exposes `IdUserAutenticado` (extracted from JWT).
- `Controllers/V1/` — Version 1 controllers for all domain entities.
- `Controllers/V2/` — Version 2 controllers (`Auth`, `User` only).
- `Controllers/Hubs/DataHub` — SignalR hub for real-time broadcast.
- `Controllers/RPT/` — Reporting controllers (all **commented out**, not active).
- `Helpers/JwtAuthenticateHelper` — JWT generation and claim extraction.
- `Helpers/DependencyInjectionHelper` — Extension method `AddApplicationServices()`.
- `Helpers/ConfigureSwaggerOptions` — Swagger multi-version configuration.

---

## 3. Layer Responsibilities

| Layer | Responsibility |
|---|---|
| **OBJ** | Defines entities shared across all layers; has no dependencies. |
| **DAT** | Translates between domain entities and the PostgreSQL database using EF Core. Owns the migration history. |
| **NEG** | Applies business rules (audit timestamps, account lockout, field preservation on update), wraps results in `ResultModel<T>`. |
| **API** | Receives HTTP requests, extracts the authenticated user ID, delegates to services, and maps success/failure to `200 OK` / `400 BadRequest`. |

---

## 4. Dependency Flow

All dependencies are declared in `DependencyInjectionHelper.cs` and registered with `IServiceCollection`:

- Repositories are registered as `IRepositoryBase<Entity, BaseFilter, int>` → concrete `*DA` class (`Scoped`).
- Services are registered as their interface → concrete `*Service` class (`Scoped`).
- `CryptographerSHA512B` is registered as `ICryptographerB` (`Scoped`).
- `HubCommunicationService` is registered as itself (`Scoped`).
- `ConfigService` is registered as itself (`Scoped`).
- `RETAIL_BASEDbContext` is registered as EF Core `DbContext` with Npgsql, migrations assembly set to `RETAIL.BASE.DAT`.

---

## 5. Authentication and Authorization Design

### Mechanism
JWT Bearer authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`). The token signing algorithm is **HMAC-SHA256**.

### Token issuance (v1)
1. `POST /api/v1/Auth/login` receives `LoginModel`.
2. `AuthService.AuthenticateUserAsync()` checks credentials against:
   - A hardcoded default administrator account (`DefaultUser` / `DefaultPassword` in `appsettings.json`).
   - Regular users via SHA-512 hash comparison.
3. On success, `JwtAuthenticateHelper.GenerateJwtToken()` mints a token embedding the numeric user ID as a `ClaimTypes.Name` claim.
4. Token duration is read from `Jwt:Duration` (appsettings), defaulting to 60 minutes.

### Token issuance (v2) — incomplete / non-production
V2 `AuthController` uses a **hardcoded signing key** (`"Cryoinfra_SDL_3d80b5da-824b-4dde-b1db-3942c6d3d9fc"`) and does **not** validate credentials. It accepts any username/password and generates a 15-minute token. This endpoint is functionally insecure.

### Token consumption
All controllers except `AuthController` and a few explicitly anonymous endpoints inherit from `BaseController`, which carries `[Authorize]`. The base class exposes `IdUserAutenticado`, which re-parses the `Authorization` header on each request to extract the user ID.

### Authorization
Role-based authorization is **not enforced at the HTTP layer**. The `[Authorize]` attribute only requires a valid JWT. Role filtering is expected to be handled at the UI or service layer but no middleware-level role checks are present in the codebase.

---

## 6. Configuration and Environment Handling

Configuration is provided via the standard ASP.NET Core `IConfiguration` pipeline.

| Key | Purpose |
|---|---|
| `ConnectionStrings:SqlConn_RETAIL_BASE` | PostgreSQL connection string |
| `Jwt:Key` | JWT signing key (symmetric) |
| `Jwt:Duration` | Token lifetime in minutes |
| `DefaultUser` | Username of the built-in administrator |
| `DefaultPassword` | Password of the built-in administrator (plain text) |
| `Logging:LogLevel` | Log verbosity per provider |

`appsettings.Development.json` only overrides log levels. No environment-specific connection strings are defined in code.

`ConfigService` provides a live read/write interface for `appsettings.json` at runtime via `ConfigController`. Any authenticated user can modify configuration keys through this controller (no additional authorization guard).

---

## 7. Architectural Patterns in Use

| Pattern | Where |
|---|---|
| Repository Pattern | `IRepositoryBase<>` + `*DA` classes in `RETAIL.BASE.DAT` |
| Service Layer | `IServiceBase<>` + `*Service` classes in `RETAIL.BASE.NEG` |
| Generic typing | `IRepositoryBase<MyClass, FilterClass, IdType>` and `IServiceBase<>` — one interface covers all entities |
| Result wrapper | `ResultModel<T>` wraps every service and controller response |
| Paged results | `PagedResult<T>` used for all list operations |
| API Versioning | URL segment versioning via `Asp.Versioning` (v1, v2) |
| Real-time push | SignalR hub (`DataHub`) for broadcasting events to connected clients |
| Audit fields | `IdUserCreation`, `IdUserModification`, `DateTimeCreation`, `DateTimeModification` set automatically in services |

---

## 8. CI/CD Pipeline

The repository contains a `Jenkinsfile` that defines a declarative Jenkins pipeline. The pipeline runs inside a `mcr.microsoft.com/dotnet/sdk:8.0` Docker agent.

### Stages

| Stage | Description |
|---|---|
| **Prepare** | Installs `openssh-client` via `apt-get`; creates a writable home directory for the .NET CLI. |
| **Checkout** | Clones the repository via `checkout scm`. |
| **Set Variables** | Resolves environment-specific variables (`PROJECT_DIR`, `PROJECT_NAME`, `DEPLOY_DIR`, `DEPLOY_PATH`, `SERVICE`) from parameterized build inputs (`PROJECT`, `ENVIRONMENT`). |
| **Build** | Runs `dotnet restore`, `dotnet build -c Release`, and `dotnet publish -c Release -o ./publish` on the selected project. |
| **Deploy** | Copies the publish output to the remote server via `rsync` over SSH. |
| **Restart** | Restarts the target `systemd` service on the remote host and verifies it is active. |

### Environment variables

| Variable | Value |
|---|---|
| `DOTNET_CLI_TELEMETRY_OPTOUT` | `1` |
| `DOTNET_SKIP_FIRST_TIME_EXPERIENCE` | `1` |
| `DOTNET_CLI_HOME` | `/tmp/dotnet-home` |
| `USER` | `mrodriguex` |
| `SERVER` | `62.72.3.22` |

### Notes
- Parameterized pipeline inputs (`PROJECT`, `ENVIRONMENT`, `SKIP_BUILD`) are commented out in the current `Jenkinsfile`; their `Set Variables` usage remains active in code but would fail without values being provided.
- SSH key management is expected to be configured as a Jenkins credential; the `Jenkinsfile` does not embed keys.
- The pipeline targets a Linux server running `systemd`.
