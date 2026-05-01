
# RETAIL.BASE

RETAIL.BASE is a modular and extensible framework designed to facilitate rapid development of enterprise-grade applications. It provides a set of reusable components, libraries, and tools that streamline common development tasks and promote best practices.

## Current repository status

This README was regenerated from the current source tree on **2026-04-30**.

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

## Screenshots

### API
![API Swagger UI](docs/assets/images/img_api_swagger.png)

### Login
![Login Screen](docs/assets/images/img_ui_login.png)

### Users Management
![Users Management](docs/assets/images/img_ui_users.png)
![User Details](docs/assets/images/img_ui_users_view.png)
![Edit User](docs/assets/images/img_ui_users_edit.png)
![Change Password](docs/assets/images/img_ui_users_change_pass.png)
![User Roles](docs/assets/images/img_ui_users_roles.png)
![User Companies](docs/assets/images/img_ui_users_companies.png)

### Roles Management
![Roles Management](docs/assets/images/img_ui_roles.png)
![Role Details](docs/assets/images/img_ui_roles_view.png)
![Edit Role](docs/assets/images/img_ui_roles_edit.png)

### Companies Management
![Companies Management](docs/assets/images/img_ui_companies.png)
![Edit Company](docs/assets/images/img_ui_companies_edit.png)

### Customers Management
![Customers Management](docs/assets/images/img_ui_customers.png)
![Edit Customer](docs/assets/images/img_ui_customers_edit.png)

### Brands Management
![Brands Management](docs/assets/images/img_ui_brands.png)
![Edit Brand](docs/assets/images/img_ui_brands_edit.png)

### Categories Management
![Categories Management](docs/assets/images/img_ui_categories.png)
![Edit Category](docs/assets/images/img_ui_categories_edit.png)

### Products Management
![Products Management](docs/assets/images/img_ui_products.png)
![Edit Product](docs/assets/images/img_ui_products_edit.png)

### Product Presentations
![Product Presentations](docs/assets/images/img_ui_product_presentations.png)
![Edit Product Presentation](docs/assets/images/img_ui_product_presentations_edit.png)

### Menus Management
![Menus Management](docs/assets/images/img_ui_menus.png)
![Edit Menu](docs/assets/images/img_ui_manus_edit.png)

### Messages
![Broadcast Messages](docs/assets/images/img_ui_messages_broadcast.png)
![Unicast Messages](docs/assets/images/img_ui_messages_unicast.png)

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
- Node.js and npm (for the web frontend, if developing the UI)

## Configuration

Primary app configuration file:

- `RETAIL.BASE.API/appsettings.json`

Important keys expected by the API startup and DI code:

- `Jwt:Key`
- `ConnectionStrings:DefaultConnection`

> Recommended: keep secrets out of source control and use environment variables / secret management.

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/mrodriguex/RETAIL.BASE.git
cd RETAIL.BASE
```

### Build

Build the solution:

```bash
dotnet build RETAIL.BASE.sln
```

Build API only:

```bash
dotnet build RETAIL.BASE.API/RETAIL.BASE.API.csproj
```

### Run

Run API locally:

```bash
dotnet run --project RETAIL.BASE.API/RETAIL.BASE.API.csproj
```

Swagger UI is enabled by the API pipeline.

For the web frontend:

```bash
cd RETAIL.BASE.WEB
npm install
npm run dev
```

### Test

Run business/service tests:

```bash
dotnet test RETAIL.BASE.NEG.Tests/RETAIL.BASE.NEG.Tests.csproj
```

## API Endpoints

### V1 Endpoints

- **Auth**: `POST /api/v1/auth/login`
- **User**: `GET /api/v1/user`, `POST /api/v1/user`, `PUT /api/v1/user/{id}`, `DELETE /api/v1/user/{id}`
- **Customer**: `GET /api/v1/customer`, `POST /api/v1/customer`, `PUT /api/v1/customer/{id}`, `DELETE /api/v1/customer/{id}`
- **Company**: `GET /api/v1/company`, `POST /api/v1/company`, `PUT /api/v1/company/{id}`, `DELETE /api/v1/company/{id}`
- **Role**: `GET /api/v1/role`, `POST /api/v1/role`, `PUT /api/v1/role/{id}`, `DELETE /api/v1/role/{id}`
- **MenuItem**: `GET /api/v1/menuitem`
- **Config**: `GET /api/v1/config`
- **Cryptographer**: `POST /api/v1/cryptographer/encrypt`, `POST /api/v1/cryptographer/decrypt`
- **Reports**: `GET /api/v1/rpt/...` (various report endpoints)

### V2 Endpoints

- **Auth**: `POST /api/v2/auth/login`
- **User**: `GET /api/v2/user`, `POST /api/v2/user`, `PUT /api/v2/user/{id}`, `DELETE /api/v2/user/{id}`

## CI/CD

Current `Jenkinsfile` pipeline:

1. Checkout from GitHub (`main`)
2. Publish `RETAIL.BASE.API` for `linux-x64`
3. Deploy via SSH + `rsync`
4. Restart and verify systemd service `RETAIL.BASE.API`

## Contributing

We welcome contributions! To contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Create a new Pull Request

Please ensure:
- Code follows the existing style
- All tests pass
- New features include tests
- Documentation is updated

## License

See `LICENSE`.
