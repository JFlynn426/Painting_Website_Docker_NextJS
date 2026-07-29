# ARM64 Multi-Site Deployment Analysis

**Target Platform:** ARM64 laptop (Snapdragon processor, Windows/Linux)
**Date:** 2026-07-29

## Existing File Structure

### Single-Site Files (Reference)

| File | Purpose |
|------|---------|
| `docker-compose.prod.yml` | Single-site production (x86_64) |
| `docker-compose.arm64.yml` | Single-site ARM64 local/production |
| `.env.example` | Single-site environment template |
| `nginx/nginx.local.conf` | Single-site local NGINX config |
| `nginx/Dockerfile.local` | NGINX Dockerfile with dos2unix for Windows |

### Multi-Site Files (Current - x86_64 Production)

| File | Purpose |
|------|---------|
| `docker-compose.multi.yml` | Multi-site production (x86_64) |
| `docker-compose.multi.local.yml` | Multi-site local development override |
| `.env.multi.example` | Multi-site environment template |
| `nginx/nginx.multi.local.conf` | Multi-site local NGINX config |

## Key Differences: ARM64 Single-Site vs x86_64 Production

Comparing [`docker-compose.arm64.yml`](docker-compose/docker-compose.arm64.yml) vs [`docker-compose.prod.yml`](docker-compose/docker-compose.prod.yml):

| Aspect | `docker-compose.prod.yml` | `docker-compose.arm64.yml` |
|--------|---------------------------|----------------------------|
| **PostgreSQL Password** | Docker secrets (`POSTGRES_PASSWORD_FILE`) | Environment variable (`POSTGRES_PASSWORD`) |
| **Container Names** | `artgallery-*-prod` | `artgallery-*-arm64` |
| **Port Exposure** | Only NGINX ports (80/443/9090) | API (8080) + Frontend (3000) + NGINX ports |
| **NGINX Dockerfile** | `Dockerfile` | `Dockerfile.local` (includes dos2unix) |
| **NGINX Config** | `nginx.conf` | `nginx.local.conf` |
| **SSL Certs Path** | `./nginx/ssl/` | `./nginx/ssl/localhost/` |
| **Read-Only FS** | `read_only: true` on API/Frontend | NOT set (relaxed for local dev) |
| **Resource Limits** | Yes (cpus/memory) | Yes (same limits) |
| **Backup Volume** | `${BACKUP_DIR:-./backups}:/backups:ro` | `${BACKUP_DIR:-./backups}:/backups:ro` |

## ARM64 Multi-Site Requirements

### What Needs to Be Created

#### 1. `docker-compose.multi.arm64.yml` (NEW - Primary File)

This is the **main ARM64 multi-site compose file**. It combines the multi-site architecture from `docker-compose.multi.yml` with the ARM64-specific adjustments from `docker-compose.arm64.yml`.

**Changes from `docker-compose.multi.yml`:**

| Service | Change | Reason |
|---------|--------|--------|
| **postgres** | Container name: `artgallery-postgres-arm64` | Avoid name conflict with x86 deployment |
| **postgres** | Password: env var instead of secrets | Simpler for local ARM64 dev |
| **postgres** | Remove `init-databases.sql` mount | Use env var `POSTGRES_DB` per connection |
| **api-gg** | Container name: `artgallery-api-gg-arm64` | Avoid name conflict |
| **api-gg** | Remove `read_only: true` | Relaxed for local dev |
| **api-flynn** | Container name: `artgallery-api-flynn-arm64` | Avoid name conflict |
| **api-flynn** | Remove `read_only: true` | Relaxed for local dev |
| **frontend-gg** | Container name: `artgallery-frontend-gg-arm64` | Avoid name conflict |
| **frontend-gg** | Remove `read_only: true` | Relaxed for local dev |
| **frontend-gg** | Add `ports: ["3001:3000"]` | Direct access for debugging |
| **frontend-flynn** | Container name: `artgallery-frontend-flynn-arm64` | Avoid name conflict |
| **frontend-flynn** | Remove `read_only: true` | Relaxed for local dev |
| **frontend-flynn** | Add `ports: ["3002:3000"]` | Direct access for debugging |
| **nginx** | Container name: `artgallery-nginx-arm64` | Avoid name conflict |
| **nginx** | Dockerfile: `Dockerfile.local` | Includes dos2unix for Windows |
| **nginx** | Config: `nginx.multi.arm64.local.conf` | ARM64-specific NGINX config |
| **nginx** | SSL path: `./nginx/ssl/localhost/` | Local SSL certs |
| **nginx** | Remove `read_only: true` | Relaxed for local dev |

#### 2. `nginx/nginx.multi.arm64.local.conf` (NEW - NGINX Config)

Based on [`nginx/nginx.multi.local.conf`](docker-compose/nginx/nginx.multi.local.conf) with adjustments for the ARM64 container names. The upstream references remain the same (since service names in compose are the same, only container names change).

**Actually, the NGINX config can reuse `nginx.multi.local.conf`** since NGINX references service names (`api-gg`, `frontend-gg`, etc.) not container names. The only difference would be SSL certificate paths.

#### 3. `.env.multi.arm64.example` (NEW - Environment Template)

Based on [`.env.multi.example`](docker-compose/.env.multi.example) with:
- Added `POSTGRES_PASSWORD` variable (required for ARM64 since no Docker secrets)
- NGINX ports adjusted for local testing (8181/8182/9090)
- Comments noting ARM64-specific usage

### What Can Be Reused

| File | Reuse? | Reason |
|------|--------|--------|
| `nginx/Dockerfile.local` | YES | Already handles ARM64 + Windows CRLF |
| `nginx/nginx.multi.local.conf` | YES | Service names are identical |
| `docker-compose/scripts/init-databases.sql` | NO | Not needed (ARM64 uses env var password) |
| `docker-compose/scripts/backup-multi.sh` | YES | Script is architecture-agnostic |

## Proposed File Structure

```
docker-compose/
├── docker-compose.multi.yml              # Existing - x86_64 production
├── docker-compose.multi.local.yml        # Existing - x86_64 local override
├── docker-compose.multi.arm64.yml        # NEW - ARM64 multi-site (standalone)
├── .env.multi.example                    # Existing - x86_64 production env
├── .env.multi.arm64.example             # NEW - ARM64 multi-site env template
├── nginx/
│   ├── Dockerfile                        # Existing - production NGINX
│   ├── Dockerfile.local                  # Existing - local NGINX (ARM64 compatible)
│   ├── nginx.conf                        # Existing - production NGINX config
│   ├── nginx.multi.local.conf            # Existing - multi-site local config
│   └── ssl/
│       ├── gg/                           # Production SSL for ggpaintings.com
│       ├── flynn/                        # Production SSL for flynnart.com
│       └── localhost/                    # Local SSL certs (for ARM64 dev)
│           ├── gg/
│           │   ├── server.crt
│           │   └── server.key
│           └── flynn/
│               ├── server.crt
│               └── server.key
```

## Usage Commands

### ARM64 Multi-Site Local Development

```bash
# 1. Copy environment template
cd docker-compose
cp .env.multi.arm64.example .env.multi.arm64

# 2. Edit environment file with your values
nano .env.multi.arm64.example

# 3. Generate local SSL certs (if not exists)
powershell -ExecutionPolicy Bypass -File scripts/generate-localhost-ssl.ps1

# 4. Start containers
docker compose -f docker-compose.multi.arm64.yml up -d --build

# Access:
#   - Site gg:     http://localhost:8181
#   - Site flynn:  http://localhost:8182
#   - Health:      http://localhost:9090/health
#   - Frontend gg (direct):  http://localhost:3001
#   - Frontend flynn (direct): http://localhost:3002
```

## Detailed Diff: `docker-compose.multi.yml` → `docker-compose.multi.arm64.yml`

### PostgreSQL Service
```yaml
# BEFORE (multi.yml)
postgres:
  image: postgres:17-alpine
  container_name: artgallery-postgres
  environment:
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD_FILE: /run/secrets/postgres_password
  volumes:
    - postgres_data:/var/lib/postgresql/data
    - ./scripts/init-databases.sql:/docker-entrypoint-initdb.d/init-databases.sql:ro
  secrets:
    - postgres_password

# AFTER (multi.arm64.yml)
postgres:
  image: postgres:17-alpine
  container_name: artgallery-postgres-arm64
  environment:
    POSTGRES_DB: artgallery_gg  # First database created automatically
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
  volumes:
    - postgres_data:/var/lib/postgresql/data
    - ${BACKUP_DIR:-./backups}:/backups:ro
```

### API-GG Service
```yaml
# BEFORE (multi.yml)
api-gg:
  container_name: artgallery-api-gg
  read_only: true
  # ... rest same

# AFTER (multi.arm64.yml)
api-gg:
  container_name: artgallery-api-gg-arm64
  # read_only removed for local dev
  # ... rest same
```

### Frontend-GG Service
```yaml
# BEFORE (multi.yml)
frontend-gg:
  container_name: artgallery-frontend-gg
  read_only: true
  # No ports exposed

# AFTER (multi.arm64.yml)
frontend-gg:
  container_name: artgallery-frontend-gg-arm64
  # read_only removed
  ports:
    - "3001:3000"  # Direct access for debugging
```

### NGINX Service
```yaml
# BEFORE (multi.yml)
nginx:
  container_name: artgallery-nginx
  build:
    context: ./nginx
    dockerfile: Dockerfile
  volumes:
    - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    - ./nginx/ssl/gg:/etc/nginx/ssl/gg:ro
    - ./nginx/ssl/flynn:/etc/nginx/ssl/flynn:ro
  read_only: true

# AFTER (multi.arm64.yml)
nginx:
  container_name: artgallery-nginx-arm64
  build:
    context: ./nginx
    dockerfile: Dockerfile.local  # Windows CRLF handling
  volumes:
    - ./nginx/nginx.multi.local.conf:/etc/nginx/nginx.conf:ro
    - ./nginx/ssl/localhost/gg:/etc/nginx/ssl/gg:ro
    - ./nginx/ssl/localhost/flynn:/etc/nginx/ssl/flynn:ro
  # read_only removed for local dev
```

## Environment Variables: `.env.multi.arm64.example`

```env
# ============================================
# PostgreSQL
# ============================================
# ARM64 uses environment variable (not Docker secrets)
POSTGRES_PASSWORD=ChangeMeToASecurePassword123!

# ============================================
# NGINX Ports (Local Development)
# ============================================
NGINX_HTTP_PORT=8181
NGINX_HTTPS_PORT=8182
NGINX_HEALTH_PORT=9090

# ============================================
# Site: gg (ggpaintings.com)
# ============================================
GG_SITE_NAME="Gloria Gronowicz Fine Art"
GG_SITE_DESCRIPTION="Gloria Gronowicz is an oil painter who creates works inspired by nature"
GG_CONTACT_EMAIL="gloriagronowicz@gmail.com"
GG_CONTACT_PHONE="860.670.0799"

GG_CSS_BACKGROUND="#3d3d3d"
GG_CSS_FOREGROUND="#ffffff"
GG_CSS_NAVBAR_FOOTER_BG="#2d2d2d"
GG_CSS_TITLE_COLOR="#66b3ff"
GG_CSS_BUTTON_COLOR="#1e3a8a"

GG_NEXT_PUBLIC_API_URL=/api
GG_SERVER_API_URL=http://api-gg:8080/api

GG_CORS_ALLOWED_ORIGINS=http://localhost:8181

GG_GOOGLE_AUTH_CLIENT_ID=YOUR_GG_CLIENT_ID.apps.googleusercontent.com
GG_GOOGLE_AUTH_CLIENT_SECRET=YOUR_GG_CLIENT_SECRET
GG_GOOGLE_AUTH_REDIRECT_URI=http://localhost:8181/api/auth/google/callback

GG_ADMIN_JWT_SECRET_KEY=CHANGE_THIS_TO_A_RANDOM_SECRET_KEY_GG
GG_ADMIN_JWT_EXPIRY_MINUTES=60
GG_ADMIN_AUTHORIZED_EMAILS=gloriagronowicz@gmail.com

GG_DATABASE_READ_ONLY_MODE=false

# ============================================
# Site: flynn (flynnart.com)
# ============================================
FLYNN_SITE_NAME="Flynn Art Gallery"
FLYNN_SITE_DESCRIPTION="Fine art paintings by Flynn"
FLYNN_CONTACT_EMAIL="flynn@example.com"
FLYNN_CONTACT_PHONE="555.123.4567"

FLYNN_CSS_BACKGROUND="#1a1a2e"
FLYNN_CSS_FOREGROUND="#e0e0e0"
FLYNN_CSS_NAVBAR_FOOTER_BG="#16213e"
FLYNN_CSS_TITLE_COLOR="#e94560"
FLYNN_CSS_BUTTON_COLOR="#0f3460"

FLYNN_NEXT_PUBLIC_API_URL=/api
FLYNN_SERVER_API_URL=http://api-flynn:8080/api

FLYNN_CORS_ALLOWED_ORIGINS=http://localhost:8182

FLYNN_GOOGLE_AUTH_CLIENT_ID=YOUR_FLYNN_CLIENT_ID.apps.googleusercontent.com
FLYNN_GOOGLE_AUTH_CLIENT_SECRET=YOUR_FLYNN_CLIENT_SECRET
FLYNN_GOOGLE_AUTH_REDIRECT_URI=http://localhost:8182/api/auth/google/callback

FLYNN_ADMIN_JWT_SECRET_KEY=CHANGE_THIS_TO_A_RANDOM_SECRET_KEY_FLYNN
FLYNN_ADMIN_JWT_EXPIRY_MINUTES=60
FLYNN_ADMIN_AUTHORIZED_EMAILS=flynn@example.com

FLYNN_DATABASE_READ_ONLY_MODE=false
```

## Database Initialization Consideration

The current `docker-compose.multi.yml` uses `init-databases.sql` to create both `artgallery_gg` and `artgallery_flynn` databases. The ARM64 version simplifies this:

- **PostgreSQL automatically creates** the database specified in `POSTGRES_DB` on first start
- **Second database** (`artgallery_flynn`) will be created by the .NET EF migrations when `api-flynn` starts and connects
- This matches the existing behavior since `AppInitializer` runs database seeding on startup

Alternatively, keep the `init-databases.sql` script mount since it works with both secrets and env var authentication. The script only creates databases and doesn't depend on the authentication method.

**Recommendation:** Keep the `init-databases.sql` mount for reliability. The only change is removing the Docker secrets mechanism.

## Implementation Plan

See the todo list below for the phased implementation approach.
