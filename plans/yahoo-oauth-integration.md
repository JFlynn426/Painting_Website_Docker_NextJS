# Yahoo OAuth2 Integration Plan

## Overview

This document outlines the changes required to add Yahoo OAuth2 authentication as an additional provider alongside the existing Google OAuth2 authentication.

## PLAN REVIEW - UPDATED 2026-08-10

The following section documents discrepancies discovered when cross-referencing this plan against the current codebase, and additional requirements that were missed in the original plan.

### Discrepancy 1: Multi-Site Environment Variables Missing

The original plan mentions adding Yahoo config to `docker-compose/.env.example` and `appsettings.Development.json`. However, the current multi-site architecture uses site-specific environment variables in `docker-compose/.env.multi` (e.g., `GG_GOOGLE_AUTH_CLIENT_ID`, `FLYNN_GOOGLE_AUTH_CLIENT_ID`). The plan must be updated to include:

```env
# For each site in .env.multi and .env.multi.example:
GG_YAHOO_AUTH_CLIENT_ID=your_yahoo_client_id
GG_YAHOO_AUTH_CLIENT_SECRET=your_yahoo_client_secret
GG_YAHOO_AUTH_REDIRECT_URI=https://localhost:8181/admin/login

FLYNN_YAHOO_AUTH_CLIENT_ID=your_yahoo_client_id
FLYNN_YAHOO_AUTH_CLIENT_SECRET=your_yahoo_client_secret
FLYNN_YAHOO_AUTH_REDIRECT_URI=https://localhost:8182/admin/login
```

Additionally, `.env.multi.arm64.example` needs the same variables.

### Discrepancy 2: `appsettings.Development.json` is NOT the Source of Config

The original plan suggests adding Yahoo config to `ServerApp/ServerApp.Api/appsettings.Development.json`. However, the current codebase reads OAuth config from environment variables using `IConfiguration` with colon notation (e.g., `configuration["GoogleAuth:ClientId"]` in [`GoogleAuthService.cs`](ServerApp/ServerApp.Infrastructure/Services/GoogleAuthService.cs:24)). The multi-site Docker Compose files map these environment variables to the backend containers. No `appsettings` changes are needed for the multi-site deployment.

### Discrepancy 3: `AdminUser` Constructor Requires `googleSubjectId` as Non-Nullable

The current [`AdminUser`](ServerApp/ServerApp.Domain/Entities/AdminUser.cs:21) constructor signature is:
```csharp
internal AdminUser(AdminID id, AdminEmail email, AdminName displayName,
    AdminPictureUrl? pictureUrl, AdminGoogleSub googleSubjectId)
```

The `googleSubjectId` parameter is non-nullable. The plan correctly identifies this needs to change, but the constructor body at line 28 directly assigns `GoogleSubjectId = googleSubjectId;`. Making this nullable requires updating both the parameter type AND handling the null assignment.

### Discrepancy 4: Unique Index on `GoogleSubjectId` with Nullable Column

The current EF configuration at [`AdminUserConfiguration.cs:59`](ServerApp/ServerApp.Infrastructure/EF/Config/AdminUserConfiguration.cs:59) creates a unique index:
```csharp
builder.HasIndex(e => e.GoogleSubjectId).IsUnique();
```

When making `GoogleSubjectId` nullable in PostgreSQL, a unique index allows only ONE null value. This means only ONE Yahoo user could be created. The unique index must be changed to a **partial index** that excludes null values:
```csharp
builder.HasIndex(e => e.GoogleSubjectId)
    .HasDatabaseName("IX_AdminUsers_GoogleSubjectId")
    .IsUnique()
    .HasFilter("GoogleSubjectId IS NOT NULL");
```

Alternatively, use a filtered index via migration SQL.

### Discrepancy 5: `AdminUserFactory.Create` Method Signature

The current [`IAdminUserFactory`](ServerApp/ServerApp.Domain/Factories/IAdminUserFactory.cs:8) and [`AdminUserFactory`](ServerApp/ServerApp.Domain/Factories/AdminUserFactory.cs:8) use a single `Create` method. The plan suggests making `googleSubjectId` optional and adding `yahooGuid`. However, this creates ambiguity: a Yahoo-only login would pass `googleSubjectId = null` and `yahooGuid = "xxx"`. The factory needs to handle BOTH provider IDs being potentially null, with validation that at least ONE is provided.

### Discrepancy 6: `LoginWithYahoo` Command Return Type

The plan suggests `LoginWithYahoo` should return `GoogleAuthResponse`. While technically correct (the response structure is generic), this is semantically confusing. Consider creating a provider-generic `OAuthResponse` DTO or renaming `GoogleAuthResponse` to `AuthResponse`.

### Discrepancy 7: Frontend Login Page Needs Significant Updates

The current [`login/page.tsx`](clientapp/src/app/(admin)/admin/login/page.tsx:20) has:
- Google-specific `handleGoogleLogin()` function
- Google-specific callback handling in `useEffect`
- Google-branded button with inline SVG icon

The plan mentions adding a Yahoo button but doesn't detail the refactoring needed to support multiple providers cleanly. Consider:
- Extracting provider-agnostic callback handling
- Adding a Yahoo button with appropriate branding
- Updating the subtitle text from "Sign in with Google" to "Sign in to access the admin panel"

### Discrepancy 8: `GoogleAuthRequest` DTO Reuse

The plan mentions reusing `GoogleAuthRequest` for Yahoo callback. The current DTO at [`GoogleAuthRequest.cs`](ServerApp/ServerApp.Application/DTOs/GoogleAuthRequest.cs:3) is named specifically for Google. Consider renaming to `OAuthRequest` for clarity.

## CRITICAL ISSUES IDENTIFIED FROM AI RESPONSE REVIEW

The following issues were discovered when cross-referencing the AI's suggested approach against the actual codebase:

### Issue 1: AI Assumes ASP.NET Core Built-in Authentication Middleware - **WRONG**

The AI response suggests using `AddOAuth()` or `AspNet.Security.OAuth.Yahoo` NuGet package. **This project does NOT use ASP.NET Core's built-in authentication middleware at all.** The search for `AddAuthentication`, `AddGoogle`, or `AddOAuth` in the codebase returns zero results. Instead, this project implements a **custom OAuth flow** using:
- Manual HTTP calls via `HttpClient` in [`GoogleAuthService`](ServerApp/ServerApp.Infrastructure/Services/GoogleAuthService.cs:10)
- MediatR CQRS pattern for command handling
- Custom JWT token generation via [`JwtTokenService`](ServerApp/ServerApp.Infrastructure/Services/JwtTokenService.cs:13)

**Impact:** The AI's suggested approach with `AddOAuth()` is completely irrelevant. The Yahoo implementation must follow the existing custom pattern.

### Issue 2: AI Assumes NextAuth.js/Auth.js - **WRONG**

The AI suggests using `next-auth/providers/yahoo`. **This project does NOT use NextAuth.js.** The search for `next-auth`, `NextAuth`, or `@auth` returns zero results. Instead, the frontend uses:
- Custom Next.js API routes in [`/clientapp/src/app/api/auth/`](clientapp/src/app/api/auth/) that proxy to the .NET backend
- Manual `fetch()` calls in the login page
- httpOnly cookies managed by the backend

**Impact:** The Yahoo frontend implementation must follow the existing custom proxy pattern, not NextAuth.

### Issue 3: CRITICAL - `GoogleSubjectId` is NON-NULLABLE

The current [`AdminUser`](ServerApp/ServerApp.Domain/Entities/AdminUser.cs:12) entity has `GoogleSubjectId` configured as **required** (`.IsRequired()` in [`AdminUserConfiguration`](ServerApp/ServerApp.Infrastructure/EF/Config/AdminUserConfiguration.cs:55)). The initial migration creates it as `nullable: false`. **Yahoo-only users cannot be created without modifying this constraint.**

**Impact:** The `GoogleSubjectId` column MUST be made nullable before Yahoo users can be added. This is a **breaking database change** that requires a migration.

### Issue 4: Yahoo Userinfo Endpoint Discrepancy

The AI claims Yahoo supports OpenID Connect at `https://api.login.yahoo.com/openidconnect/v1/userinfo`. **This is misleading.** Yahoo's OAuth2 flow works differently:
- The token response contains a `guid` field
- User profile is fetched via: `https://social.yahooapis.com/v1/user/{guid}/profile?format=json`
- The original plan's endpoint was correct; the AI's OIDC endpoint may not be reliable

### Issue 5: Yahoo HTTPS Redirect URI Requirement

The AI correctly notes that **Yahoo requires HTTPS redirect URIs**, even in development. The current `.env.example` shows Google redirect URIs using `http://localhost` patterns. Yahoo will require either:
- A tunneling solution (ngrok, cloudflared) for local development
- Or Yahoo's local testing exception (if available)

### Issue 6: Refresh Token Handling Not Applicable

The AI discusses refresh token storage and a "provider column" pattern. **This project does NOT store or use refresh tokens.** The current implementation:
- Exchanges the OAuth code for an access token
- Fetches the user profile once
- Generates a JWT token
- Does NOT persist the OAuth access token or refresh token

**Impact:** No refresh token infrastructure is needed. The AI's discussion of this is irrelevant.

## Current Architecture

The existing Google OAuth flow follows a clean layered architecture with **custom OAuth implementation** (no built-in middleware):

```mermaid
sequenceDiagram
    participant Frontend as Frontend (Next.js)
    participant ApiRoute as Next.js API Route
    participant Api as AuthController
    participant Mediator as MediatR
    participant Handler as LoginWithGoogleHandler
    participant Google as IGoogleAuthService
    participant Repo as AdminUserRepository
    participant Jwt as IJwtTokenService

    Frontend->>ApiRoute: GET /api/auth/google/url
    ApiRoute->>Api: GET /api/auth/google/url
    Api->>Google: GetAuthorizationUrl()
    Google-->>Api: Authorization URL
    Api-->>ApiRoute: { url: ... }
    ApiRoute-->>Frontend: { url: ... }
    Frontend->>Frontend: Redirect to Google
    Frontend->>Frontend: Google returns code+state
    Frontend->>ApiRoute: POST /api/auth/google/callback
    ApiRoute->>Api: POST /api/auth/google/callback
    Api->>Mediator: Send LoginWithGoogle command
    Mediator->>Handler: Handle(command)
    Handler->>Google: ExchangeCodeForUserProfileAsync(code)
    Google-->>Handler: GoogleUserProfile
    Handler->>Repo: GetByEmailAsync(email)
    Repo-->>Handler: Existing user or null
    Handler->>Handler: Create/Update admin user
    Handler->>Jwt: GenerateToken(adminUser)
    Jwt-->>Handler: JWT token
    Handler-->>Mediator: GoogleAuthResponse
    Mediator-->>Api: GoogleAuthResponse
    Api->>Api: Set httpOnly cookie
    Api-->>ApiRoute: GoogleAuthResponse
    ApiRoute->>ApiRoute: Set httpOnly cookie
    ApiRoute-->>Frontend: Response with cookie
```

### Key Components

| Layer | Component | Purpose |
|-------|-----------|---------|
| Domain | [`AdminUser`](ServerApp/ServerApp.Domain/Entities/AdminUser.cs:7) | Entity with `AdminGoogleSub` value object (**currently non-nullable**) |
| Domain | [`AdminGoogleSub`](ServerApp/ServerApp.Domain/ValueObjects/Admin/AdminGoogleSub.cs:5) | Value object for Google subject ID |
| Domain | [`IAdminUserFactory`](ServerApp/ServerApp.Domain/Factories/IAdminUserFactory.cs:6) | Factory with Google-specific `AdminGoogleSub` parameter |
| Application | [`IGoogleAuthService`](ServerApp/ServerApp.Application/Services/IGoogleAuthService.cs:3) | Interface with `GetAuthorizationUrl()` and `ExchangeCodeForUserProfileAsync()` |
| Application | [`LoginWithGoogle`](ServerApp/ServerApp.Application/Commands/LoginWithGoogle.cs:6) | Command record |
| Application | [`LoginWithGoogleHandler`](ServerApp/ServerApp.Application/Commands/Handlers/LoginWithGoogleHandler.cs:15) | Handler with email authorization check |
| Application | [`GoogleAuthRequest`](ServerApp/ServerApp.Application/DTOs/GoogleAuthRequest.cs:3) / [`GoogleAuthResponse`](ServerApp/ServerApp.Application/DTOs/GoogleAuthResponse.cs:3) | DTOs |
| Infrastructure | [`GoogleAuthService`](ServerApp/ServerApp.Infrastructure/Services/GoogleAuthService.cs:10) | **Custom** implementation with Google-specific URLs (no middleware) |
| Infrastructure | [`JwtTokenService`](ServerApp/ServerApp.Infrastructure/Services/JwtTokenService.cs:13) | Custom JWT generation (no ASP.NET auth middleware) |
| API | [`AuthController`](ServerApp/ServerApp.Api/Controllers/AuthController.cs:15) | Endpoints for `/google/url` and `/google/callback` |
| Frontend | [`login/page.tsx`](clientapp/src/app/(admin)/admin/login/page.tsx:20) | Login page with Google button |
| Frontend | [`route.ts`](clientapp/src/app/api/auth/google/callback/route.ts:7) | Next.js API route proxying to backend |

## Design Approach

**Recommended: Parallel Implementation Pattern**

Rather than abstracting the existing Google implementation, we will add Yahoo as a parallel implementation. This approach:
- Minimizes risk to existing working code
- Follows the existing codebase patterns
- Keeps Google and Yahoo implementations independent
- Allows future abstraction if a third provider is needed

## Yahoo OAuth2 Endpoints

| Purpose | URL |
|---------|-----|
| Authorization | `https://api.login.yahoo.com/oauth2/request_auth` |
| Token Exchange | `https://api.login.yahoo.com/oauth2/get_token` |
| User Info | `https://social.yahooapis.com/v1/user/{guid}/profile?format=json` |

Yahoo returns a `guid` (unique user identifier) in the token response, which is then used to fetch the user profile. The profile contains `email`, `givenName`, `familyName`, and `image`.

**IMPORTANT:** Yahoo requires HTTPS redirect URIs. For local development, a tunneling solution (ngrok, cloudflared) will be needed.

## Required Changes

### Phase 1: Backend - Domain Layer

#### 1.1 Make `GoogleSubjectId` Nullable - **CRITICAL PREREQUISITE**

**Modify:** [`AdminUser`](ServerApp/ServerApp.Domain/Entities/AdminUser.cs:12)

Change `GoogleSubjectId` from non-nullable to nullable. Currently it is defined as:
```csharp
public AdminGoogleSub GoogleSubjectId { get; private set; } = default!;
```

Change to:
```csharp
public AdminGoogleSub? GoogleSubjectId { get; private set; }
```

**Modify:** [`AdminUserConfiguration`](ServerApp/ServerApp.Infrastructure/EF/Config/AdminUserConfiguration.cs:51)

**CRITICAL - Unique Index with Nullable Column:** The current unique index on `GoogleSubjectId` (line 59) will fail with multiple null values in PostgreSQL. A standard unique index allows only ONE null. This must be changed to a **partial index** that excludes null values:

```csharp
// Remove the old unique index and replace with partial index:
// builder.HasIndex(e => e.GoogleSubjectId).IsUnique();  // REMOVE THIS

// Add partial unique index (allows multiple NULLs, unique for non-NULL values):
builder.HasIndex(e => e.GoogleSubjectId)
    .HasDatabaseName("IX_AdminUsers_GoogleSubjectId")
    .IsUnique()
    .HasFilter("GoogleSubjectId IS NOT NULL");
```

Change from `.IsRequired()` to `.IsRequired(false)` and update the conversion to handle nullable:
```csharp
builder.Property(e => e.GoogleSubjectId)
    .HasColumnName("GoogleSubjectId")
    .HasColumnType("varchar(100)")
    .IsRequired(false)
    .HasConversion(
        new ValueConverter<AdminGoogleSub?, string?>(
            v => v == null ? null : v.Value,
            v => v == null ? null : new AdminGoogleSub(v)));
```

**EF Migration Strategy:** This requires TWO migrations in sequence:
1. **Migration 1:** Drop the existing unique index `IX_AdminUsers_GoogleSubjectId` and create a new partial unique index with `HasFilter("GoogleSubjectId IS NOT NULL")`. Alter column to allow NULL.
2. **Migration 2:** Add `YahooGuid` column as nullable varchar(100) with its own partial unique index.

These can potentially be combined into a single migration, but separating them reduces risk.

#### 1.2 Add `AdminYahooGuid` Value Object

**New file:** `ServerApp/ServerApp.Domain/ValueObjects/Admin/AdminYahooGuid.cs`

```csharp
namespace ServerApp.Domain.ValueObjects.Admin;

using ServerApp.Shared.Domain;

public record AdminYahooGuid : StringValueObject
{
    public const int MaxLength = 100;

    public AdminYahooGuid() : base() { }

    public AdminYahooGuid(string value) : base(value, MaxLength) { }

    public static implicit operator AdminYahooGuid(string yahooGuid) => new(yahooGuid);
}
```

#### 1.3 Update `AdminUser` Entity

**Modify:** [`AdminUser`](ServerApp/ServerApp.Domain/Entities/AdminUser.cs:7)

Add nullable `AdminYahooGuid` property:
```csharp
public AdminYahooGuid? YahooGuid { get; private set; }
```

Update constructor to accept optional provider identifiers. **Note:** The current constructor at line 21 requires `AdminGoogleSub googleSubjectId` as non-nullable. Both parameters must become optional with validation that at least one is provided:

```csharp
internal AdminUser(AdminID id, AdminEmail email, AdminName displayName,
    AdminPictureUrl? pictureUrl, AdminGoogleSub? googleSubjectId = null,
    AdminYahooGuid? yahooGuid = null)
{
    // Validation: at least one provider ID must be provided
    if (googleSubjectId == null && yahooGuid == null)
    {
        throw new ArgumentException("At least one OAuth provider identifier must be provided.");
    }

    Id = id.Value;
    Email = email;
    DisplayName = displayName;
    PictureUrl = pictureUrl;
    GoogleSubjectId = googleSubjectId;
    YahooGuid = yahooGuid;
    CreatedAt = new AdminCreatedAt(DateTime.UtcNow);
    LastLoginAt = new AdminLastLoginAt(DateTime.UtcNow);
    IsActive = new AdminIsActive(true);

    AddEvent(new AdminCreatedEvent(Id, Email.Value));
}
```

#### 1.4 Update `IAdminUserFactory` and `AdminUserFactory`

**Modify:** [`IAdminUserFactory`](ServerApp/ServerApp.Domain/Factories/IAdminUserFactory.cs:6)

Update `Create` method signature. **Important:** The current signature requires `googleSubjectId` as non-nullable. Both parameters become optional:

```csharp
AdminUser Create(
    AdminEmail email,
    AdminName displayName,
    AdminPictureUrl? pictureUrl,
    AdminGoogleSub? googleSubjectId = null,
    AdminYahooGuid? yahooGuid = null);
```

**Modify:** [`AdminUserFactory`](ServerApp/ServerApp.Domain/Factories/AdminUserFactory.cs:6)

Update implementation to pass both provider identifiers to constructor. The factory delegates validation to the entity constructor.

**BREAKING CHANGE NOTE:** The existing `LoginWithGoogleHandler` calls `_factory.Create(email, displayName, pictureUrl, googleSub)` at line 84. This call site must continue to work since `googleSubjectId` now accepts nullable but the Google handler will always pass a non-null value. No changes needed at the call site.

### Phase 2: Backend - Application Layer

#### 2.1 Create `IYahooAuthService` Interface

**New file:** `ServerApp/ServerApp.Application/Services/IYahooAuthService.cs`

```csharp
namespace ServerApp.Application.Services;

public interface IYahooAuthService
{
    string GetAuthorizationUrl();
    Task<YahooUserProfile?> ExchangeCodeForUserProfileAsync(string code);
}

public record YahooUserProfile(
    string Email,
    string DisplayName,
    string? PictureUrl,
    string YahooGuid);
```

#### 2.2 Create `LoginWithYahoo` Command

**New file:** `ServerApp/ServerApp.Application/Commands/LoginWithYahoo.cs`

```csharp
namespace ServerApp.Application.Commands;

using MediatR;
using ServerApp.Application.DTOs;

public record LoginWithYahoo(
    string Code,
    string State
) : IRequest<GoogleAuthResponse>;  // Reuse existing response type
```

#### 2.3 Create `LoginWithYahooHandler`

**New file:** `ServerApp/ServerApp.Application/Commands/Handlers/LoginWithYahooHandler.cs`

Mirrors [`LoginWithGoogleHandler`](ServerApp/ServerApp.Application/Commands/Handlers/LoginWithGoogleHandler.cs:15) but uses `IYahooAuthService` and `YahooGuid`.

Key differences from Google handler:
- Uses `_yahooAuthService` instead of `_googleAuthService`
- Creates `AdminYahooGuid` instead of `AdminGoogleSub`
- Sets `GoogleSubjectId` to null (or empty) for Yahoo users

### Phase 3: Backend - Infrastructure Layer

#### 3.1 Create `YahooAuthService` Implementation

**New file:** `ServerApp/ServerApp.Infrastructure/Services/YahooAuthService.cs`

Implements `IYahooAuthService` with Yahoo-specific OAuth2 flow:

1. `GetAuthorizationUrl()` - Builds Yahoo authorization URL with client_id, redirect_uri, scope, and state
2. `ExchangeCodeForUserProfileAsync(code)`:
   - Exchange code for access token at Yahoo token endpoint
   - Extract `guid` from token response
   - Fetch user profile using GUID
   - Map Yahoo profile fields to `YahooUserProfile` record

#### 3.2 Register Yahoo Service in DI Container

**Modify:** [`InfrastructureExtensions`](ServerApp/ServerApp.Infrastructure/Extensions.cs:59)

Add registration alongside Google service:

```csharp
services.AddScoped<IYahooAuthService, YahooAuthService>();
```

#### 3.3 Add EF Migration for YahooGuid Column

Generate new migration to add nullable `YahooGuid` column to `AdminUsers` table.

**Modify:** [`AdminUserConfiguration`](ServerApp/ServerApp.Infrastructure/EF/Config/AdminUserConfiguration.cs:11)

Add configuration for `YahooGuid` property.

### Phase 4: Backend - API Layer

#### 4.1 Update `AuthController`

**Modify:** [`AuthController`](ServerApp/ServerApp.Api/Controllers/AuthController.cs:15)

Add Yahoo-specific endpoints:

```csharp
[HttpGet("yahoo/url")]
public ActionResult<Dictionary<string, string>> GetYahooAuthorizationUrl()

[HttpPost("yahoo/callback")]
public async Task<ActionResult<GoogleAuthResponse>> LoginWithYahoo([FromBody] GoogleAuthRequest request)
```

Inject `IYahooAuthService` into constructor.

### Phase 5: Frontend

#### 5.1 Update Login Page

**Modify:** [`login/page.tsx`](clientapp/src/app/(admin)/admin/login/page.tsx:20)

Current state:
- Line 37-46: `useEffect` checks URL params for `code` and `state`, calls `handleCallback(code, state)` which posts to `/api/auth/google/callback`
- Line 48-64: `handleGoogleLogin()` fetches `/api/auth/google/url`
- Line 109: Subtitle says "Sign in with Google to access the admin panel"
- Line 117-147: Single Google button with inline SVG icon

Required changes:
1. **Refactor callback handling** - The current `useEffect` at line 37 handles Google callback. Need to detect which provider callback is being processed. Options:
   - Add a `provider` URL param (e.g., `?code=xxx&state=yyy&provider=yahoo`)
   - Or use separate URL paths for each provider callback
2. **Add `handleYahooLogin()`** - Mirrors `handleGoogleLogin()` but calls `/api/auth/yahoo/url`
3. **Add Yahoo button** - Yahoo-branded button (purple `#720e9e` is Yahoo's brand color) with appropriate icon
4. **Update subtitle** - Change from "Sign in with Google" to "Sign in to access the admin panel"
5. **Update page title** - Consider making it provider-agnostic

#### 5.2 Create Yahoo URL API Route

**New file:** `clientapp/src/app/api/auth/yahoo/url/route.ts`

Mirrors [`google/url/route.ts`](clientapp/src/app/api/auth/google/url/route.ts:7) but proxies to `/auth/yahoo/url`.

#### 5.3 Create Yahoo Callback API Route

**New file:** `clientapp/src/app/api/auth/yahoo/callback/route.ts`

Mirrors [`google/callback/route.ts`](clientapp/src/app/api/auth/google/callback/route.ts:7) but proxies to `/auth/yahoo/callback`.

#### 5.4 No Changes to `api.ts` Needed

**NOTE:** The current login page uses direct `fetch()` calls to `/api/auth/google/*` routes, not the `api.ts` service layer. No changes to `api.ts` are needed for Yahoo auth. The API route proxy pattern is sufficient.

### Phase 6: Configuration

#### 6.1 Multi-Site Environment Variables

**IMPORTANT:** The current multi-site architecture uses site-prefixed environment variables (e.g., `GG_GOOGLE_AUTH_CLIENT_ID`, `FLYNN_GOOGLE_AUTH_CLIENT_ID`). The `YahooAuthService` must read config using the same pattern.

**Modify:** `docker-compose/.env.multi` and `docker-compose/.env.multi.example`:

```env
# Site: gg
GG_YAHOO_AUTH_CLIENT_ID=your_yahoo_client_id
GG_YAHOO_AUTH_CLIENT_SECRET=your_yahoo_client_secret
GG_YAHOO_AUTH_REDIRECT_URI=https://localhost:8181/admin/login

# Site: flynn
FLYNN_YAHOO_AUTH_CLIENT_ID=your_yahoo_client_id
FLYNN_YAHOO_AUTH_CLIENT_SECRET=your_yahoo_client_secret
FLYNN_YAHOO_AUTH_REDIRECT_URI=https://localhost:8182/admin/login
```

**Modify:** `docker-compose/.env.multi.arm64.example` - same variables as above.

**Modify:** `docker-compose/.env.example` (single-site):
```env
YAHOO_AUTH_CLIENT_ID=your_yahoo_client_id
YAHOO_AUTH_CLIENT_SECRET=your_yahoo_client_secret
YAHOO_AUTH_REDIRECT_URI=http://localhost:5000/api/auth/yahoo/callback
```

#### 6.2 YahooAuthService Configuration Reading

**CRITICAL:** The current [`GoogleAuthService`](ServerApp/ServerApp.Infrastructure/Services/GoogleAuthService.cs:24) reads config with:
```csharp
_clientId = configuration["GoogleAuth:ClientId"]
```

For multi-site support, the `YahooAuthService` must use the same pattern:
```csharp
_clientId = configuration["YahooAuth:ClientId"]
_clientSecret = configuration["YahooAuth:ClientSecret"]
_redirectUri = configuration["YahooAuth:RedirectUri"]
```

#### 6.3 Docker Compose Environment Variable Mapping

The docker-compose files use the `__` (double underscore) convention to map env vars to nested config keys. The existing Google pattern in `docker-compose.multi.yml` (line 62-64):

```yaml
GoogleAuth__ClientId: ${GG_GOOGLE_AUTH_CLIENT_ID}
GoogleAuth__ClientSecret: ${GG_GOOGLE_AUTH_CLIENT_SECRET}
GoogleAuth__RedirectUri: ${GG_GOOGLE_AUTH_REDIRECT_URI}
```

This must be replicated for Yahoo in **ALL** docker-compose files:

**Files to modify:**
- `docker-compose/docker-compose.multi.yml` - Add to both gg (after line 64) and flynn (after line 167) API service blocks:
  ```yaml
  YahooAuth__ClientId: ${GG_YAHOO_AUTH_CLIENT_ID}
  YahooAuth__ClientSecret: ${GG_YAHOO_AUTH_CLIENT_SECRET}
  YahooAuth__RedirectUri: ${GG_YAHOO_AUTH_REDIRECT_URI}
  ```
- `docker-compose/docker-compose.multi.arm64.yml` - Same pattern for both sites
- `docker-compose/docker-compose.yml` - Single-site version
- `docker-compose/docker-compose.arm64.yml` - Single-site ARM64 version
- `docker-compose/docker-compose.prod.yml` - Production single-site version

#### 6.3 No AppSettings Changes Needed

**NOTE:** Unlike the original plan suggestion, `appsettings.Development.json` does NOT need Yahoo configuration. The multi-site architecture reads all config from environment variables mapped through Docker Compose. The `appsettings` files are only used for VS Code local debugging, which is a secondary concern.

## Files Summary

### New Files
| File | Layer | Description |
|------|-------|-------------|
| `ServerApp/ServerApp.Domain/ValueObjects/Admin/AdminYahooGuid.cs` | Domain | Value object for Yahoo GUID (extends `StringValueObject`) |
| `ServerApp/ServerApp.Application/Services/IYahooAuthService.cs` | Application | Yahoo auth service interface with `YahooUserProfile` record |
| `ServerApp/ServerApp.Application/Commands/LoginWithYahoo.cs` | Application | Yahoo login command |
| `ServerApp/ServerApp.Application/Commands/Handlers/LoginWithYahooHandler.cs` | Application | Yahoo login handler (mirrors `LoginWithGoogleHandler`) |
| `ServerApp/ServerApp.Infrastructure/Services/YahooAuthService.cs` | Infrastructure | Yahoo OAuth2 implementation |
| `clientapp/src/app/api/auth/yahoo/url/route.ts` | Frontend | Next.js API route for Yahoo auth URL |
| `clientapp/src/app/api/auth/yahoo/callback/route.ts` | Frontend | Next.js API route for Yahoo callback |

### Modified Files
| File | Layer | Change |
|------|-------|--------|
| `ServerApp/ServerApp.Domain/Entities/AdminUser.cs` | Domain | Make `GoogleSubjectId` nullable, add `YahooGuid` property, add validation in constructor |
| `ServerApp/ServerApp.Domain/Factories/IAdminUserFactory.cs` | Domain | Make `googleSubjectId` optional, add `yahooGuid` parameter |
| `ServerApp/ServerApp.Domain/Factories/AdminUserFactory.cs` | Domain | Update `Create` method with optional provider IDs |
| `ServerApp/ServerApp.Infrastructure/Extensions.cs` | Infrastructure | Register `IYahooAuthService` |
| `ServerApp/ServerApp.Infrastructure/EF/Config/AdminUserConfiguration.cs` | Infrastructure | Make `GoogleSubjectId` nullable with partial unique index, add `YahooGuid` column config with partial unique index |
| `ServerApp/ServerApp.Api/Controllers/AuthController.cs` | API | Add Yahoo endpoints, inject `IYahooAuthService` |
| `clientapp/src/app/(admin)/admin/login/page.tsx` | Frontend | Add Yahoo login button, refactor callback handling for multiple providers |
| `clientapp/src/lib/api.ts` | Frontend | Add Yahoo API functions (if used; currently login page uses direct fetch) |
| `docker-compose/.env.multi` | Config | Add `GG_YAHOO_AUTH_*` and `FLYNN_YAHOO_AUTH_*` vars |
| `docker-compose/.env.multi.example` | Config | Add Yahoo env vars template |
| `docker-compose/.env.multi.arm64.example` | Config | Add Yahoo env vars template |
| `docker-compose/.env.example` | Config | Add Yahoo env vars for single-site |
| `docker-compose/docker-compose.multi.local.yml` | Config | Map Yahoo env vars to container config keys |

## Risk Assessment

| Risk | Level | Mitigation |
|------|-------|------------|
| Breaking existing Google auth | **Medium** | Making `GoogleSubjectId` nullable affects existing entity. All existing Google users have values, but the factory and handler need careful testing. |
| Unique index on nullable column | **High** | PostgreSQL unique index allows only ONE null value. Must use partial index with `HasFilter("GoogleSubjectId IS NOT NULL")`. Same for `YahooGuid`. |
| Yahoo API changes | Medium | Yahoo OAuth2 is stable; abstracted behind interface |
| Email authorization conflict | Low | Same `_authorizedEmails` list used for both providers |
| Database migration downtime | **Medium** | Two migrations needed: (1) Make `GoogleSubjectId` nullable with partial index, (2) Add `YahooGuid` column with partial index. Both are non-breaking but require careful ordering. |
| Yahoo HTTPS redirect URI | **High** | Yahoo requires HTTPS for redirect URIs. Local development requires tunneling (ngrok/cloudflared). Production nginx already uses HTTPS so no issue there. |
| Multi-site config mapping | **Medium** | Docker Compose must correctly map `GG_YAHOO_AUTH_CLIENT_ID` to container's `YahooAuth:ClientId`. Must follow existing Google pattern exactly. |

## AI Response Evaluation

### What the AI Got RIGHT

1. **Parallel implementation approach** - Correctly identified that Yahoo should be added as a parallel provider, not replacing Google.
2. **Yahoo OAuth2 endpoints** - Correctly identified the authorization and token endpoints.
3. **HTTPS redirect URI requirement** - Correctly flagged that Yahoo requires HTTPS, which is a real constraint.
4. **Scopes differ from Google** - Correctly noted that Yahoo scope names and available data differ.
5. **Account linking logic** - Correctly identified that email-based matching is the key linking strategy.

### What the AI Got WRONG or MISLEADING

1. **Assumed ASP.NET Core Authentication Middleware** - The AI suggested using `AddOAuth()` or `AspNet.Security.OAuth.Yahoo`. This project uses a **custom OAuth implementation** with manual `HttpClient` calls, MediatR CQRS, and custom JWT generation. The AI's middleware-based approach is completely irrelevant.

2. **Assumed NextAuth.js** - The AI suggested using `next-auth/providers/yahoo`. This project uses **custom Next.js API routes** that proxy to the .NET backend, with manual `fetch()` calls and httpOnly cookie management. No NextAuth.js is used.

3. **Missed the `GoogleSubjectId` non-nullable constraint** - The AI did not identify that `GoogleSubjectId` is currently `.IsRequired()` in the EF configuration. This is the **most critical prerequisite** for adding Yahoo users, since Yahoo-only users would have a NULL `GoogleSubjectId`.

4. **Missed the Yahoo URL API route** - The AI only mentioned the callback route. The existing architecture also has a `/api/auth/google/url` route that needs a Yahoo counterpart.

5. **Refresh token discussion is irrelevant** - The AI discussed refresh token storage and a "provider column" pattern. This project does NOT store OAuth refresh tokens at all. The flow is: exchange code → get profile → generate JWT → done.

6. **Userinfo endpoint discrepancy** - The AI mentioned `https://api.login.yahoo.com/openidconnect/v1/userinfo` as the userinfo endpoint. Yahoo's OAuth2 flow actually returns a `guid` in the token response, and the profile is fetched via `https://social.yahooapis.com/v1/user/{guid}/profile?format=json`. The OIDC endpoint may not be reliable.

### What the AI MISSED ENTIRELY

1. **EF Migration requirements** - No mention of the database migrations needed to make `GoogleSubjectId` nullable and add `YahooGuid` column.
2. **Domain layer changes** - No mention of the `AdminUser` entity changes, factory updates, or value object creation.
3. **MediatR command/handler pattern** - No mention of the CQRS pattern used for authentication commands.
4. **Environment variable configuration** - No mention of the `.env.example` and `appsettings.Development.json` updates.
5. **Custom JWT token generation** - No mention of how JWT tokens are generated and validated in this project.

## Yahoo OAuth2 Registration Steps

1. Go to [Yahoo Developer Network](https://developer.yahoo.com/)
2. Create a new app or use existing app
3. Configure OAuth2 permissions for `profile` and `email` scopes
4. Set redirect URI to match your backend callback URL
5. Copy Client ID and Client Secret to environment configuration

## Testing Strategy

1. **Unit Tests:** Test `YahooAuthService` with mocked HTTP client
2. **Integration Tests:** Test full OAuth flow with Yahoo sandbox environment
3. **E2E Tests:** Test login page with both Google and Yahoo buttons
4. **Regression Tests:** Verify Google auth still works after changes

## Future Considerations

If a third provider (e.g., Microsoft, Apple) is added in the future, consider abstracting the auth service into a provider-generic interface:

```csharp
public interface IOAuthService
{
    string ProviderName { get; }
    string GetAuthorizationUrl();
    Task<OAuthUserProfile?> ExchangeCodeForUserProfileAsync(string code);
}

public record OAuthUserProfile(
    string Email,
    string DisplayName,
    string? PictureUrl,
    string ProviderSubjectId,
    string ProviderName);
```

This would allow a single `LoginWithOAuth` command and handler that accepts a provider identifier.
