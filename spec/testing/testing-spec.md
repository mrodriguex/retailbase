# Testing Specification

All content is derived from the actual test projects in the repository.

---

## 1. Test Projects

| Project | Test type | Framework | Target |
|---|---|---|---|
| `RETAIL.BASE.NEG.Tests` | Unit tests — service layer | xUnit + Moq | `net8.0` |
| `RETAIL.BASE.DAT.Tests` | Integration tests — repository layer (in-memory DB) | xUnit + EF InMemory | `net8.0` |

---

## 2. Test Dependencies

### RETAIL.BASE.NEG.Tests

```xml
xunit 2.9.3
xunit.runner.visualstudio 3.1.4
Moq 4.20.72
Microsoft.NET.Test.Sdk 17.14.1
coverlet.collector 6.0.4
```

References: `RETAIL.BASE.NEG`, `RETAIL.BASE.OBJ`

### RETAIL.BASE.DAT.Tests

```xml
xunit 2.9.3
xunit.runner.visualstudio 3.1.4
Microsoft.EntityFrameworkCore.InMemory 8.0.10
Microsoft.Extensions.Configuration 8.0.0
Microsoft.NET.Test.Sdk 17.14.1
coverlet.collector 6.0.4
```

References: `RETAIL.BASE.DAT`, `RETAIL.BASE.OBJ`

---

## 3. Running Tests

Run all tests from the solution root:

```bash
dotnet test RETAIL.BASE.sln
```

Run a specific test project:

```bash
dotnet test RETAIL.BASE.NEG.Tests/RETAIL.BASE.NEG.Tests.csproj
dotnet test RETAIL.BASE.DAT.Tests/RETAIL.BASE.DAT.Tests.csproj
```

Run with coverage collection:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## 4. NEG.Tests — Service Layer Unit Tests

### Strategy
Services are tested in isolation. All dependencies (`IRepositoryBase<>`, `ILogger<>`, `ICryptographerB`, `IConfiguration`) are mocked with **Moq**. No database or file system is involved.

### Test files

| File | Service under test | Test count |
|---|---|---|
| `BrandServiceTests.cs` | `BrandService` | — |
| `CategoryServiceTests.cs` | `CategoryService` | — |
| `CompanyServiceTests.cs` | `CompanyService` | — |
| `ConfigServiceTests.cs` | `ConfigService` | 4 |
| `CryptographerServiceTests.cs` | `CryptographerService` | 8 |
| `CustomerServiceTests.cs` | `CustomerService` | — |
| `MenuItemServiceTests.cs` | `MenuItemService` | — |
| `ProductPresentationServiceTests.cs` | `ProductPresentationService` | — |
| `ProductServiceTests.cs` | `ProductService` | — |
| `RoleServiceTests.cs` | `RoleService` | — |
| `UsuarioServiceTests.cs` | `UserService` | 10 |

The `UNIT_TESTS_SUMMARY.md` file in the repository root records a historical baseline of **73 tests, all passing, with 0 failures**, covering the original service layer (before the product catalog was added).

### Constructor pattern

Every test class follows the same constructor pattern:

```csharp
public BrandServiceTests()
{
    _repositoryMock = new Mock<IRepositoryBase<Brand, BaseFilter, int>>();
    _loggerMock     = new Mock<ILogger<BrandService>>();
    _service        = new BrandService(_loggerMock.Object, _repositoryMock.Object);
}
```

---

## 5. DAT.Tests — Repository Integration Tests

### Strategy
Repositories are tested against the **EF Core InMemory** provider. A fresh in-memory database is created per test (unique `Guid` name) via `TestDataFactory.CreateContext()`, ensuring test isolation.

### Helpers

**`TestRETAIL_BASEDbContext`** — Subclass of `RETAIL_BASEDbContext` that overrides `OnConfiguring` to use `UseInMemoryDatabase`.

```csharp
internal sealed class TestRETAIL_BASEDbContext : RETAIL_BASEDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(_databaseName);
    }
}
```

**`TestDataFactory`** — Static factory with three helpers:
- `CreateContext(string? databaseName)` — creates a fresh in-memory `DbContext`.
- `Logger<T>()` — creates a no-op `ILogger<T>`.
- `Paged(bool? enabled, string name, int pageIndex, int pageSize)` — creates a `BaseFilter` for common filter scenarios.

### Test files

| File | Repository under test |
|---|---|
| `BrandDATests.cs` | `BrandDA` |
| `CategoryDATests.cs` | `CategoryDA` |
| `CompanyDATests.cs` | `CompanyDA` |
| `CustomerDATests.cs` | `CustomerDA` |
| `MenuItemDATests.cs` | `MenuItemDA` |
| `ProductDATests.cs` | `ProductDA` |
| `ProductPresentationDATests.cs` | `ProductPresentationDA` |
| `RoleDATests.cs` | `RoleDA` |
| `UserDATests.cs` | `UserDA` |

---

## 6. Testing Patterns

### Arrange–Act–Assert (AAA)
All tests follow the AAA structure:

```csharp
// Arrange
var brand = new Brand { Name = "Brand A", Enabled = true };
_repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(brand);

// Act
var result = await _service.GetByIdAsync(1);

// Assert
Assert.True(result.Success);
Assert.Equal(brand, result.Data);
```

### Mock verification
Mocks are verified to confirm call count and arguments:

```csharp
_repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
_repositoryMock.Verify(x => x.AddAsync(It.IsAny<Brand>()), Times.Never);
```

### Audit field injection
Verifies that services stamp `IdUserCreation`, `IdUserModification`, `DateTimeCreation`, `DateTimeModification` before delegating to the repository:

```csharp
[Fact]
public async Task Add_WhenBrandIsValid_InjectsAuditFieldsAndReturnsId()
{
    // ...
    var result = await _service.AddAsync(brand, idUserAutenticado);

    Assert.Equal(idUserAutenticado, brand.IdUserCreation);
    Assert.NotEqual(default, brand.DateTimeCreation);
}
```

### Exception handling
Verifies that when a dependency throws, the service returns a failed `ResultModel` instead of propagating:

```csharp
[Fact]
public async Task GetById_WhenBusinessThrows_ReturnsFailure()
{
    _repositoryMock.Setup(x => x.GetByIdAsync(1))
        .ThrowsAsync(new Exception("DB error"));

    var result = await _service.GetByIdAsync(1);

    Assert.False(result.Success);
    Assert.Contains("DB error", result.Errors);
}
```

### Filter propagation
Repository tests verify that filter parameters (`PageIndex`, `PageSize`, `Enabled`, `Name`) are passed through correctly:

```csharp
_repositoryMock.Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
    f.PageIndex == 2 && f.PageSize == 15 && f.Enabled == true)))
    .ReturnsAsync(...);
```

### In-memory persistence (DAT.Tests)
Repository tests write to the in-memory DB and then query back to confirm persistence:

```csharp
var result = await repository.AddAsync(brand);
Assert.True(result > 0);
var saved = await context.Brands.FirstOrDefaultAsync(b => b.Id == result);
Assert.NotNull(saved);
Assert.Equal("Brand A", saved!.Name);
```

---

## 7. What Is Not Tested

- `RETAIL.BASE.API` controllers (no controller-level unit or integration tests found).
- Authentication flow end-to-end.
- SignalR hub behaviour.
- `ConfigController` and `CryptographerController` at the HTTP layer.
- `RETAIL.BASE.WEB` front-end (no test files present).
