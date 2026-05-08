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
Shared class library referenced by all other projects. Owns the domain ports (Hexagonal Architecture Phase 1).
- **Entities**: `Base`, `Brand`, `Category`, `Company`, `Customer`, `MenuItem`, `Product`, `ProductPresentation`, `Role`, `User`
- **Models**: `ResultModel<T>`, `PagedResult<T>`, `LoginModel`
- **Filter**: `BaseFilter`
- **Enums**: `Entidad`, `Evento`, `LogEvento`, `TipoEvento`
- **Ports**: `Ports/IRepositoryBase<TEntity, TFilter, TId>` — Generic CRUD port (technology-agnostic).

### RETAIL.BASE.DAT
Data access class library — EF Core adapter (Hexagonal Architecture Phase 1 implemented).
- `Adapters/EF/RETAIL_BASEDbContext.cs` — EF Core `DbContext` targeting SQLite via `Microsoft.EntityFrameworkCore.Sqlite`.
- `Adapters/EF/*DA.cs` — One concrete repository adapter per entity, implementing `IRepositoryBase<TEntity, TFilter, TId>` (the port defined in `RETAIL.BASE.OBJ`).
- `Migrations/` — EF Core migration history.

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
| **DAT** | Translates between domain entities and the SQLite database using EF Core. Owns the migration history. |
| **NEG** | Applies business rules (audit timestamps, account lockout, field preservation on update), wraps results in `ResultModel<T>`. |
| **API** | Receives HTTP requests, extracts the authenticated user ID, delegates to services, and maps success/failure to `200 OK` / `400 BadRequest`. |

---

## 4. Dependency Flow

All dependencies are declared in `DependencyInjectionHelper.cs` and registered with `IServiceCollection`:

- Repositories are registered as `IRepositoryBase<Entity, BaseFilter, int>` (port from `RETAIL.BASE.OBJ.Ports`) → concrete `*DA` adapter class in `RETAIL.BASE.DAT` (`Scoped`).
- Services are registered as their interface → concrete `*Service` class (`Scoped`).
- `CryptographerSHA512B` is registered as `ICryptographerB` (`Scoped`).
- `CryptographerService` is registered as `ICryptographerService` (`Scoped`) — exposes hashing operations via `CryptographerController`.
- `HubCommunicationService` is registered as itself (`Scoped`).
- `ConfigService` is registered as itself (`Scoped`).
- `RETAIL_BASEDbContext` is registered as EF Core `DbContext` with SQLite, migrations assembly set to `RETAIL.BASE.DAT`.

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
| `ConnectionStrings:SqlConn_RETAIL_BASE` | SQLite connection string (`Data Source=retailbase.db`) |
| `Jwt:Key` | JWT signing key (symmetric) |
| `Jwt:Duration` | Token lifetime in minutes |
| `DefaultUser` | Username of the built-in administrator |
| `DefaultPassword` | Password of the built-in administrator (plain text) |
| `Logging:LogLevel` | Log verbosity per provider |

`appsettings.Development.json` only overrides log levels. No environment-specific connection strings are defined in code.

`ConfigService` provides a live read/write interface for `appsettings.json` at runtime via `ConfigController`. `ConfigController` carries **no** `[Authorize]` attribute — any unauthenticated caller can read or overwrite configuration keys (see security spec §9).

---

## 7. Architectural Patterns in Use

| Pattern | Where |
|---|---|
| Repository Pattern (Hexagonal Port) | `IRepositoryBase<>` in `RETAIL.BASE.OBJ.Ports` (port) + `*DA` adapter classes in `RETAIL.BASE.DAT/Adapters/EF` |
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

---

## 9. Target Architecture — Hexagonal (Ports & Adapters)

This section defines the evolution toward a Hexagonal Architecture. **Phase 1 (RETAIL.BASE.DAT) is complete.** Future phases will cover NEG and API layers.

### Concept

Hexagonal Architecture (Ports & Adapters) isolates the application core from infrastructure concerns by defining:

- **Ports** — technology-agnostic interfaces owned by the domain/application core.
- **Adapters** — concrete implementations that plug into those ports (e.g., EF Core, HTTP, SignalR).

The core never depends on a specific technology. Technologies depend on the core.

### Phase 1 — RETAIL.BASE.DAT

#### Current state

```
RETAIL.BASE.DAT
├── Interfaces/
│   └── IRepositoryBase<TEntity, TFilter, TId>   ← lives inside DAT
├── Repositories/
│   └── *DA.cs                                   ← EF Core adapters
└── RETAIL_BASEDbContext.cs
```

The `IRepositoryBase<>` interface currently lives **inside `RETAIL.BASE.DAT`**, which means any layer that wants to depend on it must reference the DAT project — pulling in EF Core transitively. This couples the business logic layer to the persistence technology.

#### Target state

```
RETAIL.BASE.OBJ  (or a new RETAIL.BASE.PORTS project)
└── Ports/
    └── IRepositoryBase<TEntity, TFilter, TId>   ← Port (owned by the domain)

RETAIL.BASE.DAT
└── Adapters/
    ├── EF/
    │   ├── RETAIL_BASEDbContext.cs
    │   └── *DA.cs                               ← EF Core adapter (implements the port)
    └── Migrations/
```

#### Structural rules (Phase 1)

| Rule | Description |
|---|---|
| **Port location** | `IRepositoryBase<>` moves to `RETAIL.BASE.OBJ` (or a dedicated `RETAIL.BASE.PORTS` project). DAT must not own ports. |
| **Adapter location** | All EF Core code (`*DA`, `DbContext`, migrations) stays in `RETAIL.BASE.DAT` under an `Adapters/EF/` folder. |
| **Dependency direction** | `RETAIL.BASE.NEG` depends only on the port interface, not on `RETAIL.BASE.DAT` directly. |
| **No leakage** | `DbContext`, EF types, and SQLite-specific code must not be referenced outside `RETAIL.BASE.DAT`. |
| **DI wiring** | `DependencyInjectionHelper` in `RETAIL.BASE.API` remains the only place where ports are bound to adapters. |

#### Dependency diagram (Phase 1 target)

```
RETAIL.BASE.API
  └── registers IRepositoryBase → *DA   (DI wiring only)

RETAIL.BASE.NEG
  └── depends on IRepositoryBase<>      (port — no DAT reference)

RETAIL.BASE.OBJ
  └── defines IRepositoryBase<>         (port ownership)

RETAIL.BASE.DAT
  └── implements IRepositoryBase<>      (EF Core adapter)
  └── depends on RETAIL.BASE.OBJ
```

#### Files to move (Phase 1)

| Current path | Target path | Action |
|---|---|---|
| `RETAIL.BASE.DAT/Interfaces/IRepositoryBase.cs` | `RETAIL.BASE.OBJ/Ports/IRepositoryBase.cs` | Move + namespace update |
| `RETAIL.BASE.DAT/Interfaces/IRepositoryRead.cs` | `RETAIL.BASE.OBJ/Ports/IRepositoryRead.cs` | Move + namespace update |
| `RETAIL.BASE.DAT/Repositories/*DA.cs` | `RETAIL.BASE.DAT/Adapters/EF/*DA.cs` | Move (folder rename only) |
| `RETAIL.BASE.DAT/RETAIL_BASEDbContext.cs` | `RETAIL.BASE.DAT/Adapters/EF/RETAIL_BASEDbContext.cs` | Move (folder rename only) |

#### What does NOT change in Phase 1

- `RETAIL.BASE.NEG` service code — interface usage is identical; only the `using` namespace changes.
- `RETAIL.BASE.API` controller code — no change required.
- EF Core migrations — remain in `RETAIL.BASE.DAT/Migrations/`.
- `DependencyInjectionHelper` — small `using` update to reference the new namespace.
- Test projects — `using` namespace update only.

#### Acceptance criteria (Phase 1 — COMPLETE ✓)

1. ✓ `RETAIL.BASE.NEG.csproj` does **not** contain a `<ProjectReference>` to `RETAIL.BASE.DAT`.
2. ✓ `IRepositoryBase<>` namespace is `RETAIL.BASE.OBJ.Ports`.
3. ✓ `RETAIL.BASE.DAT` builds and all tests pass (183 tests, 0 failures).
4. ✓ `dotnet build RETAIL.BASE.sln` reports 0 errors.

#### Additional changes made during implementation

- `RETAIL.BASE.API.csproj` gained an explicit `<ProjectReference>` to `RETAIL.BASE.DAT` (previously resolved transitively through NEG). This is architecturally correct: the API is the composition root and is the only place that knows about adapters.
- `RETAIL.BASE.NEG.csproj` gained an explicit `<PackageReference>` to `Microsoft.Extensions.Logging.Abstractions 8.0.2` (previously resolved transitively through DAT → EF Core).
