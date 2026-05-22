# Front-End Auth Guard Implementation Plan

## Overview

Migrate from localStorage-based auth verification to a hybrid model where:
- **Primary security**: Backend validates httpOnly cookie `admin_token` on every API call
- **Front-end guard**: Calls `GET /api/auth/me` on admin route navigation for fast UX
- **Global 401 handler**: Catches unauthorized responses from any API call and redirects to login

## Current State (Issues)

| Issue | Location | Risk |
|-------|----------|------|
| Token stored in `localStorage` | `admin/login/page.tsx:74` | XSS can steal token |
| Auth verified via `localStorage` check | `admin/page.tsx:22-28` | Client-side state can be manipulated |
| No server-side session validation on navigation | `admin/page.tsx` | Token may be expired but client doesn't know |
| Backend sets httpOnly cookie but frontend ignores it | `AuthController.cs:49` | Cookie auth is not used |

## Phased Implementation

### Phase 1: Backend — Auth Verification Endpoint

**Goal**: Ensure `GET /api/auth/me` properly validates the httpOnly cookie and returns user info or 401.

#### Step 1.1: Update AuthController to validate httpOnly cookie
- **File**: `ServerApp/ServerApp.Api/Controllers/AuthController.cs`
- **Change**: Modify `GetCurrentUser()` to extract admin ID from validated JWT cookie instead of `X-Admin-Id` header
- **Logic**:
  ```csharp
  [HttpGet("me")]
  public async Task<ActionResult<AdminUserDto>> GetCurrentUser()
  {
      // Read admin_token cookie
      var token = Request.Cookies["admin_token"];
      if (string.IsNullOrEmpty(token))
          return Unauthorized();
      
      // Validate token via IJwtTokenService
      var principal = _jwtTokenService.ValidateToken(token);
      if (principal == null)
          return Unauthorized();
      
      // Extract admin ID from claims
      var adminId = Guid.Parse(principal.FindFirst("id")?.Value!);
      var result = await _mediator.Send(new GetCurrentUser(adminId));
      return OkOrNotFound(result);
  }
  ```
- **Also**: Remove `X-Admin-Id` header dependency from this endpoint

#### Step 1.2: Create AuthMiddleware for automatic cookie validation
- **File**: `ServerApp/ServerApp.Api/Middleware/AuthMiddleware.cs` (new)
- **Purpose**: Automatically validate `admin_token` cookie on protected routes and populate `HttpContext.Items["AdminId"]`
- **Logic**:
  ```csharp
  public async Task InvokeAsync(HttpContext context)
  {
      // Skip public routes
      var isProtectedRoute = context.Request.Path.StartsWithSegments("/api/paintings")
          || context.Request.Path.StartsWithSegments("/api/pagecontent")
          || context.Request.Path.StartsWithSegments("/api/paintingcategories")
          || context.Request.Path.StartsWithSegments("/api/auth/me")
          || context.Request.Path.StartsWithSegments("/api/auth/");
      
      if (isProtectedRoute && !context.Request.Path.StartsWithSegments("/api/auth/google"))
      {
          var token = context.Request.Cookies["admin_token"];
          if (!string.IsNullOrEmpty(token))
          {
              var principal = _jwtTokenService.ValidateToken(token);
              if (principal != null)
              {
                  var adminId = Guid.Parse(principal.FindFirst("id")?.Value!);
                  context.Items["AdminId"] = adminId;
              }
          }
      }
      
      await _next(context);
  }
  ```
- **Register**: Add to `Program.cs` before `MapControllers()`

### Phase 2: Front-End — Remove localStorage, Use Cookie Auth

**Goal**: Eliminate localStorage token storage; rely on httpOnly cookie for all auth.

#### Step 2.1: Update login page to not store token in localStorage
- **File**: `clientapp/src/app/admin/login/page.tsx`
- **Change**: Remove `localStorage.setItem('admin_token', ...)` and `localStorage.setItem('admin_user', ...)`
- **Keep**: Store only minimal user display info in localStorage (non-sensitive: displayName, pictureUrl) for UI purposes
- **The actual auth token** is already stored as httpOnly cookie by the backend

#### Step 2.2: Create auth verification utility
- **File**: `clientapp/src/lib/auth.ts` (new)
- **Functions**:
  ```typescript
  // Check if user is authenticated by calling /api/auth/me
  export async function verifyAuth(): Promise<AdminUserDto | null> {
      const response = await fetch('/api/auth/me', {
          credentials: 'include',
          cache: 'no-store'
      });
      if (response.status === 401) return null;
      if (!response.ok) throw new Error('Auth verification failed');
      return await response.json();
  }
  
  // Logout by clearing cookie
  export async function logout(): Promise<void> {
      await fetch('/api/auth/logout', {
          method: 'POST',
          credentials: 'include'
      });
      window.location.href = '/admin/login';
  }
  ```

#### Step 2.3: Update admin page to verify via API
- **File**: `clientapp/src/app/admin/page.tsx`
- **Change**: Replace localStorage check with `verifyAuth()` API call
- **Logic**:
  ```tsx
  useEffect(() => {
      verifyAuth().then(user => {
          if (!user) {
              router.push('/admin/login');
          } else {
              setUser(user);
          }
          setLoading(false);
      }).catch(() => {
          router.push('/admin/login');
          setLoading(false);
      });
  }, [router]);
  ```

### Phase 3: Front-End — Global 401 Handler

**Goal**: Any API call that returns 401 should redirect to login.

#### Step 3.1: Add 401 handling to api.ts fetch wrapper
- **File**: `clientapp/src/lib/api.ts`
- **Change**: Create a `fetchWithAuth` wrapper that catches 401 and redirects
- **Logic**:
  ```typescript
  export async function fetchWithAuth(url: string, options?: RequestInit): Promise<Response> {
      const response = await fetch(url, { ...options, credentials: 'include' });
      if (response.status === 401) {
          // Clear any stale session data
          localStorage.removeItem('admin_user');
          // Redirect to login (only on client side)
          if (typeof window !== 'undefined') {
              window.location.href = '/admin/login';
          }
      }
      return response;
  }
  ```
- **Note**: This wrapper is only used for admin mutation endpoints, not public read endpoints

### Phase 4: Front-End — Next.js Middleware for Admin Routes

**Goal**: Block access to `/admin` routes at the edge before page load.

#### Step 4.1: Create Next.js middleware
- **File**: `clientapp/src/middleware.ts` (new)
- **Logic**:
  ```typescript
  import { NextResponse } from 'next/server';
  import type { NextRequest } from 'next/server';
  
  export function middleware(request: NextRequest) {
      const isAdminRoute = request.nextUrl.pathname.startsWith('/admin');
      const isLoginRoute = request.nextUrl.pathname === '/admin/login';
      
      if (isAdminRoute && !isLoginRoute) {
          const token = request.cookies.get('admin_token');
          if (!token) {
              return NextResponse.redirect(new URL('/admin/login', request.url));
          }
      }
      
      return NextResponse.next();
  }
  
  export const config = {
      matcher: ['/admin/:path*'],
  };
  ```
- **Note**: This only checks cookie presence, not validity. The API call in Phase 2 validates the actual token.

### Phase 5: Backend — AdminAuthorizedAttribute Filter

**Goal**: Apply JWT validation as an action filter on mutation endpoints.

#### Step 5.1: Update AdminAuthorizedAttribute
- **File**: `ServerApp/ServerApp.Api/Filters/AdminAuthorizedAttribute.cs`
- **Current**: Validates via `X-Admin-Id` header
- **Change**: Read and validate `admin_token` cookie; populate `HttpContext.Items["AdminId"]`
- **Apply to**: All mutation endpoints (PATCH, POST, DELETE) via `[AdminAuthorized]` attribute

#### Step 5.2: Apply filter to mutation endpoints
- **Controllers**: `PaintingsController`, `PaintingCategoriesController`, `PageContentController`, `AuthController`
- **Change**: Add `[AdminAuthorized]` attribute to all mutation methods
- **Remove**: Manual `X-Admin-Id` header extraction from controller methods

## Implementation Order

```
Phase 1 (Backend auth endpoint)  →  Phase 5 (AdminAuthorized filter)
                                      ↓
Phase 4 (Next.js middleware)     →  Phase 2 (Remove localStorage)
                                      ↓
Phase 3 (Global 401 handler)
```

## Auth Flow After Implementation

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│   Browser     │         │  Next.js     │         │  .NET API    │
│               │         │  Middleware  │         │              │
│ 1. Navigate   │────────▶│ 2. Check     │         │              │
│    /admin     │         │    cookie    │         │              │
│               │◀────────│ 3. Redirect  │         │              │
│ 4. Page loads │         │    if missing│         │              │
│               │         │              │         │              │
│ 5. verifyAuth │────────▶│              │────────▶│ 6. Validate  │
│    () call    │         │              │         │    cookie    │
│               │         │              │◀────────│ 7. Return    │
│ 8. Show UI    │◀────────│              │         │    user or   │
│               │         │              │         │    401       │
└──────────────┘         └──────────────┘         └──────────────┘
       │                                                              │
       │ 9. Mutation API call                                        │
       │    with cookie ───────────────────────────────────────────▶ │
       │                                                              │
       │                                                              │
       │ 10. AdminAuthorized validates cookie, executes, returns     │
       │     CommandCompletionResponse ◀───────────────────────────── │
       │                                                              │
```

## Security Improvements

| Before | After |
|--------|-------|
| Token in localStorage (XSS vulnerable) | Token in httpOnly cookie (XSS safe) |
| Auth checked client-side only | Auth verified server-side on every request |
| No 401 handling | Global redirect on 401 |
| No route protection | Middleware blocks unauthenticated access |
| Token validity unknown until API call | Edge middleware + API verification |

## Files to Create

| File | Purpose |
|------|---------|
| `clientapp/src/lib/auth.ts` | Auth verification and logout utilities |
| `clientapp/src/middleware.ts` | Next.js middleware for admin route protection |
| `ServerApp/ServerApp.Api/Middleware/AuthMiddleware.cs` | Automatic cookie validation middleware |

## Files to Modify

| File | Change |
|------|--------|
| `ServerApp/ServerApp.Api/Controllers/AuthController.cs` | Validate cookie in `GetCurrentUser()` |
| `ServerApp/ServerApp.Api/Filters/AdminAuthorizedAttribute.cs` | Read cookie instead of header |
| `ServerApp/ServerApp.Api/Program.cs` | Register AuthMiddleware |
| `clientapp/src/app/admin/login/page.tsx` | Remove localStorage token storage |
| `clientapp/src/app/admin/page.tsx` | Use `verifyAuth()` API call |
| `clientapp/src/lib/api.ts` | Add `fetchWithAuth` wrapper |
| `ServerApp/ServerApp.Api/Controllers/*.cs` | Add `[AdminAuthorized]` to mutations |

## Risk Mitigation

- **Backward compatibility**: Keep localStorage `admin_user` for display-only data (name, picture) during transition
- **Graceful degradation**: If `/api/auth/me` is unavailable, fall back to showing login page
- **Cookie security**: Ensure `Secure`, `HttpOnly`, `SameSite=Strict` flags are set (already done in AuthController)
