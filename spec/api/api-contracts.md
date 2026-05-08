# API Contracts

Base URL pattern: `https://<host>/api/v{version}/{controller}`

All versioned endpoints use URL segment versioning (e.g., `/api/v1/`, `/api/v2/`).  
Swagger UI is available at `/swagger`.

---

## Global Response Envelope

All endpoints (except `ConfigController`) return a `ResultModel<T>` envelope:

```json
{
  "statusCode": 0,
  "success": true,
  "data": <T>,
  "message": "...",
  "currentTime": "2026-05-07T00:00:00Z",
  "errors": []
}
```

List endpoints return `ResultModel<PagedResult<T>>`:

```json
{
  "success": true,
  "data": {
    "data": [ ... ],
    "totalCount": 42,
    "pageIndex": 1,
    "pageSize": 50
  }
}
```

HTTP status mapping:
- `resultModel.Success == true` → `200 OK`
- `resultModel.Success == false` → `400 Bad Request`

---

## Authentication Requirement

| Symbol | Meaning |
|---|---|
| 🔒 | Requires valid JWT Bearer token |
| 🔓 | Anonymous — no token required |
| ⚠️ | No `[Authorize]` attribute found on controller |

---

## V1 Endpoints

### Auth — `/api/v1/Auth`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v1/Auth/login` | 🔓 | Authenticate user; returns JWT |

**Request body** (`LoginModel`):
```json
{ "username": "string", "password": "string" }
```
**Response data**: `string` (JWT token)

---

### User — `/api/v1/User`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/User/GetById?idUser={int}` | 🔓 | Get user by ID |
| GET | `/api/v1/User/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List users (paginated) |
| GET | `/api/v1/User/Exists?idUser={int}` | 🔓 | Check if user exists |
| POST | `/api/v1/User/Add` | 🔒 | Create user |
| PUT | `/api/v1/User/Update` | 🔒 | Update user |
| DELETE | `/api/v1/User/Delete?idUser={int}` | 🔒 | Delete user |
| PUT | `/api/v1/User/UpdatePassword` | 🔒 | Change password (body: `LoginModel`) |
| PUT | `/api/v1/User/UnlockUser` | 🔒 | Unlock a locked-out user (body: `int idUser`) |

**User body** (`User` entity):
```json
{
  "id": 0,
  "userName": "string",
  "firstName": "string",
  "lastNameFather": "string",
  "lastNameMother": "string",
  "email": "string",
  "password": "string",
  "avatar": "string",
  "employeeId": 0,
  "attempts": 0,
  "roles": [],
  "companys": [],
  "name": "string",
  "description": "string",
  "abbreviation": "string",
  "enabled": true,
  "order": 0
}
```

---

### Role — `/api/v1/Role`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Role/GetById?idRole={int}` | 🔒 | Get role by ID |
| GET | `/api/v1/Role/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List roles (paginated) |
| POST | `/api/v1/Role/Add` | 🔒 | Create role |
| PUT | `/api/v1/Role/Update` | 🔒 | Update role |
| DELETE | `/api/v1/Role/Delete?idRole={int}` | 🔒 | Delete role |
| GET | `/api/v1/Role/GetUserProfiles?idUser={int}` | 🔒 | Get roles assigned to a user |
| POST | `/api/v1/Role/AssignProfileToUser?idUser={int}&idRole={int}` | 🔒 | Assign a role to a user |
| POST | `/api/v1/Role/RemoveProfileFromUser?idUser={int}&idRole={int}` | 🔒 | Remove a role from a user |

**Role body** (`Role` entity):
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "abbreviation": "string",
  "enabled": true,
  "order": 0,
  "users": [],
  "menuItems": []
}
```

---

### MenuItem — `/api/v1/MenuItem`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/MenuItem/GetById?idMenuItem={int}` | 🔒 | Get menu item by ID |
| GET | `/api/v1/MenuItem/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List menu items (paginated) |
| GET | `/api/v1/MenuItem/GetMenuItemsByUser?idUser={int}&idRole={int}` | 🔒 | Get menu items for a user/role |
| GET | `/api/v1/MenuItem/GetMenuItemsByProfile?idRole={int}` | 🔒 | Get menu items for a role |
| POST | `/api/v1/MenuItem/Add` | 🔒 | Create menu item |
| PUT | `/api/v1/MenuItem/Update` | 🔒 | Update menu item |
| DELETE | `/api/v1/MenuItem/Delete?idMenuItem={int}` | 🔒 | Delete menu item |

**MenuItem body**:
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "abbreviation": "string",
  "image": "string",
  "path": "string",
  "menuItemIdFather": null,
  "enabled": true,
  "order": 0,
  "roles": []
}
```

---

### Company — `/api/v1/Company`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Company/GetById?idCompany={int}` | 🔒 | Get company by ID |
| GET | `/api/v1/Company/GetAll?enabled={bool?}&idRole={int?}&idUser={int?}&pageIndex={int}&pageSize={int}` | 🔒 | List companies (paginated, filterable) |
| POST | `/api/v1/Company/Add` | 🔒 | Create company |
| PUT | `/api/v1/Company/Update` | 🔒 | Update company |
| DELETE | `/api/v1/Company/Delete?idCompany={int}` | 🔒 | Delete company |
| GET | `/api/v1/Company/GetCompaniesByUser?idUser={int}&pageIndex={int}&pageSize={int}` | 🔒 | Get companies assigned to a user |
| POST | `/api/v1/Company/AssignCompanyToUser?idUser={int}&idCompany={int}` | 🔒 | Assign a company to a user |
| POST | `/api/v1/Company/RemoveCompanyFromUser?idUser={int}&idCompany={int}` | 🔒 | Remove a company from a user |

**Company body**:
```json
{
  "id": 0,
  "taxid": "string",
  "legalName": "string",
  "name": "string",
  "description": "string",
  "abbreviation": "string",
  "enabled": true,
  "order": 0,
  "users": []
}
```

---

### Customer — `/api/v1/Customer`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Customer/GetById?idCustomer={int}` | 🔒 | Get customer by ID |
| GET | `/api/v1/Customer/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List customers (paginated) |
| POST | `/api/v1/Customer/Add` | 🔒 | Create customer |
| PUT | `/api/v1/Customer/Update` | 🔒 | Update customer |
| DELETE | `/api/v1/Customer/Delete?idCustomer={int}` | 🔒 | Delete customer |

**Customer body**:
```json
{
  "id": 0,
  "taxid": "string",
  "legalName": "string",
  "idCustomerFather": null,
  "name": "string",
  "description": "string",
  "abbreviation": "string",
  "enabled": true,
  "order": 0
}
```

---

### Brand — `/api/v1/Brand`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Brand/GetById?id={int}` | 🔒 | Get brand by ID |
| GET | `/api/v1/Brand/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List brands (paginated) |
| POST | `/api/v1/Brand/Add` | 🔒 | Create brand |
| PUT | `/api/v1/Brand/Update` | 🔒 | Update brand |
| DELETE | `/api/v1/Brand/Delete?id={int}` | 🔒 | Delete brand |

---

### Category — `/api/v1/Category`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Category/GetById?id={int}` | 🔒 | Get category by ID |
| GET | `/api/v1/Category/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List categories (paginated) |
| POST | `/api/v1/Category/Add` | 🔒 | Create category |
| PUT | `/api/v1/Category/Update` | 🔒 | Update category |
| DELETE | `/api/v1/Category/Delete?id={int}` | 🔒 | Delete category |

---

### Product — `/api/v1/Product`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Product/GetById?id={int}` | 🔒 | Get product by ID |
| GET | `/api/v1/Product/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List products (paginated) |
| POST | `/api/v1/Product/Add` | 🔒 | Create product |
| PUT | `/api/v1/Product/Update` | 🔒 | Update product |
| DELETE | `/api/v1/Product/Delete?id={int}` | 🔒 | Delete product |

**Product body**:
```json
{
  "id": 0,
  "brandId": 0,
  "categoryId": 0,
  "name": "string",
  "description": "string",
  "abbreviation": "string",
  "enabled": true,
  "order": 0,
  "presentations": []
}
```

---

### ProductPresentation — `/api/v1/ProductPresentation`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/ProductPresentation/GetById?id={int}` | 🔒 | Get presentation by ID |
| GET | `/api/v1/ProductPresentation/GetAll?enabled={bool?}&productId={int?}&pageIndex={int}&pageSize={int}` | 🔒 | List presentations (paginated, filterable by product) |
| POST | `/api/v1/ProductPresentation/Add` | 🔒 | Create presentation |
| PUT | `/api/v1/ProductPresentation/Update` | 🔒 | Update presentation |
| DELETE | `/api/v1/ProductPresentation/Delete?id={int}` | 🔒 | Delete presentation |

**ProductPresentation body**:
```json
{
  "id": 0,
  "productId": 0,
  "barcode": "string",
  "sizeLabel": "string",
  "netContent": 0.0,
  "unit": "string",
  "presentation": "string",
  "suggestedPrice": null,
  "costEstimate": null,
  "name": "string",
  "description": "string",
  "enabled": true,
  "order": 0
}
```

---

### Cryptographer — `/api/v1/Cryptographer`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Cryptographer/CreateHash?input={string}` | 🔒 | Compute SHA-512 Base64 hash |
| GET | `/api/v1/Cryptographer/CompareHash?input={string}&hash={string}` | 🔒 | Compare input against stored hash |

**Response data for CreateHash**: `string` (Base64 hash)  
**Response data for CompareHash**: `bool`

---

### Config — `/api/v1/Config`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v1/Config/{key}` | ⚠️ | Read a setting from appsettings.json |
| PUT | `/api/v1/Config/{key}?newValue={string}` | ⚠️ | Write a setting to appsettings.json |

**Note**: No `[Authorize]` attribute is applied to `ConfigController`. Access is unrestricted.

---

## V2 Endpoints

### Auth — `/api/v2/Auth`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/v2/Auth/login` | 🔓 | Generate JWT (no credential validation) |

**Note**: V2 uses a hardcoded signing key embedded in source code. This endpoint does not validate credentials and must not be used in production.

---

### User — `/api/v2/User`

Shares most endpoint signatures with V1 `UserController` but has additional endpoints and no `Delete`.

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/v2/User/GetById?idUser={int}` | 🔓 | Get user by ID |
| GET | `/api/v2/User/GetAll?enabled={bool?}&pageIndex={int}&pageSize={int}` | 🔒 | List users (paginated) |
| GET | `/api/v2/User/Exists?idUser={int}` | 🔓 | Check if user exists |
| POST | `/api/v2/User/Add` | 🔒 | Create user |
| PUT | `/api/v2/User/Update` | 🔒 | Update user |
| POST | `/api/v2/User/AuthenticateUser` | 🔒 | Authenticate a user (V2-only) |
| PUT | `/api/v2/User/UpdatePassword` | 🔒 | Change password (body: `LoginModel`) |
| PUT | `/api/v2/User/UnlockUser` | 🔒 | Unlock a locked-out user |

**Note**: V2 `UserController` does not expose a `Delete` endpoint. V2 adds `AuthenticateUser` which is not present in V1.

---

## SignalR Hub — `/datahub`

Connection parameters: `?userName={string}` in the WebSocket upgrade query.

| Hub Method (server) | Client Event | Description |
|---|---|---|
| `SendUpdate()` | `ReceiveUpdate` | Broadcasts an update signal to all clients |
| `SendMessage(string message)` | `ReceiveMessage` | Broadcasts a message to all clients |
| `SendMessageToUser(string userName, string message)` | `ReceiveMessage` | Sends a message to all connections of a specific user |

---

## Error Handling Pattern

- All service calls are wrapped in `try/catch` at the service layer.
- Exceptions are caught; `ex.Message` is appended to `ResultModel.Errors`.
- `ResultModel.Success` is set to `false`; `ResultModel.Data` is left at its default.
- The controller maps `Success == false` to `400 Bad Request`.
- Unhandled exceptions at the middleware level fall through to the default ASP.NET Core exception handler (`/Error` in non-development environments).
- No global exception middleware is registered in `Program.cs`.
