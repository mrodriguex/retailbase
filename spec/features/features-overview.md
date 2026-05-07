# Features Overview

All features listed here are based strictly on the existing codebase. No functionality has been assumed or inferred beyond what is directly implemented.

---

## 1. User Authentication

**Description**: System users can authenticate with a username and password. On success a JWT is returned and must be presented on all subsequent API calls.

**Scope**:
- `POST /api/v1/Auth/login` — validates credentials, handles account lockout, returns JWT.
- `POST /api/v2/Auth/login` — returns a JWT without credential validation (non-production endpoint; hardcoded signing key).
- Default administrator account (`administrador`) resolved directly from `appsettings.json` — bypasses database lookup.
- Failed login attempts increment a counter on the user record.
- After **3 consecutive failed attempts** the user account is automatically disabled (`Enabled = false`).

**User flow**:
1. Client sends `{ "username": "...", "password": "..." }` to `/api/v1/Auth/login`.
2. Service checks default user; if not matched, looks up user in database.
3. SHA-512 hash of provided password is compared to stored hash.
4. On success: attempt counter reset, JWT returned.
5. On failure: attempt counter incremented; if ≥ 3, account disabled.

---

## 2. User Management

**Description**: Full CRUD management of system users. Users have roles and can belong to multiple companies.

**Scope**:
- Create, read (by ID, by username), list (paginated, filterable by enabled status), update, delete, exists-check.
- Additional operations: `UnlockUser`, `LockUser` (defined in `IUserService` interface).
- Password is preserved from the existing record on update (cannot be changed through the generic Update endpoint).
- Audit fields (`IdUserCreation`, `IdUserModification`, `DateTimeCreation`, `DateTimeModification`) are set automatically.

**Entities involved**: `User`, `Role`, `Company`

---

## 3. Role Management

**Description**: Roles group users and define which menu items are accessible.

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- Roles carry a list of associated `MenuItem` objects.
- Roles carry a list of associated `User` objects.
- Pagination and enabled-status filtering supported on `GetAll`.

**Entities involved**: `Role`, `User`, `MenuItem`

---

## 4. Menu Item Management

**Description**: Hierarchical navigation menu whose visibility is tied to roles.

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- `GetMenuItemsByUser` — returns menu items accessible to a specific user/role combination.
- Menu items have an optional `MenuItemIdFather` for parent-child hierarchy.
- Each menu item carries an `Image` (path/URL) and a `Path` (navigation route).

**Entities involved**: `MenuItem`, `Role`, `User`

---

## 5. Company Management

**Description**: Management of companies that users can be associated with.

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- `GetAll` accepts optional filters: `enabled`, `idRole`, `idUser`.
- Companies carry a `TAXID` (tax identifier) and `LegalName`.
- Many-to-many association with `User`.

**Entities involved**: `Company`, `User`

---

## 6. Customer Management

**Description**: Management of end customers (clients of the retail operation).

**Scope**:
- Full CRUD (GetAll, GetById, Add, Update, Delete).
- Customers support a self-referential hierarchy via `IdCustomerFather` (nullable).
- Customers carry `TAXID` and `LegalName`.

**Entities involved**: `Customer`

---

## 7. Brand Management

**Description**: Management of product brands.

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- A brand has a collection of associated products.
- Pagination and enabled-status filtering on `GetAll`.

**Entities involved**: `Brand`, `Product`

---

## 8. Category Management

**Description**: Management of product categories.

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- A category has a collection of associated products.

**Entities involved**: `Category`, `Product`

---

## 9. Product Management

**Description**: Management of products, each associated with exactly one brand and one category.

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- Each product has a collection of presentations (SKU-level records).

**Entities involved**: `Product`, `Brand`, `Category`, `ProductPresentation`

---

## 10. Product Presentation Management

**Description**: Management of product SKUs / packaging variants (barcode, size, unit, pricing).

**Scope**:
- Full CRUD (GetById, GetAll, Add, Update, Delete).
- `GetAll` accepts an optional `productId` filter to retrieve presentations for a specific product.
- Fields: `Barcode`, `SizeLabel`, `NetContent`, `Unit`, `Presentation`, `SuggestedPrice`, `CostEstimate`.

**Entities involved**: `ProductPresentation`, `Product`

---

## 11. Cryptography Utility

**Description**: Exposes SHA-512 hashing as an HTTP API endpoint for utility or administrative use.

**Scope**:
- `GET /api/v1/Cryptographer/CreateHash?input=...` — generates a SHA-512 Base64 hash from plain text.
- `GET /api/v1/Cryptographer/CompareHash?input=...&hash=...` — compares plain text against an existing hash.

**Note**: Both endpoints require authentication (`[Authorize]`).

---

## 12. Runtime Configuration Management

**Description**: Allows reading and updating application settings at runtime without restarting the process.

**Scope**:
- `GET /api/v1/Config/{key}` — reads a key from `appsettings.json`.
- `PUT /api/v1/Config/{key}?newValue=...` — writes a new value for a key in `appsettings.json`.
- Nested keys are supported using `:` as separator.

**Note**: `[Authorize]` attribute is **not present** on `ConfigController`; any caller can access these endpoints.

---

## 13. Real-Time Notifications (SignalR)

**Description**: Pushes live events to connected front-end clients via a SignalR WebSocket hub.

**Scope**:
- Hub endpoint: `/datahub`
- On connect: client registers with a `userName` query parameter.
- `SendUpdate` — broadcasts a `ReceiveUpdate` event to all clients.
- `SendMessage(string message)` — broadcasts a `ReceiveMessage` event to all clients.
- `SendMessageToUser(string userName, string message)` — sends a `ReceiveMessage` event to all connections associated with the given username.
- Connection registry is maintained in-memory via `HubCommunicationService` (thread-safe `ConcurrentDictionary`).

---

## 14. Reporting (Commented Out — Not Active)

The `Controllers/RPT/` folder contains several reporting controller files (`ReportsController`, `ViewerController`, `ReportDesignerController`, `ReportsHistoryController`, and composed variants). **All controller code is commented out**. No reporting functionality is active in the codebase.
