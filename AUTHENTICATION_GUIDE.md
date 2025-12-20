# Authentication & Authorization Guide

## Overview

The StartupStarter API implements JWT (JSON Web Token) based authentication with role-based authorization.

## Features Implemented

✅ JWT Token Generation & Validation
✅ Login Endpoint with User Session Tracking
✅ Login Attempt Recording (Success/Failure)
✅ Role-Based Authorization
✅ Swagger Integration with Bearer Token Support
✅ Protected API Endpoints

## Configuration

### JWT Settings

Located in [appsettings.json](src/StartupStarter.Api/appsettings.json):

```json
{
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm",
    "Issuer": "StartupStarter",
    "Audience": "StartupStarter.WebApp",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

**IMPORTANT**: Change the `Secret` value in production to a secure random string!

## Authentication Flow

### 1. Login

**Endpoint**: `POST /api/auth/login`

**Request**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response** (Success):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64encodedrefreshtoken...",
  "userId": "user-123",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "accountId": "account-456",
  "roles": ["SuperAdmin", "ContentManager"],
  "expiresAt": "2025-12-19T15:30:00Z"
}
```

**Response** (Failure):
```json
{
  "message": "Invalid email or password"
}
```

**HTTP Status Codes**:
- `200 OK`: Successful login
- `401 Unauthorized`: Invalid credentials or inactive user

### 2. Using the Access Token

Include the access token in the `Authorization` header for all protected endpoints:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Get Current User Info

**Endpoint**: `GET /api/auth/me`

Requires authentication. Returns the current user's information from the JWT token.

**Response**:
```json
{
  "userId": "user-123",
  "email": "user@example.com",
  "name": "John Doe",
  "accountId": "account-456",
  "roles": ["SuperAdmin", "ContentManager"]
}
```

### 4. Logout

**Endpoint**: `POST /api/auth/logout`

Requires authentication. Ends the current user session.

**Response**:
```json
{
  "message": "Logged out successfully"
}
```

## Authorization

### Available Roles

Defined in [Roles.cs](src/StartupStarter.Api/Authentication/Authorization/Roles.cs):

- `SuperAdmin` - Full system access
- `AccountAdmin` - Account-level administration
- `ContentManager` - Content management access
- `ContentEditor` - Content editing access
- `ContentViewer` - Read-only content access
- `UserManager` - User management access
- `SystemAdmin` - System administration access

### Permission System

Permissions are defined in [Permissions.cs](src/StartupStarter.Api/Authentication/Authorization/Permissions.cs) and organized by feature area:

**Account Management**:
- `accounts:read`
- `accounts:write`
- `accounts:delete`

**User Management**:
- `users:read`
- `users:write`
- `users:delete`

**Content Management**:
- `content:read`
- `content:write`
- `content:publish`
- `content:delete`

*(and more for other features)*

### Protecting Controllers

Controllers can be protected at the class or method level:

#### Class-Level Authorization

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class AccountsController : ControllerBase
{
    // ...
}
```

#### Method-Level Authorization with Roles

```csharp
[HttpPost]
[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AccountAdmin}")]
public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountCommand command)
{
    // Only SuperAdmin or AccountAdmin can access
}
```

#### Mixed Authorization

```csharp
[HttpGet("{id}")]
[Authorize] // Any authenticated user can access
public async Task<ActionResult<AccountDto>> GetAccount(string id)
{
    // ...
}
```

## Security Features

### 1. Login Attempt Tracking

All login attempts (successful and failed) are recorded in the database with:
- Email
- IP Address
- User Agent
- Timestamp
- Success/Failure status
- Failure reason (if applicable)

### 2. User Session Management

User sessions are created upon successful login and tracked with:
- Session ID
- User ID
- Account ID
- IP Address
- User Agent
- Login Method
- Expiration Time
- Active Status

### 3. Password Verification

**NOTE**: The current implementation uses plain text password comparison for simplicity.

**TODO**: Implement proper password hashing using BCrypt or similar:

```csharp
// Instead of:
if (user.PasswordHash != request.Password)

// Use:
if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
```

### 4. User Status Validation

The login flow validates:
- User exists
- Password is correct
- User status is `Active` (not `Inactive`, `Locked`, or `Deleted`)

### 5. Token Validation

JWT tokens are validated on every request for:
- Valid signature
- Valid issuer
- Valid audience
- Not expired
- Valid claims

## Controllers with Authorization

### Currently Protected

- ✅ **AccountsController** - Class-level `[Authorize]`, role-based on POST
- ✅ **UsersController** - Class-level `[Authorize]`, role-based on POST and activate
- ✅ **AuthController** - Login is `[AllowAnonymous]`, others require `[Authorize]`

### To Be Protected

The following controllers should have authorization attributes added:

- ProfilesController
- RolesController
- ContentsController
- DashboardsController
- MediaController
- ApiKeysController
- WebhooksController
- AuditController
- AuthenticationController (existing)
- WorkflowsController
- SystemController

## Swagger Integration

Swagger UI includes JWT Bearer authentication:

1. Click the **Authorize** button in Swagger UI
2. Enter: `Bearer your-jwt-token-here`
3. Click **Authorize**
4. All subsequent requests will include the token

## Testing Authentication

### Using Swagger

1. Start the API: `dotnet run` from `src/StartupStarter.Api`
2. Navigate to: `https://localhost:{port}/swagger`
3. Use the `/api/auth/login` endpoint to get a token
4. Click **Authorize** and paste the token
5. Test protected endpoints

### Using cURL

```bash
# Login
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'

# Use token
curl -X GET https://localhost:7001/api/accounts/account-123 \
  -H "Authorization: Bearer your-jwt-token-here"
```

### Using Postman

1. Create a POST request to `/api/auth/login`
2. Copy the `accessToken` from the response
3. In subsequent requests, go to **Authorization** tab
4. Select **Bearer Token**
5. Paste the access token

## Architecture

### Components

**Authentication**:
- [JwtSettings.cs](src/StartupStarter.Api/Authentication/JwtSettings.cs) - JWT configuration model
- [IJwtTokenService.cs](src/StartupStarter.Api/Authentication/IJwtTokenService.cs) - Token service interface
- [JwtTokenService.cs](src/StartupStarter.Api/Authentication/JwtTokenService.cs) - JWT token generation and validation

**Authorization**:
- [Permissions.cs](src/StartupStarter.Api/Authentication/Authorization/Permissions.cs) - Permission constants
- [Roles.cs](src/StartupStarter.Api/Authentication/Authorization/Roles.cs) - Role constants

**Login Feature**:
- [LoginCommand.cs](src/StartupStarter.Api/Features/AuthManagement/Commands/LoginCommand.cs) - Login request
- [LoginCommandHandler.cs](src/StartupStarter.Api/Features/AuthManagement/Commands/LoginCommandHandler.cs) - Login logic
- [LoginDto.cs](src/StartupStarter.Api/Features/AuthManagement/Dtos/LoginDto.cs) - Login response
- [AuthController.cs](src/StartupStarter.Api/Controllers/AuthController.cs) - Authentication endpoints

### Flow Diagram

```
User Login Request
      ↓
LoginCommandHandler
      ↓
1. Find user by email
2. Verify password
3. Check user status
4. Get user roles
      ↓
JwtTokenService
      ↓
Generate Access Token (with claims)
Generate Refresh Token
      ↓
Create UserSession
Record LoginAttempt
      ↓
Return LoginDto
```

## Next Steps

1. ✅ ~~Implement JWT authentication~~ **COMPLETE**
2. Add password hashing (BCrypt)
3. Add refresh token endpoint
4. Add password reset flow
5. Add MFA (Multi-Factor Authentication)
6. Add rate limiting for login attempts
7. Add account lockout after failed attempts
8. Add authorization policies (instead of just roles)
9. Add claims-based authorization
10. Add API key authentication for external services

## Security Best Practices

1. **Never commit JWT secret to source control** - Use environment variables or Azure Key Vault
2. **Use HTTPS only** - Tokens should never be transmitted over HTTP
3. **Keep tokens short-lived** - Current: 60 minutes
4. **Implement refresh tokens** - Allow token renewal without re-login
5. **Hash passwords** - Never store plain text passwords
6. **Log security events** - All login attempts are logged
7. **Validate all inputs** - Add FluentValidation to commands
8. **Use CORS properly** - Configure allowed origins in production

## Troubleshooting

### 401 Unauthorized

- Check token is included in `Authorization: Bearer {token}` header
- Verify token hasn't expired
- Ensure user is still active
- Check role requirements match user's roles

### Invalid Token

- Verify JWT secret matches between token generation and validation
- Check token hasn't been tampered with
- Ensure issuer and audience match configuration

### Build Errors

If you get compilation errors:
1. Ensure all NuGet packages are restored: `dotnet restore`
2. Rebuild solution: `dotnet build`
3. Check .NET version is 8.0

---

**Status**: Authentication & Authorization Implemented ✅
**Last Updated**: 2025-12-19
**Version**: 1.0.0
