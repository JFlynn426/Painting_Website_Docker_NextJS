# Multi-Site Implementation Plan

**Goal:** Implement multi-site architecture so two sites can run simultaneously on localhost and production, each with independent databases, configurations, and URLs.

**Target Command:** `docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build`

## Current Hardcoded Values That Must Be Parameterized

| Value | Current Location | Current Value | Strategy |
|-------|-----------------|---------------|----------|
| Site name (metadata) | [`layout.tsx:16`](clientapp/src/app/layout.tsx:16) | `"Gloria Gronowicz Fine Art"` | `NEXT_PUBLIC_SITE_NAME` env var |
| Site description | [`layout.tsx:17`](clientapp/src/app/layout.tsx:17) | Long description string | `NEXT_PUBLIC_SITE_DESCRIPTION` env var |
| Site name (navbar) | [`NavBar.tsx:49`](clientapp/src/components/NavBar.tsx:49) | `"Gloria Gronowicz Fine Art"` | `NEXT_PUBLIC_SITE_NAME` env var |
| Email (footer) | [`Footer.tsx:11`](clientapp/src/components/Footer.tsx:11) | `"gloriagronowicz@gmail.com"` | `NEXT_PUBLIC_CONTACT_EMAIL` env var |
| Phone (footer) | [`Footer.tsx:15`](clientapp/src/components/Footer.tsx:15) | `"860.670.0799"` | `NEXT_PUBLIC_CONTACT_PHONE` env var |
| CSS colors | [`globals.css:6-23`](clientapp/src/app/globals.css:6) | Hardcoded hex values | CSS variable injection in layout.tsx |
| Database name | [`docker-compose.yml:6`](docker-compose/docker-compose.yml:6) | `artgallery` | Per-site via init script |
| Domain name | [`nginx.conf:125`](docker-compose/nginx/nginx.conf:125) | `ggpaintings.com` | Per-site server blocks |
| API URL | [`api.ts:19`](clientapp/src/lib/api.ts:19) | `SERVER_API_URL` (env var, already parameterized) | Per-site env var |
| Browser API URL | `.env.example:51` | `https://ggpaintings.com/api` | Per-site env var |

## Implementation Phases

### Phase 1: Parameterize Frontend Site Configuration

**Files to modify:**
1. [`clientapp/src/app/layout.tsx`](clientapp/src/app/layout.tsx) - Use env vars for metadata, inject CSS variables
2. [`clientapp/src/components/NavBar.tsx`](clientapp/src/components/NavBar.tsx) - Use env var for site name
3. [`clientapp/src/components/Footer.tsx`](clientapp/src/components/Footer.tsx) - Use env vars for contact info
4. [`clientapp/Dockerfile`](clientapp/Dockerfile) - Accept build args for site config

**Changes:**

#### `layout.tsx`
- Replace hardcoded `title` and `description` with `process.env.NEXT_PUBLIC_SITE_NAME` and `process.env.NEXT_PUBLIC_SITE_DESCRIPTION`
- Add `<style>` injection for CSS variables (`--background`, `--foreground`, `--navbar-footer-bg`, `--title-color`, `--button-color`) that reads from `NEXT_PUBLIC_CSS_*` env vars
- Handle both light and dark mode `:root` blocks

#### `NavBar.tsx`
- Replace `"Gloria Gronowicz Fine Art"` with `process.env.NEXT_PUBLIC_SITE_NAME || "Fine Art Gallery"`

#### `Footer.tsx`
- Replace hardcoded email with `process.env.NEXT_PUBLIC_CONTACT_EMAIL || ""`
- Replace hardcoded phone with `process.env.NEXT_PUBLIC_CONTACT_PHONE || ""`

#### `clientapp/Dockerfile`
- Add `ARG` directives for all `NEXT_PUBLIC_*` site config variables in builder stage
- Convert args to `ENV` for build-time availability
- Keep production stage `ENV` defaults for runtime override

### Phase 2: Create Multi-Site Docker Compose Structure

**Files to create/modify:**
1. Create `docker-compose/docker-compose.multi.yml` - Multi-site compose file
2. Create `docker-compose/scripts/init-databases.sql` - Database creation script
3. Create `docker-compose/.env.multi.example` - Multi-site environment template
4. Modify `docker-compose/nginx/nginx.conf` - Add per-site server blocks

**Docker Compose Structure:**

```yaml
# docker-compose/docker-compose.multi.yml
services:
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
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 20
      start_period: 30s
    networks:
      - artgallery-network
    restart: unless-stopped

  api-gg:
    container_name: artgallery-api-gg
    build:
      context: ../ServerApp
      dockerfile: ./ServerApp.Api/Dockerfile
      target: final
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=artgallery_gg;Username=postgres;Password=${POSTGRES_PASSWORD};
      CORS_ALLOWED_ORIGINS: ${GG_CORS_ALLOWED_ORIGINS}
      GoogleAuth__ClientId: ${GG_GOOGLE_AUTH_CLIENT_ID}
      GoogleAuth__ClientSecret: ${GG_GOOGLE_AUTH_CLIENT_SECRET}
      GoogleAuth__RedirectUri: ${GG_GOOGLE_AUTH_REDIRECT_URI}
      Admin__JwtSecretKey: ${GG_ADMIN_JWT_SECRET_KEY}
      Admin__AuthorizedEmails: ${GG_ADMIN_AUTHORIZED_EMAILS}
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - artgallery-network
    restart: unless-stopped
    read_only: true
    volumes:
      - image_data_gg:/app/images:rw
    tmpfs:
      - /tmp
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/api/health/health"]
      interval: 30s
      timeout: 10s
      retries: 5
      start_period: 40s

  frontend-gg:
    container_name: artgallery-frontend-gg
    build:
      context: ../clientapp
      dockerfile: Dockerfile
      target: production
      args:
        NEXT_PUBLIC_SITE_NAME: ${GG_SITE_NAME}
        NEXT_PUBLIC_SITE_DESCRIPTION: ${GG_SITE_DESCRIPTION}
        NEXT_PUBLIC_CONTACT_EMAIL: ${GG_CONTACT_EMAIL}
        NEXT_PUBLIC_CONTACT_PHONE: ${GG_CONTACT_PHONE}
        NEXT_PUBLIC_CSS_BACKGROUND: ${GG_CSS_BACKGROUND}
        NEXT_PUBLIC_CSS_FOREGROUND: ${GG_CSS_FOREGROUND}
        NEXT_PUBLIC_CSS_NAVBAR_FOOTER_BG: ${GG_CSS_NAVBAR_FOOTER_BG}
        NEXT_PUBLIC_CSS_TITLE_COLOR: ${GG_CSS_TITLE_COLOR}
        NEXT_PUBLIC_CSS_BUTTON_COLOR: ${GG_CSS_BUTTON_COLOR}
        NEXT_PUBLIC_API_URL: ${GG_NEXT_PUBLIC_API_URL}
        SERVER_API_URL: ${GG_SERVER_API_URL}
    user: "1001:1001"
    environment:
      NODE_ENV: production
      HOSTNAME: 0.0.0.0
      PORT: 3000
      NEXT_PUBLIC_API_URL: ${GG_NEXT_PUBLIC_API_URL}
      SERVER_API_URL: ${GG_SERVER_API_URL}
    networks:
      - artgallery-network
    restart: unless-stopped
    read_only: true
    tmpfs:
      - /tmp
      - /app/.next/cache
    command: node server.js
    depends_on:
      api-gg:
        condition: service_healthy

  api-flynn:
    container_name: artgallery-api-flynn
    build:
      context: ../ServerApp
      dockerfile: ./ServerApp.Api/Dockerfile
      target: final
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=artgallery_flynn;Username=postgres;Password=${POSTGRES_PASSWORD};
      CORS_ALLOWED_ORIGINS: ${FLYNN_CORS_ALLOWED_ORIGINS}
      GoogleAuth__ClientId: ${FLYNN_GOOGLE_AUTH_CLIENT_ID}
      GoogleAuth__ClientSecret: ${FLYNN_GOOGLE_AUTH_CLIENT_SECRET}
      GoogleAuth__RedirectUri: ${FLYNN_GOOGLE_AUTH_REDIRECT_URI}
      Admin__JwtSecretKey: ${FLYNN_ADMIN_JWT_SECRET_KEY}
      Admin__AuthorizedEmails: ${FLYNN_ADMIN_AUTHORIZED_EMAILS}
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - artgallery-network
    restart: unless-stopped
    read_only: true
    volumes:
      - image_data_flynn:/app/images:rw
    tmpfs:
      - /tmp
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/api/health/health"]
      interval: 30s
      timeout: 10s
      retries: 5
      start_period: 40s

  frontend-flynn:
    container_name: artgallery-frontend-flynn
    build:
      context: ../clientapp
      dockerfile: Dockerfile
      target: production
      args:
        NEXT_PUBLIC_SITE_NAME: ${FLYNN_SITE_NAME}
        NEXT_PUBLIC_SITE_DESCRIPTION: ${FLYNN_SITE_DESCRIPTION}
        NEXT_PUBLIC_CONTACT_EMAIL: ${FLYNN_CONTACT_EMAIL}
        NEXT_PUBLIC_CONTACT_PHONE: ${FLYNN_CONTACT_PHONE}
        NEXT_PUBLIC_CSS_BACKGROUND: ${FLYNN_CSS_BACKGROUND}
        NEXT_PUBLIC_CSS_FOREGROUND: ${FLYNN_CSS_FOREGROUND}
        NEXT_PUBLIC_CSS_NAVBAR_FOOTER_BG: ${FLYNN_CSS_NAVBAR_FOOTER_BG}
        NEXT_PUBLIC_CSS_TITLE_COLOR: ${FLYNN_CSS_TITLE_COLOR}
        NEXT_PUBLIC_CSS_BUTTON_COLOR: ${FLYNN_CSS_BUTTON_COLOR}
        NEXT_PUBLIC_API_URL: ${FLYNN_NEXT_PUBLIC_API_URL}
        SERVER_API_URL: ${FLYNN_SERVER_API_URL}
    user: "1001:1001"
    environment:
      NODE_ENV: production
      HOSTNAME: 0.0.0.0
      PORT: 3000
      NEXT_PUBLIC_API_URL: ${FLYNN_NEXT_PUBLIC_API_URL}
      SERVER_API_URL: ${FLYNN_SERVER_API_URL}
    networks:
      - artgallery-network
    restart: unless-stopped
    read_only: true
    tmpfs:
      - /tmp
      - /app/.next/cache
    command: node server.js
    depends_on:
      api-flynn:
        condition: service_healthy

  nginx:
    container_name: artgallery-nginx
    build:
      context: ./nginx
      dockerfile: Dockerfile
    ports:
      - "${NGINX_HTTP_PORT:-80}:80"
      - "${NGINX_HTTPS_PORT:-443}:443"
      - "${NGINX_HEALTH_PORT:-9090}:8080"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/ssl/gg:/etc/nginx/ssl/gg:ro
      - ./nginx/ssl/flynn:/etc/nginx/ssl/flynn:ro
    networks:
      - artgallery-network
    restart: unless-stopped
    read_only: true
    tmpfs:
      - /var/cache/nginx
      - /var/run

volumes:
  postgres_data:
  image_data_gg:
  image_data_flynn:

networks:
  artgallery-network:
    driver: bridge

secrets:
  postgres_password:
    file: ./secrets/postgres_password
```

### Phase 3: Create NGINX Multi-Site Configuration

**File:** `docker-compose/nginx/nginx.conf`

The NGINX config needs per-site server blocks with per-site upstreams:

```nginx
events {
    worker_connections 1024;
}

http {
    include       /etc/nginx/mime.types;
    default_type  application/octet-stream;

    # Cloudflare Real IP
    set_real_ip_from 173.245.48.0/20;
    # ... (keep existing Cloudflare IP ranges) ...
    real_ip_header CF-Connecting-IP;
    real_ip_recursive on;

    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for"';
    access_log /var/log/nginx/access.log main;
    error_log /var/log/nginx/error.log warn;

    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;

    gzip on;
    gzip_vary on;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_types text/plain text/css text/xml application/json application/javascript;

    # === Site: gg Upstreams ===
    upstream api_gg {
        server api-gg:8080;
        keepalive 16;
    }
    upstream frontend_gg {
        server frontend-gg:3000;
        keepalive 16;
    }

    # === Site: flynn Upstreams ===
    upstream api_flynn {
        server api-flynn:8080;
        keepalive 16;
    }
    upstream frontend_flynn {
        server frontend-flynn:3000;
        keepalive 16;
    }

    # Health check
    server {
        listen 8080;
        server_name _;
        location /health {
            access_log off;
            return 200 "healthy\n";
            add_header Content-Type text/plain;
        }
    }

    # === Site: ggpaintings.com ===
    server {
        listen 443 ssl http2;
        server_name ggpaintings.com www.ggpaintings.com;

        ssl_certificate /etc/nginx/ssl/gg/server.crt;
        ssl_certificate_key /etc/nginx/ssl/gg/server.key;
        ssl_protocols TLSv1.2 TLSv1.3;

        client_max_body_size 20M;

        # Security headers
        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
        add_header X-Frame-Options "DENY" always;
        add_header X-Content-Type-Options "nosniff" always;

        location /api/ {
            proxy_pass http://api_gg/api/;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-For $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-Proto $scheme;
        }

        location /images/ {
            proxy_pass http://api_gg/images/;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
        }

        location / {
            proxy_pass http://frontend_gg;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-For $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }

    # === Site: flynnart.com ===
    server {
        listen 443 ssl http2;
        server_name flynnart.com www.flynnart.com;

        ssl_certificate /etc/nginx/ssl/flynn/server.crt;
        ssl_certificate_key /etc/nginx/ssl/flynn/server.key;
        ssl_protocols TLSv1.2 TLSv1.3;

        client_max_body_size 20M;

        add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
        add_header X-Frame-Options "DENY" always;
        add_header X-Content-Type-Options "nosniff" always;

        location /api/ {
            proxy_pass http://api_flynn/api/;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-For $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-Proto $scheme;
        }

        location /images/ {
            proxy_pass http://api_flynn/images/;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
        }

        location / {
            proxy_pass http://frontend_flynn;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-For $http_cf_connecting_ip;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }

    # HTTP to HTTPS redirect
    server {
        listen 80;
        server_name ggpaintings.com www.ggpaintings.com flynnart.com www.flynnart.com;
        return 301 https://$host$request_uri;
    }
}
```

### Phase 4: Create Database Init Script

**File:** `docker-compose/scripts/init-databases.sql`

```sql
-- Multi-Site Database Initialization Script
-- This script creates separate databases for each site.
-- It is mounted to /docker-entrypoint-initdb.d/ in the PostgreSQL container
-- and will only run on first startup when the data directory is empty.
--
-- On existing deployments, this script will NOT modify existing databases.
-- To add a new site, create a new script file (e.g., init-databases-site3.sql)
-- and temporarily remove the postgres_data volume to reinitialize.

-- Create database for gg site (ggpaintings.com)
SELECT 'CREATE DATABASE artgallery_gg'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'artgallery_gg')\gexec

-- Create database for flynn site (flynnart.com)
SELECT 'CREATE DATABASE artgallery_flynn'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'artgallery_flynn')\gexec
```

**Implementation Notes:**
- Uses idempotent `SELECT ... WHERE NOT EXISTS ... \gexec` pattern instead of plain `CREATE DATABASE` to prevent errors on re-runs
- Script is mounted to `/docker-entrypoint-initdb.d/` in the PostgreSQL container
- Only executes on first startup when the data volume is empty
- PostgreSQL service in `docker-compose.multi.yml` mounts the script via:
  ```yaml
  volumes:
    - ./scripts/init-databases.sql:/docker-entrypoint-initdb.d/init-databases.sql:ro
  ```
- Each API service connects to its respective database:
  - `api-gg` → `artgallery_gg` (`ConnectionStrings__DefaultConnection: ...Database=artgallery_gg;...`)
  - `api-flynn` → `artgallery_flynn` (`ConnectionStrings__DefaultConnection: ...Database=artgallery_flynn;...`)

### Phase 5: Create Multi-Site Environment File

**Template File:** `docker-compose/.env.multi.example` (committed to git)
**Production File:** `docker-compose/.env.multi` (created from template, gitignored)
**Local File:** `docker-compose/.env.multi.local` (local development, gitignored)

> **IMPORTANT:** The `.env.multi` file contains secrets and MUST be in `.gitignore`.
> Copy `.env.multi.example` to `.env.multi` and fill in actual production values.

```bash
# Multi-Site Environment Configuration
# Copy this file to .env.multi and fill in actual values
#
# Usage: docker compose -f docker-compose.multi.yml up -d --build
#
# IMPORTANT:
# - PostgreSQL password must match the content of ./secrets/postgres_password
# - Each site needs its own Google OAuth client ID configured in Google Cloud Console
# - JWT secrets should be long, random strings (use: openssl rand -base64 32)

# ============================================
# PostgreSQL
# ============================================
# Password stored in secrets/postgres_password file
# Create it with: echo -n 'your_password' > secrets/postgres_password

# ============================================
# NGINX Ports
# ============================================
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443
NGINX_HEALTH_PORT=9090

# ============================================
# Site: gg (ggpaintings.com)
# ============================================

# --- Frontend Site Configuration ---
GG_SITE_NAME="Gloria Gronowicz Fine Art"
GG_SITE_DESCRIPTION="Gloria Gronowicz is an oil painter who creates works inspired by nature"
GG_CONTACT_EMAIL="gloriagronowicz@gmail.com"
GG_CONTACT_PHONE="860.670.0799"

# --- CSS Theme Variables ---
GG_CSS_BACKGROUND="#3d3d3d"
GG_CSS_FOREGROUND="#ffffff"
GG_CSS_NAVBAR_FOOTER_BG="#2d2d2d"
GG_CSS_TITLE_COLOR="#66b3ff"
GG_CSS_BUTTON_COLOR="#1e3a8a"

# --- API URLs ---
GG_NEXT_PUBLIC_API_URL=/api
GG_SERVER_API_URL=http://api-gg:8080/api

# --- CORS ---
GG_CORS_ALLOWED_ORIGINS=https://ggpaintings.com

# --- Google OAuth ---
GG_GOOGLE_AUTH_CLIENT_ID=YOUR_GG_CLIENT_ID.apps.googleusercontent.com
GG_GOOGLE_AUTH_CLIENT_SECRET=YOUR_GG_CLIENT_SECRET
GG_GOOGLE_AUTH_REDIRECT_URI=https://ggpaintings.com/api/auth/google/callback

# --- Admin JWT ---
GG_ADMIN_JWT_SECRET_KEY=CHANGE_THIS_TO_A_RANDOM_SECRET_KEY_GG
GG_ADMIN_JWT_EXPIRY_MINUTES=60
GG_ADMIN_AUTHORIZED_EMAILS=gloriagronowicz@gmail.com

# --- Database ---
GG_DATABASE_READ_ONLY_MODE=false

# ============================================
# Site: flynn (flynnart.com)
# ============================================

# --- Frontend Site Configuration ---
FLYNN_SITE_NAME="Flynn Art Gallery"
FLYNN_SITE_DESCRIPTION="Fine art paintings by Flynn"
FLYNN_CONTACT_EMAIL="flynn@example.com"
FLYNN_CONTACT_PHONE="555.123.4567"

# --- CSS Theme Variables ---
FLYNN_CSS_BACKGROUND="#1a1a2e"
FLYNN_CSS_FOREGROUND="#e0e0e0"
FLYNN_CSS_NAVBAR_FOOTER_BG="#16213e"
FLYNN_CSS_TITLE_COLOR="#e94560"
FLYNN_CSS_BUTTON_COLOR="#0f3460"

# --- API URLs ---
FLYNN_NEXT_PUBLIC_API_URL=/api
FLYNN_SERVER_API_URL=http://api-flynn:8080/api

# --- CORS ---
FLYNN_CORS_ALLOWED_ORIGINS=https://flynnart.com

# --- Google OAuth ---
FLYNN_GOOGLE_AUTH_CLIENT_ID=YOUR_FLYNN_CLIENT_ID.apps.googleusercontent.com
FLYNN_GOOGLE_AUTH_CLIENT_SECRET=YOUR_FLYNN_CLIENT_SECRET
FLYNN_GOOGLE_AUTH_REDIRECT_URI=https://flynnart.com/api/auth/google/callback

# --- Admin JWT ---
FLYNN_ADMIN_JWT_SECRET_KEY=CHANGE_THIS_TO_A_RANDOM_SECRET_KEY_FLYNN
FLYNN_ADMIN_JWT_EXPIRY_MINUTES=60
FLYNN_ADMIN_AUTHORIZED_EMAILS=flynn@example.com

# --- Database ---
FLYNN_DATABASE_READ_ONLY_MODE=false
```

**Implementation Notes:**
- Template file (`.env.multi.example`) is committed to git with placeholder values
- Production file (`.env.multi`) is gitignored and created by copying the template
- Local file (`.env.multi.local`) overrides for local development (different ports, dev credentials)
- Each site has independent configuration for:
  - Frontend branding (name, description, contact info, CSS theme)
  - API URLs (client-side and server-side)
  - CORS origins
  - Google OAuth credentials (separate client IDs per site)
  - Admin JWT (separate secrets and authorized emails per site)
  - Database read-only mode toggle
- Variables are consumed by `docker-compose.multi.yml` via `${VAR_NAME}` substitution
- Frontend build args pass `NEXT_PUBLIC_*` variables at build time
- Backend environment variables passed at runtime

### Phase 6: Create Multi-Site Deploy Script

**File:** `docker-compose/deploy-multi.sh`

The deploy script handles the complete multi-site deployment workflow including NGINX permissions, PostgreSQL initialization, database backup restore, security checks, and health verification.

**Deployment Steps (6 phases):**

1. **[1/6] NGINX File Permissions** — Sets ownership of SSL directories and `nginx.conf` to UID 101 (nginx non-root user)
2. **[2/6] Stop Existing Containers** — Tears down any running multi-site containers
3. **[3/6] Start PostgreSQL** — Starts PostgreSQL alone and waits for healthy status (up to 180s)
4. **[4/6] Restore Databases from Backups** — For each site (`gg`, `flynn`), finds the latest backup in `/opt/artgallery/backups/` and restores it. Falls back to database seeder if no backup exists.
5. **[5/6] Build and Start Remaining Containers** — Builds and starts all API, frontend, and NGINX containers
6. **[6/6] Container Status Check** — Displays container status table

**Key Features:**
- **Mandatory `.env.multi`** — Script exits with error if `.env.multi` is not found (no silent fallback)
- **Docker Compose v1/v2 detection** — Auto-detects `docker compose` or `docker-compose`
- **Per-site database restore** — Maps `gg` → `artgallery_gg`, `flynn` → `artgallery_flynn`
- **Backup directory** — Configurable via `BACKUP_DIR` env var, defaults to `/opt/artgallery/backups`
- **PostgreSQL health wait** — Blocks until PostgreSQL is healthy before proceeding
- **Security checks** — Verifies all containers run as non-root users, checks for sensitive files in git, verifies `.env.multi` is gitignored

**Security Checks:**
```
Container User Verification:
  API (GG):         appuser (non-root)
  API (Flynn):      appuser (non-root)
  Frontend (GG):    nextjs (non-root)
  Frontend (Flynn): nextjs (non-root)
  PostgreSQL:       postgres (non-root)
  NGINX:            nginx (non-root)

Git Security:
  No sensitive files tracked in git
  .env.multi properly excluded from git
```

**Usage:**
```bash
cd docker-compose
sudo bash deploy-multi.sh
```

**Database Restore Logic:**
```bash
# Site database mapping
declare -A SITE_DATABASES
SITE_DATABASES[gg]="artgallery_gg"
SITE_DATABASES[flynn]="artgallery_flynn"

for site in gg flynn; do
    DATABASE_NAME="${SITE_DATABASES[$site]}"
    # Find latest backup for this specific site
    LATEST_BACKUP=$(ls -1t "$BACKUP_DIR"/artgallery_${DATABASE_NAME}_*.dump 2>/dev/null | head -1)
    if [ -n "$LATEST_BACKUP" ]; then
        # Drop and recreate database, then restore
        docker exec "$CONTAINER_NAME" psql -U postgres -c "DROP DATABASE IF EXISTS $DATABASE_NAME;"
        docker exec "$CONTAINER_NAME" psql -U postgres -c "CREATE DATABASE $DATABASE_NAME;"
        docker exec -i "$CONTAINER_NAME" pg_restore -U postgres -d "$DATABASE_NAME" "/tmp/$BACKUP_NAME"
    fi
done
```

**Implementation Notes:**
- Script is significantly more comprehensive than the original plan (6 steps vs 4)
- Added PostgreSQL health wait loop (up to 180s) to ensure DB is ready before restore
- Added per-site database restore from backups (matching single-site `deploy.sh` behavior)
- Added security verification (non-root users, git-sensitive-file checks)
- Backup restore is non-fatal — falls back to database seeder if backup unavailable

## Local Development Testing

For localhost testing, use these URLs:
- **Site gg:** `https://gg.localhost` (or `http://localhost:3001`)
- **Site flynn:** `https://flynn.localhost` (or `http://localhost:3002`)

Add to `/etc/hosts` (Linux) or `C:\Windows\System32\drivers\etc\hosts` (Windows):
```
127.0.0.1 gg.localhost
127.0.0.1 flynn.localhost
```

## Deployment Workflow

### Local Development
```bash
# Start multi-site stack
cd docker-compose
docker compose -f docker-compose.multi.yml up -d --build

# Visit sites
# http://gg.localhost or http://localhost:80 -> ggpaintings.com
# http://flynn.localhost or http://localhost:80 -> flynnart.com
```

### Production Deployment
```bash
# Pull latest code
cd ~/Painting_Website_Docker_NextJS
git pull

# Update .env.multi with production values
nano docker-compose/.env.multi

# Deploy
cd docker-compose
bash deploy-multi.sh
```

### Production Deployment with Backup Restore

The `deploy-multi.sh` script includes automatic database restore from backups:

```bash
# 1. Pull latest code
cd ~/Painting_Website_Docker_NextJS
git pull

# 2. Ensure .env.multi is configured
nano docker-compose/.env.multi

# 3. Deploy (automatically restores latest backups if available)
cd docker-compose
bash deploy-multi.sh
```

**Deployment flow:**
1. Sets NGINX SSL permissions
2. Stops existing containers
3. Starts PostgreSQL, waits for health
4. **Restores latest backup for `artgallery_gg`** (if exists in `/opt/artgallery/backups/`)
5. **Restores latest backup for `artgallery_flynn`** (if exists in `/opt/artgallery/backups/`)
6. Builds and starts all remaining containers
7. Runs security checks

If no backups exist, the database seeder will populate initial data.

### Setting Up Automated Backups

After initial deployment, install the cron-based backup system:

```bash
# Install multi-site backup cron (requires sudo)
cd docker-compose/scripts
sudo ./install-backup-cron-multi.sh
```

This installs:
- `backup-multi.sh` - Backs up both site databases automatically
- `restore-multi.sh` - Restores individual site databases
- Cron job running every Sunday at 2:00 AM
- Configuration file with PostgreSQL credentials

### Manual Backup and Restore

```bash
# Backup both sites
/opt/artgallery/scripts/backup-multi.sh

# Backup specific site
/opt/artgallery/scripts/backup-multi.sh gg
/opt/artgallery/scripts/backup-multi.sh flynn

# Restore specific site from backup
/opt/artgallery/scripts/restore-multi.sh gg artgallery_artgallery_gg_20260101_120000.dump
/opt/artgallery/scripts/restore-multi.sh flynn artgallery_artgallery_flynn_20260101_120000.dump
```

### Backup File Naming Convention

| Site | Database | Backup File Pattern |
|------|----------|---------------------|
| ggpaintings.com | artgallery_gg | `artgallery_artgallery_gg_TIMESTAMP.dump` |
| flynnart.com | artgallery_flynn | `artgallery_artgallery_flynn_TIMESTAMP.dump` |

### Single-Site vs Multi-Site Backup Comparison

| Feature | Single-Site | Multi-Site |
|---------|-------------|------------|
| Container | `artgallery-postgres-prod` | `artgallery-postgres` |
| Database | `artgallery` | `artgallery_gg`, `artgallery_flynn` |
| API Container | `artgallery-api-prod` | `artgallery-api-gg`, `artgallery-api-flynn` |
| Backup Script | `backup.sh` | `backup-multi.sh` |
| Restore Script | `restore.sh` | `restore-multi.sh` |
| Install Script | `install-backup-cron.sh` | `install-backup-cron-multi.sh` |
| Backup Pattern | `artgallery_db_TIMESTAMP.dump` | `artgallery_artgallery_{site}_TIMESTAMP.dump` |
| Deploy Restore | Yes (single DB) | Yes (per-site DB restore) |

## Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| Existing single-site deployment broken | HIGH | Keep `docker-compose.yml` unchanged; multi-site uses separate `docker-compose.multi.yml` |
| Database volume conflict | HIGH | Multi-site uses fresh `postgres_data` volume; existing data preserved |
| NGINX config syntax error | HIGH | Test `nginx -t` before deploying |
| Build args not passed correctly | MEDIUM | Verify with `docker inspect` after build |
| CSS injection breaks layout | MEDIUM | Test locally first with both sites |

## Rollback Plan

If multi-site deployment fails:
```bash
# Stop multi-site stack
docker compose -f docker-compose.multi.yml down

# Restart single-site stack
docker compose -f docker-compose.prod.yml up -d
```

## Summary of Files to Create/Modify

### Create (New Files)
1. `docker-compose/docker-compose.multi.yml` - Multi-site compose file
2. `docker-compose/scripts/init-databases.sql` - Database creation script
3. `docker-compose/.env.multi.example` - Multi-site env template
4. `docker-compose/deploy-multi.sh` - Multi-site deploy script

### Modify (Existing Files)
1. `clientapp/src/app/layout.tsx` - Parameterize metadata, inject CSS vars
2. `clientapp/src/components/NavBar.tsx` - Parameterize site name
3. `clientapp/src/components/Footer.tsx` - Parameterize contact info
4. `clientapp/Dockerfile` - Add build args for site config
5. `docker-compose/nginx/nginx.conf` - Add per-site server blocks

### Keep Unchanged
1. `docker-compose/docker-compose.yml` - Single-site config (for rollback)
2. `docker-compose/docker-compose.prod.yml` - Single-site production config
3. `docker-compose/deploy.sh` - Single-site deploy script
