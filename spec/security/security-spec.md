# Security Specification

All findings are based strictly on the existing source code. Issues marked **[RISK]** represent observable security concerns in the codebase.

---

## 1. Authentication Mechanism

### Protocol
JWT Bearer authentication using `Microsoft.AspNetCore.Authentication.JwtBearer`. Tokens are transmitted in the `Authorization: Bearer <token>` HTTP header.

### Token validation (V1 — production path)
```
ValidateIssuerSigningKey = true
IssuerSigningKey = SymmetricSecurityKey(Encoding.ASCII.GetBytes(Jwt:Key))
ValidateIssuer = false      ← [RISK] issuer not validated
ValidateAudience = false    ← [RISK] audience not validated
```
The signing key is read from `appsettings.json` (`Jwt:Key`). Key absence throws `InvalidOperationException` at startup.

### Token generation (V1)
- Algorithm: `HmacSha256`.
- Claims: single claim `ClaimTypes.Name = idUser.ToString()` (numeric user ID).
- Expiry: configurable via `Jwt:Duration` (minutes), default 60.
- Signing key: read from `IConfiguration["Jwt:Key"]`.
- Token is generated in `JwtAuthenticateHelper.GenerateJwtToken()`.

### Token generation (V2) — **[RISK]**
- Signing key is **hardcoded** in source: `"Cryoinfra_SDL_3d80b5da-824b-4dde-b1db-3942c6d3d9fc"`.
- No credential validation — any username/password is accepted.
- Tokens expire in 15 minutes.
- This endpoint (`POST /api/v2/Auth/login`) must be considered insecure for production.

---

## 2. Password Handling

### Algorithm
SHA-512 via `System.Security.Cryptography.SHA512.Create()`, implemented in `CryptographerSHA512B`.

### Storage format
Passwords are stored as **Base64-encoded SHA-512 hashes** (`Convert.ToBase64String(hashBytes)`).

### Input encoding
UTF-8 byte encoding of the plain-text password before hashing.

### Comparison
Case-insensitive string equality between the computed Base64 hash and the stored value (`StringComparison.OrdinalIgnoreCase`).

### **[RISK] No salt**
The SHA-512 implementation hashes the raw password without a salt. This makes stored passwords vulnerable to precomputed rainbow-table attacks. The file `CryptographerSHA512B.cs` contains a commented-out `VerifyPassword` method that includes a 16-byte salt extraction, but it is **not used anywhere in the active codebase**.

### **[RISK] No password complexity enforcement**
No minimum length, complexity, or policy checks are applied before storing or comparing passwords.

---

## 3. Default Administrator Account

A default administrator account is configured directly in `appsettings.json`:

```json
"DefaultUser": "administrador",
"DefaultPassword": "Default.123@"
```

**[RISK]** The default password is stored in plain text in configuration. This account bypasses database lookup and the SHA-512 comparison. If `appsettings.json` is exposed or committed to version control without secret management, this credential is compromised.

---

## 4. Token / Session Management

- JWTs are stateless; there is no server-side session or token revocation mechanism.
- No refresh token flow is implemented.
- **[RISK]** A valid token remains usable until its expiry time even if the user is disabled or deleted from the database after issuance.
- The `IdUserAutenticado` property in `BaseController` re-parses the JWT on every request rather than reading from `HttpContext.User.Claims`. This bypasses ASP.NET Core's claim principal and is not affected by middleware-level token validation caching.

---

## 5. Account Lockout

- Failed login attempts are tracked in the `User.Attempts` column.
- After **3 consecutive failures**, `User.Enabled` is set to `false`, preventing further logins.
- Unlocking is performed via `IUserService.UnlockUserAsync()` (interface defined, implementation exists in `UserService`).
- **[RISK]** The lockout check is not atomic with the attempt increment (two separate database writes: increment, then check and disable). Under concurrent requests this could allow more than 3 attempts before lockout.

---

## 6. Authorization

- The `[Authorize]` attribute on `BaseController` requires a valid JWT for all inheriting controllers.
- No role-based or policy-based authorization is applied at the HTTP middleware level.
- **[RISK]** Any user with a valid JWT — regardless of role — can call any authorized endpoint.

---

## 7. CORS Policy

```csharp
builder.AllowAnyOrigin()
       .AllowAnyHeader()
       .AllowAnyMethod();
```

**[RISK]** The CORS policy is fully permissive (`AllowAnyOrigin`). The commented-out `builder.WithOrigins(...)` shows intent to restrict origins but it is not active.

**Note**: `AllowAnyOrigin()` is incompatible with `AllowCredentials()`. Since credentials are not used in CORS (JWT is sent in the header), this is functionally acceptable but still represents an overly broad policy for production.

---

## 8. HTTPS / Transport Security

- `UseHsts()` is enabled only in non-development environments.
- `UseHttpsRedirection()` is **not called** in `Program.cs`.
- **[RISK]** HTTP requests are accepted without automatic upgrade to HTTPS. Tokens and credentials could be transmitted in plain text if clients connect via HTTP.

---

## 9. Runtime Configuration Endpoint

`ConfigController` exposes `GET` and `PUT` endpoints to read and modify `appsettings.json` at runtime.

**[RISK]** No `[Authorize]` attribute is applied to `ConfigController`. Any unauthenticated caller can:
- Read sensitive keys (JWT signing key, database connection string, default credentials).
- Overwrite any configuration key, including the JWT signing key, potentially enabling token forgery.

---

## 10. Sensitive Data in Configuration

The following sensitive values are stored in `appsettings.json` without environment-variable substitution or a secrets manager:

| Key | Value type |
|---|---|
| `ConnectionStrings:SqlConn_RETAIL_BASE` | Database connection string with credentials |
| `Jwt:Key` | JWT symmetric signing key |
| `DefaultPassword` | Administrator password in plain text |

**[RISK]** If `appsettings.json` is committed to a public repository or exposed through the Config API, all of these values are compromised.

---

## 11. SignalR Hub Security

- `DataHub` does not carry an `[Authorize]` attribute.
- Any client that can reach `/datahub` can connect and receive all broadcasted messages.
- The `userName` used for connection registration is taken from the query string without validation or sanitization.

---

## 12. Not Found in Codebase

- No rate limiting on authentication or any other endpoint.
- No anti-CSRF measures (not applicable for JWT-based stateless APIs, but noted).
- No input validation beyond `[Required]` on select query parameters.
- No output sanitization or content-type enforcement beyond the framework defaults.
- No audit log for write operations (despite `Entidad`, `Evento`, `LogEvento`, `TipoEvento` enums being defined in `RETAIL.BASE.OBJ/Enums` — they are defined but not used in any active service).
