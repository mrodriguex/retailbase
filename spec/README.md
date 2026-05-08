# RETAIL.BASE — System Specification

## High-Level Summary

RETAIL.BASE is a retail management backend API built on **.NET 8 / ASP.NET Core**. It exposes a versioned REST API (v1 and v2) backed by a **PostgreSQL** database accessed through **Entity Framework Core**. The system covers identity and access management (users, roles, menu items), company/customer master data, and a product catalog (brands, categories, products, and product presentations). Real-time notifications are delivered via a **SignalR** hub.

---

## How the System is Structured

The solution (`RETAIL.BASE.sln`) is composed of six C# projects plus two test projects:

| Project | Role |
|---|---|
| `RETAIL.BASE.OBJ` | Shared entities, models, enums, and DTOs |
| `RETAIL.BASE.DAT` | Data access layer — EF Core DbContext, repositories, migrations |
| `RETAIL.BASE.NEG` | Business logic layer — services and cryptography helpers |
| `RETAIL.BASE.API` | ASP.NET Core Web API — controllers, program bootstrap, Swagger |
| `RETAIL.BASE.DAT.Tests` | Unit tests for the data access layer (in-memory DB) |
| `RETAIL.BASE.NEG.Tests` | Unit tests for the business logic layer |
| `RETAIL.BASE.WEB` | Front-end (Vite/Vue — not covered in this spec) |

Dependency direction:
```
RETAIL.BASE.API → RETAIL.BASE.NEG → RETAIL.BASE.DAT → RETAIL.BASE.OBJ
```

---

## Guide for New Developers

1. **Prerequisites** — .NET 8 SDK, PostgreSQL server, Node.js + npm (for front-end).
2. **Clone**
   ```bash
   git clone https://github.com/mrodriguex/RETAIL.BASE.git
   cd RETAIL.BASE
   ```
3. **Configuration** — Connection string and JWT settings are in `RETAIL.BASE.API/appsettings.json`. Update `ConnectionStrings:SqlConn_RETAIL_BASE` and `Jwt:Key` for your environment.
4. **Database setup** — Apply EF migrations:
   ```bash
   dotnet ef database update --project RETAIL.BASE.DAT --startup-project RETAIL.BASE.API
   ```
5. **Build**
   ```bash
   dotnet build RETAIL.BASE.sln
   ```
6. **Run API**
   ```bash
   dotnet run --project RETAIL.BASE.API/RETAIL.BASE.API.csproj
   ```
   Swagger UI is available at `/swagger`.
7. **Run front-end**
   ```bash
   cd RETAIL.BASE.WEB
   npm install
   npm run dev
   ```
8. **Authentication** — Call `POST /api/v1/Auth/login` with `{ "username": "...", "password": "..." }` to obtain a JWT. Include it as `Authorization: Bearer <token>` on subsequent requests.
9. **Run tests**
   ```bash
   dotnet test RETAIL.BASE.sln
   ```
   Or run each project individually:
   ```bash
   dotnet test RETAIL.BASE.NEG.Tests/RETAIL.BASE.NEG.Tests.csproj
   dotnet test RETAIL.BASE.DAT.Tests/RETAIL.BASE.DAT.Tests.csproj
   ```
10. **CI/CD** — A `Jenkinsfile` defines a Jenkins pipeline that builds, publishes, and deploys via SSH + rsync to a remote Linux server running systemd. See [architecture/architecture.md](architecture/architecture.md) §8 for details.

---

## Spec Document Index

| File | Contents |
|---|---|
| [architecture/architecture.md](architecture/architecture.md) | Layers, dependencies, auth design, configuration, CI/CD |
| [features/features-overview.md](features/features-overview.md) | All implemented features and user flows |
| [api/api-contracts.md](api/api-contracts.md) | All controllers, endpoints, request/response shapes |
| [database/database-spec.md](database/database-spec.md) | Entities, relationships, migrations |
| [standards/coding-standards.md](standards/coding-standards.md) | Naming conventions, patterns, exception handling, contributing |
| [security/security-spec.md](security/security-spec.md) | Auth mechanism, password handling, known risks |
| [testing/testing-spec.md](testing/testing-spec.md) | Test projects, frameworks, patterns, test inventory |
