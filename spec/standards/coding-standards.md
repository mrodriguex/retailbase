# Coding Standards

These conventions are inferred directly from the existing codebase. They represent observed patterns, not aspirational guidelines.

---

## 1. Naming Conventions

### Projects
- Pattern: `RETAIL.BASE.<LAYER>` where `<LAYER>` is `OBJ`, `DAT`, `NEG`, `API`, or `Tests`.
- All uppercase, dot-separated.

### Namespaces
- Mirror project name: `RETAIL.BASE.DAT`, `RETAIL.BASE.NEG.Services`, `RETAIL.BASE.API.Controllers.V1`, etc.
- Sub-namespaces use PascalCase folder names.

### Classes
- PascalCase.
- Repository classes: `<Entity>DA` (e.g., `UserDA`, `ProductDA`).
- Service classes: `<Entity>Service` (e.g., `UserService`, `BrandService`).
- Controller classes: `<Entity>Controller` (e.g., `UserController`, `AuthController`).
- Interface names: `I<Entity>Service`, `I<Entity>Repository` style not followed for repositories; repository interface is generic (`IRepositoryBase`).

### Interfaces
- Prefix `I` + PascalCase: `IAuthService`, `IBrandService`, `IRepositoryBase`, `IServiceBase`.

### Methods
- PascalCase.
- Async methods suffixed with `Async`: `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
- Non-async service helpers use standard PascalCase: `CreateHash`, `CompareHash`, `GenerateJwtToken`.

### Properties and Fields
- Public properties: PascalCase.
- Private backing fields: `_camelCase` prefix with underscore (e.g., `_userName`, `_userRepository`).
- Constants: not observed in codebase (not found).

### HTTP action method parameters
- Query parameters use camelCase in method signatures matching the attribute names (e.g., `idUser`, `pageIndex`, `pageSize`, `enabled`).

---

## 2. Project Layer Conventions

### OBJ (Entities)
- All domain entities inherit from `Base`.
- `Base` provides `Id`, `Name`, `Description`, `Abbreviation`, `Enabled`, `DateTimeCreation`, `DateTimeModification`, `IdUserCreation`, `IdUserModification`, `Order`.
- String properties in entities use backing fields with null-coalescing to return `""` instead of `null`:
  ```csharp
  private string _userName;
  public string UserName { get { _userName ??= ""; return _userName; } set { _userName = value; } }
  ```
- Navigation collection properties are initialized inline: `public List<Role> Roles { get; set; } = new();`

### DAT (Repositories)
- One repository class per entity.
- Each implements `IRepositoryBase<TEntity, BaseFilter, int>`.
- Repositories take `RETAIL_BASEDbContext` and `ILogger<T>` via constructor injection.
- EF Core eager loading used in `GetByIdAsync` via `.Include()` / `.ThenInclude()`.
- Pagination implemented in `GetAllAsync` with `.Skip()` and `.Take()` using `PageIndex` and `PageSize` from `BaseFilter`.
- All data operations are `async` / `await`.
- Attached related entities with `_context.Attach(entity)` before adding/updating to avoid duplicate tracking errors.

### NEG (Services)
- One service class per entity.
- Each implements a specific `I<Entity>Service` interface that extends `IServiceBase<>`.
- Services take the repository interface and `ILogger<T>` via constructor injection.
- All public methods return `ResultModel<T>` — never throw to the caller.
- Audit fields are stamped in services: `IdUserCreation`, `IdUserModification`, `DateTimeCreation`, `DateTimeModification`.
- Authenticated user ID is passed in as `int idUserAutenticado` parameter on write operations.

### API (Controllers)
- All controllers except `AuthController` (and `ConfigController`) extend `BaseController`.
- `BaseController` is `[Authorize]` and `[ApiController]`.
- Individual endpoints can override with `[AllowAnonymous]`.
- Controllers call service methods and respond with `Ok(webResult)` or `BadRequest(webResult)` based on `webResult.Success`.
- Controller methods are all `async Task<IActionResult>`.
- No business logic in controllers.

---

## 3. DTO / Model Usage

- **No separate DTO classes** exist. Domain entities from `RETAIL.BASE.OBJ` are used directly as request and response bodies in all controllers.
- `ResultModel<T>` wraps all responses at the service and controller boundary.
- `PagedResult<T>` wraps all list responses.
- `BaseFilter` is the universal filter/pagination input object for all `GetAll` operations.
- `LoginModel` is the only dedicated input model (username + password).

---

## 4. Exception Handling

- Exceptions are never propagated to controllers.
- Every service method has a `try/catch(Exception ex)` block.
- On exception: `_logger.LogError(ex, "...", ...)` is called, then `webResult.Errors.Add(ex.Message)`, and `webResult.Success = false`.
- The controller maps `Success == false` to `400 BadRequest`.
- No global exception middleware or exception filter is registered.
- `JwtAuthenticateHelper.GetUserIdFromToken` swallows exceptions silently, printing to `Console.WriteLine` and returning `0`.

---

## 5. Logging

- `ILogger<T>` is injected via constructor in all services and repositories.
- `_logger.LogError(ex, "...", structured parameters)` is used exclusively for exception logging.
- `_logger.LogInformation(...)` is used in `CryptographerService` and `HubCommunicationService`.
- `_logger.LogWarning(...)` is used in `HubCommunicationService` for edge cases (empty userId/userName).
- `_logger.LogDebug(...)` is used in `HubCommunicationService` for duplicate connection detection.
- Log messages are written in Spanish in the service layer; English-style structured templates are used in `HubCommunicationService`.
- No centralized logging provider (e.g., Serilog, Application Insights) is configured — default ASP.NET Core console/debug logging is in use.

---

## 6. Async Patterns

- All I/O operations are `async/await` end-to-end.
- `Task<T>` return types used throughout (no `ValueTask`).
- No use of `.Result` or `.Wait()` blocking calls found in codebase.

---

## 7. Dependency Injection

- All DI registration is centralized in `DependencyInjectionHelper.cs` as an extension method on `IServiceCollection`.
- Lifetime: all services and repositories are `Scoped`.
- `IConfiguration` is registered as `Singleton` explicitly (in addition to the default framework registration).

---

## 8. Code Organization

- Regions (`#region`) are used in some services and repositories to group logical sections (e.g., `#region Implementation of IServiceBase`).
- XML documentation comments (`///`) are present on public APIs in some controllers and service interfaces; not consistently applied across the codebase.
- Spanish is used in exception messages, log messages, and some comments. English is used in code identifiers, XML doc comments, and `HubCommunicationService`.
