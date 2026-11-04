
# Deployment Guide: Art Gallery Application

**Domains:** ggpaintings.com, flynnart.com
**Last Updated:** 2026-11-04

This guide explains how to deploy the Art Gallery application to production with Cloudflare.

## Overview

The `docker-compose/` folder holds several compose files. Only one stack runs at a time:

| File | Purpose |
|------|---------|
| `docker-compose.yml` (+ `docker-compose.override.yml`) | Local development: hot reload, debug builds, no NGINX |
| `docker-compose.prod.yml` | **Legacy single-site** stack (gg only). Being retired — see note below |
| **`docker-compose.multi.yml`** | **Current production** stack: gg + flynn, shared PostgreSQL, shared NGINX |
| `docker-compose.multi.arm64.yml` | Same topology, tuned for ARM64 hosts (Raspberry Pi / Apple Silicon) |
| `docker-compose.multi.local.yml` | Override for testing both sites locally on non-standard ports |

> **There is no `docker-compose.cloudflare.yml`.** Cloudflare "Full" mode is not a separate
> stack — it is an SSL mode that applies to `docker-compose.prod.yml` and
> `docker-compose.multi.yml`: NGINX serves a self-signed origin certificate and Cloudflare
> terminates TLS at the edge. Set the Cloudflare SSL/TLS mode to **Full**, never
> *Full (strict)*, which would demand a CA-signed origin certificate.

> **Legacy note:** `docker-compose.prod.yml` runs `artgallery-postgres-prod` with a single
> `artgallery` database. The multi-site stack uses a different container (`artgallery-postgres`)
> with per-site databases (`artgallery_gg`, `artgallery_flynn`). Both stacks want ports 80/443
> and the `postgres_data` volume, so **they cannot run simultaneously**. Stop the old stack
> before starting the new one.

## Deployment Options

| Mode | Use Case | SSL | Best For |
|------|----------|-----|----------|
| Development | Local development | None | Coding, testing |
| Single-site production (`prod.yml`) | One gallery | Let's Encrypt or commercial | Retired — kept for reference |
| **Multi-site + Cloudflare Full** | **Home server hosting two galleries behind Cloudflare** | **Self-signed origin cert + Cloudflare edge** | **The current production setup** |

## Security Features

This deployment includes the following security hardening:

- ✅ **All containers run as non-root users** (API: UID 1 `appuser`, Frontend: UID 1001 `nextjs`, NGINX: UID 101 `nginx`, PostgreSQL: `postgres`)
- ✅ **Read-only filesystem** for API, Frontend and NGINX containers (writable exceptions: the `/app/images` bind mount and tmpfs caches)
- ✅ **Health checks** on all containers, with startup ordering `api → frontend → nginx`
- ⚠️ **The database password is passed as an environment variable** (`POSTGRES_PASSWORD`), so it *is* visible in `docker inspect`. The file `secrets/postgres_password` must still exist because `docker-compose.multi.yml` declares it as a secret, but PostgreSQL does not read it — the official image ignores `*_FILE` variables.
- ❌ **No TLS to the database** — PostgreSQL is reached over the private `artgallery-network` bridge and is never published to a host port.
- ✅ **Security headers** (HSTS, X-Frame-Options, X-Content-Type-Options)
- ✅ **Domain-based routing** - NGINX `server_name` ensures only correct domain is served
- ✅ **Resource limits** configured for all containers (CPU/memory)
- ✅ **.dockerignore files** to optimize builds and prevent sensitive file exposure

### Cloudflare Security Configuration

Cloudflare provides additional security layers. Configure these settings in your Cloudflare dashboard:

#### 1. Security Level Settings

**Location:** Security → Overview → Security Level

- **Recommended:** `Medium` or `High`
- **What it does:** Challenges suspicious traffic with JavaScript or CAPTCHA challenges
- **Settings:**
  - `Medium`: Challenges suspicious traffic
  - `High`: Challenges all traffic from countries not in your whitelist
  - `Essentially On`: Challenges all traffic (use for testing)

#### 2. Bot Fight Mode

**Location:** Security → Bots → Bot Fight Mode

- **Enable:** `Bot Fight Mode`
- **What it does:** Automatically blocks known bots and suspicious automated traffic
- **Recommended:** Enabled for all sites

#### 3. Under Attack Mode (Emergency Only)

**Location:** Security → Settings → Under Attack Mode

- **Use when:** Under active attack
- **What it does:** Shows a challenge page to all visitors
- **Note:** Only enable during active attacks (adds delay for all users)

#### 4. WAF (Web Application Firewall) Rules

**Location:** Security → WAF → Custom Rules

Create these custom rules for additional API protection:

**Rule 1: Block Direct API Access from Non-Browser Traffic**
```
Name: Block Non-Browser API Access
Expression: (http.host eq "ggpaintings.com" or http.host eq "www.ggpaintings.com") and (http.path contains "/api") and not (http.user_agent contains "Mozilla")
Action: Block
```

**Rule 2: Rate Limiting for API Endpoints**
```
Name: API Rate Limit
Expression: (http.host eq "ggpaintings.com" or http.host eq "www.ggpaintings.com") and (http.path contains "/api")
Action: Challenge
Settings: 100 requests per IP per minute
```

**Rule 3: Block Suspicious User Agents**
```
Name: Block Malicious User Agents
Expression: (http.user_agent contains "sqlmap") or (http.user_agent contains "nikto") or (http.user_agent contains "nmap")
Action: Block
```

#### 5. Rate Limiting Rules

**Location:** Security → WAF → Rate Limiting Rules

Create a rate limiting rule:

```
Name: API Rate Limiting
Match:
  - Expression: (http.path contains "/api")
  - Request rate: 100 requests per minute per IP
Action: Challenge (JavaScript challenge)
```

#### 6. Security Headers (Cloudflare Edge)

**Location:** SSL/TLS → Edge Certificates → Custom Certificate

Cloudflare automatically adds these security headers:
- `Strict-Transport-Security` (HSTS)
- `X-Frame-Options`
- `X-Content-Type-Options`
- `Cache-Control`

#### 7. Cache Rules for API

**Location:** Caching → Cache Rules

Create a rule to prevent API caching (API responses should be dynamic):

```
Name: No Cache for API
If: (http.host eq "ggpaintings.com" or http.host eq "www.ggpaintings.com") and (http.path contains "/api")
Then: Cache everything = Off
```

#### 8. Firewall Rules (IP Blocking)

**Location:** Security → WAF → Firewall Rules

Block known malicious IPs or countries if needed:

```
# Example: Block specific country (use cautiously)
Expression: (ip.geoip.country eq "XX")  # Replace XX with country code
Action: Block
```

#### 9. Access Rules (IP Whitelisting)

**Location:** Security → WAF → Tools → IP Addresses

- View blocked/challenged IPs
- Add your IP to whitelist if needed for testing

#### 10. Page Rules (Optional)

**Location:** Rules → Page Rules

Create page rules for specific behaviors:

```
# Example: Always use HTTPS for API
URL: ggpaintings.com/api/*
Settings:
  - Always Use HTTPS: On
  - Cache Level: Bypass
  - Edge Cache TTL: 0 seconds
```

### Quick Cloudflare Security Checklist

After adding your site to Cloudflare, configure these settings:

- [ ] **SSL/TLS Mode:** Set to `Full` (not Flexible)
- [ ] **Always Use HTTPS:** Enabled
- [ ] **Automatic HTTPS Rewrites:** Enabled
- [ ] **Security Level:** Set to `Medium`
- [ ] **Bot Fight Mode:** Enabled
- [ ] **WAF Rules:** Add custom rules for API protection
- [ ] **Rate Limiting:** Configure for API endpoints
- [ ] **Cache Rules:** Bypass cache for API
- [ ] **DNS Records:** Ensure orange cloud (proxied) is enabled

### Cloudflare Security Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    INTERNET                                  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              CLOUDFLARE EDGE (Security Layer)               │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  • DDoS Protection                                    │  │
│  │  • WAF (Web Application Firewall)                     │  │
│  │  • Bot Fight Mode                                     │  │
│  │  • Rate Limiting                                      │  │
│  │  • Security Level Challenges                          │  │
│  │  • SSL/TLS Termination (Edge)                         │  │
│  └───────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTPS (Encrypted)
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              YOUR SERVER (ggpaintings.com)                  │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  NGINX Reverse Proxy                                   │  │
│  │  • Self-signed SSL Termination                         │  │
│  │  • API Access Control (Same-Origin Only)              │  │
│  │  • Security Headers                                    │  │
│  │  • Non-root User (UID 101)                            │  │
│  │  • Read-only Filesystem                               │  │
│  └────────────────────────┬───────────────────────────────┘  │
                            │
              ┌─────────────┴─────────────┐
              ▼                           ▼
       ┌────────────┐            ┌────────────┐
       │  Frontend  │            │    API     │
       │  (UID 1001)│            │  (UID 1)   │
       │  Read-only │            │  Read-only │
       └────────────┘            └─────┬──────┘
                                       │
                                       ▼
                               ┌───────────────┐
                               │  PostgreSQL 17│
                               │ (user:postgres)│
                               └───────────────┘
└─────────────────────────────────────────────────────────────┘
```

## Prerequisites (Production)

- Docker and Docker Compose installed on Linux server
- Domain name **ggpaintings.com** configured and pointing to server IP
- SSL certificates (self-signed for Cloudflare Full mode)
- At least 4GB RAM recommended
- Cloudflare account with domain added

## Quick Start: Deploy to ggpaintings.com

### Step 1: Cloudflare Setup

1. **Add your site to Cloudflare**
   - Sign up/login to [Cloudflare](https://dash.cloudflare.com)
   - Add your domain `ggpaintings.com`
   - Change nameservers at your registrar to Cloudflare's nameservers

2. **Configure DNS**
   - Go to DNS → Records
   - Create an A record: `@` → Your server's IP address
   - Enable proxy (orange cloud icon should be ON)

3. **Configure SSL/TLS**
   - Go to SSL/TLS → Overview
   - Select **"Full"** mode (not "Flexible" or "Full (Strict)")
   - Go to Edge Certificates → Enable "Always Use HTTPS"
   - Enable "Automatic HTTPS Rewrites"

### Step 2: Server Setup

```bash
# On your Linux server
git clone <repository-url>
cd Painting_Website_Docker_NextJS/docker-compose
```

### Step 3: Configure Environment

**3.1. Copy and Edit Environment File**

```bash
cp .env.example .env
nano .env
```

Ensure these values are set in `.env`:
```env
CORS_ALLOWED_ORIGINS=https://ggpaintings.com
NEXT_PUBLIC_API_URL=https://ggpaintings.com/api
SERVER_API_URL=http://api:8080/api
```

**3.2. Set the PostgreSQL Password**

The database is **PostgreSQL 17** (image `postgres:17-alpine`), not SQL Server. The
password comes from `POSTGRES_PASSWORD` in `.env`.

```bash
mkdir -p secrets
echo -n 'YourSecurePassword123!' > secrets/postgres_password
chmod 600 secrets/postgres_password
```

Set the same value in `.env`:

```env
POSTGRES_PASSWORD=YourSecurePassword123!
```

**Important:**
- Use `echo -n` to avoid a trailing newline
- File permissions should be `600` (read/write for owner only)
- The `secrets/` directory is already excluded from git (see `.gitignore`)
- Never commit the actual password file to version control

**Why the secret file if the password is also in `.env`?**
It is not really a secret mechanism here. The official postgres image **ignores**
`*_FILE` variables, so `secrets/postgres_password` is never read by the database. It exists
only because `docker-compose.prod.yml` declares it and Compose refuses to start when a
declared secret file is missing. Keep it identical to `POSTGRES_PASSWORD` and treat
`docker inspect` output as containing a live credential.

### Step 4: Generate Self-Signed SSL Certificate

```bash
mkdir -p nginx/ssl
cd nginx/ssl

openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout server.key \
  -out server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=ggpaintings.com"

chmod 600 server.key
chmod 644 server.crt
cd ../..
```

### Step 5: Deploy

```bash
# Run the deployment script (includes security checks)
bash deploy.sh

# Or manually:
docker-compose -f docker-compose.prod.yml up -d --build
```

### Step 6: Verify Deployment

**6.1. Check Container Status**

```bash
docker-compose -f docker-compose.prod.yml ps
```

All containers should show `healthy` status.

**6.2. Verify Non-Root Users**

Container names in `docker-compose.prod.yml` carry a `-prod` suffix. Note that
`docker exec ... whoami` only reflects a container's real user when its image declares a
`USER` — for NGINX and PostgreSQL it does not, so check those with `docker top`:

```bash
# Images that pin a non-root USER
docker exec artgallery-api-prod id -u        # non-zero — the .NET base image's app user
docker exec artgallery-frontend-prod id -u   # 1001 (nextjs)

# Images that start as root and drop privileges at runtime — check the running process
docker top artgallery-nginx | head -2              # USER column should be nginx (101)
docker top artgallery-postgres-prod | head -2      # USER column should be postgres
```

A `0` / `root` process owner means the privilege drop did not happen.

**6.3. Verify Resource Limits**

Declared in `docker-compose.prod.yml` under `deploy.resources.limits`. Plain
`docker compose` does not always apply these, so measure rather than assume:

```bash
# Declared limits: postgres 2 CPU / 4G, api 2 CPU / 4G,
#                 frontend 2 CPU / 4G, nginx 1 CPU / 1G
docker inspect artgallery-postgres-prod --format 'cpus={{.HostConfig.NanoCpus}} mem={{.HostConfig.Memory}}'
docker inspect artgallery-api-prod      --format 'cpus={{.HostConfig.NanoCpus}} mem={{.HostConfig.Memory}}'
docker inspect artgallery-frontend-prod --format 'cpus={{.HostConfig.NanoCpus}} mem={{.HostConfig.Memory}}'
docker inspect artgallery-nginx         --format 'cpus={{.HostConfig.NanoCpus}} mem={{.HostConfig.Memory}}'
```

`cpus=0` / `mem=0` means no limit was applied and the containers can consume the whole
host.

**6.4. Check the Database Password Is Not Leaking**

```bash
# The password IS present in the container environment by design —
# the postgres image only accepts POSTGRES_PASSWORD.
docker inspect artgallery-postgres-prod --format '{{.Config.Env}}' | tr ' ' '\n' | grep -i password
```

Treat that output as a credential: do not paste it into issues, chat logs, or committed
files. Anyone with read access to the Docker socket can already extract it, so the only
real mitigation is making the password unique to this host.

**6.5. Verify Read-Only Filesystems**

```bash
docker inspect artgallery-api-prod --format '{{.HostConfig.ReadonlyRootfs}}'       # Should return: true
docker inspect artgallery-frontend-prod --format '{{.HostConfig.ReadonlyRootfs}}'   # Should return: true
docker inspect artgallery-nginx --format '{{.HostConfig.ReadonlyRootfs}}'           # Should return: true
```

**6.6. Test the Site**

```bash
# Test HTTPS endpoint
curl -I https://ggpaintings.com

# Test API endpoint
curl -I https://ggpaintings.com/api/health/health
```

**6.7. Verify Health Checks**

```bash
# Check all container health status
docker-compose -f docker-compose.prod.yml ps
# All containers should show "healthy" in the STATUS column
```

### Step 7: Wait for Cloudflare DNS Propagation

DNS changes typically propagate within 5-10 minutes. Once complete, your site will be accessible at:
- **Frontend:** https://ggpaintings.com
- **API:** https://ggpaintings.com/api

---

## Development Mode (Local)

### Purpose
- Fast development with hot reload
- Debug mode enabled
- Direct API access (no NGINX)
- Source code mounted for live editing

### Start Development Environment

```bash
cd docker-compose
docker-compose up -d
```

### Configuration

**Environment Variables** (in `.env` or `.env.example`):
```env
NEXT_PUBLIC_API_URL=http://localhost:8080/api
SERVER_API_URL=http://api:8080/api
CORS_ALLOWED_ORIGINS=http://localhost:3000
```

**How it works:**
- Browser accesses frontend at `http://localhost:3000`
- Browser accesses API at `http://localhost:8080/api` (exposed port)
- Frontend container accesses API at `http://api:8080/api` (Docker network)
- Hot reload enabled - changes to source code automatically update

### Access Points
- Frontend: http://localhost:3000
- API: http://localhost:8080/api
- Swagger: http://localhost:8080/swagger
- PostgreSQL: **not published** — `docker-compose.yml` exposes no host port for the
  `postgres` service. Reach it with `docker compose exec postgres psql -U postgres -d artgallery`.

### Stop Development Environment

```bash
docker-compose down
```

To also remove volumes (database data):
```bash
docker-compose down -v
```

## Production Mode (Deployment)

### Purpose
- Optimized builds for performance
- NGINX reverse proxy with HTTPS
- No hot reload (static builds)
- Security hardening

### Deploy to Production Server

#### Step 1: Prepare Production Server

```bash
# On production Linux server
git clone <repository-url>
cd Painting_Website_Docker_NextJS/docker-compose
```

#### Step 2: Configure Environment

**2.1. Copy and Edit Environment File**

```bash
cp .env.example .env
nano .env
```

Update these values in `.env`:
```env
# CORS - your production domain
CORS_ALLOWED_ORIGINS=https://ggpaintings.com

# API URLs for production (NGINX routing)
NEXT_PUBLIC_API_URL=https://ggpaintings.com/api
SERVER_API_URL=http://api:8080/api

# NGINX ports
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443
```

**2.2. Set the PostgreSQL Password**

The database is PostgreSQL 17. The password is read from `POSTGRES_PASSWORD` in `.env`;
`secrets/postgres_password` must exist too (Compose errors if a declared secret file is
missing) but is not read by the postgres image. Keep both identical.

```bash
mkdir -p secrets
echo -n 'YourSecurePassword123!' > secrets/postgres_password
chmod 600 secrets/postgres_password
```

```env
# .env
POSTGRES_PASSWORD=YourSecurePassword123!
```

**Important:**
- Use `echo -n` to avoid a trailing newline
- File permissions should be `600` (read/write for owner only)
- The `secrets/` directory is excluded from git
- Never commit the actual password file to version control
- If the `postgres_data` volume already exists, the password is fixed at the value the
  volume was initialised with — changing it in `.env` does not rotate it

#### Step 3: Set Up SSL Certificates

Create SSL directory and add certificates:

```bash
mkdir -p nginx/ssl

# Option A: Let's Encrypt (recommended)
sudo apt-get update
sudo apt-get install certbot python3-certbot-nginx
sudo certbot certonly --standalone -d yourdomain.com -d www.yourdomain.com
sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem nginx/ssl/server.crt
sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem nginx/ssl/server.key
sudo apt install fail2ban
sudo apt install unattended-upgrades
sudo dpkg-reconfigure unattended-upgrades

# Option B: Self-signed (for Cloudflare Full mode - RECOMMENDED)
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/server.key \
  -out nginx/ssl/server.crt \
  -subj "/C=US/ST=State/L=City/O=Organization/CN=ggpaintings.com"

# Set permissions
chmod 600 nginx/ssl/server.key
chmod 644 nginx/ssl/server.crt
```

#### Step 4: Deploy

```bash
docker-compose -f docker-compose.prod.yml up -d --build
```

#### Step 5: Verify

```bash
# Check container status
docker-compose -f docker-compose.prod.yml ps

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Test health endpoints
curl -k https://localhost/api/health/health -H "Host: ggpaintings.com"
curl http://localhost:9090/health
```

Port 9090 maps to NGINX's internal 8080 health server. Port 80 is a 301 redirect only, and
`https://localhost` needs `-k` (self-signed origin cert) plus an explicit `Host:` header so
NGINX selects the right `server_name` block.

### Production Configuration

**Environment Variables** (in `.env`):
```env
NEXT_PUBLIC_API_URL=https://ggpaintings.com/api
SERVER_API_URL=http://api:8080/api
CORS_ALLOWED_ORIGINS=https://ggpaintings.com
```

**How it works:**
- Browser accesses everything at `https://ggpaintings.com`
- NGINX routes `/api/*` to API container
- NGINX routes `/*` to Frontend container
- Frontend container accesses API at `http://api:8080/api` (Docker network)
- No CORS issues (same origin)

### Access Points
- Frontend: https://ggpaintings.com
- API: https://ggpaintings.com/api
- Health: http://localhost:9090/health (host mapping of NGINX's internal 8080)
- Swagger: Disabled in production

### Stop Production Environment

```bash
docker-compose -f docker-compose.prod.yml down
```

## Cloudflare Full Mode (Home Server Deployment)

> **Read this section as "how Cloudflare TLS works", not as a separate deployment.**
> The canonical procedure is **Multi-Site Production Deployment** further down; it uses
> `docker-compose.multi.yml`. Nothing below requires a compose file of its own.

### Purpose
- Optimized for home servers with dynamic IP
- Cloudflare handles SSL at the edge (free, trusted certificates)
- NGINX terminates SSL with self-signed certificate
- Extra encryption layer between Cloudflare and your server
- No need for Let's Encrypt or domain validation

### Architecture

```
User Browser → HTTPS → Cloudflare (Edge SSL) → HTTPS → Your Server (NGINX) → HTTP → Containers
```

### Prerequisites

- Docker and Docker Compose installed on Linux server
- Domain name configured with Cloudflare as DNS provider
- Cloudflare account with site added
- Port 443 open on your server (port 80 optional)

### Deploy with Cloudflare

#### Step 1: Prepare Server

```bash
# On your Linux server
git clone <repository-url>
cd Painting_Website_Docker_NextJS/docker-compose
```

#### Step 2: Configure Environment

```bash
cp .env.example .env
nano .env
```

Update these values in `.env`:

```env
# PostgreSQL password — must match BOTH the password the existing postgres_data
# volume was initialized with AND secrets/postgres_password
POSTGRES_PASSWORD=YourSecurePassword123!

# CORS - your production domain
CORS_ALLOWED_ORIGINS=https://ggpaintings.com

# API URLs for Cloudflare Full mode
NEXT_PUBLIC_API_URL=https://ggpaintings.com/api
SERVER_API_URL=http://api:8080/api
```

#### Step 3: Generate Self-Signed SSL Certificates

`nginx/nginx.conf` is shared by both stacks and expects one certificate **per site**,
at `nginx/ssl/gg/` and `nginx/ssl/flynn/`. A missing directory is silently created as
an empty mount and NGINX then fails with *cannot load certificate*.

```bash
mkdir -p nginx/ssl/gg nginx/ssl/flynn

# Site gg
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/gg/server.key \
  -out nginx/ssl/gg/server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=ggpaintings.com"

# Site flynn
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/flynn/server.key \
  -out nginx/ssl/flynn/server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=flynnart.com"

chmod 600 nginx/ssl/gg/server.key nginx/ssl/flynn/server.key
chmod 644 nginx/ssl/gg/server.crt nginx/ssl/flynn/server.crt
```

`./scripts/generate-prod-ssl.sh` does the same thing interactively.

#### Step 4: Configure Cloudflare

1. **Add your site to Cloudflare** (if not already done)
2. **Change DNS nameservers** at your registrar to Cloudflare's
3. **Set SSL/TLS mode to "Full"**:
   - Go to SSL/TLS → Overview
   - Select "Full" (not "Flexible" or "Full (Strict)")
4. **Ensure DNS records are proxied** (orange cloud icon):
   - Go to DNS
   - Make sure A record for your domain has orange cloud

#### Step 5: Deploy

```bash
# Return to the docker-compose/ directory
cd ../..

# Recommended: pre-flight checks, cert/permission setup, database creation, deploy
./deploy-multi.sh

# Or manually:
docker compose -f docker-compose.multi.yml up -d --build
```

#### Step 6: Verify

```bash
# Container status — all 6 containers should report "healthy"
docker compose -f docker-compose.multi.yml ps

# View logs
docker compose -f docker-compose.multi.yml logs -f

# Test health endpoint (host 9090 -> NGINX container 8080)
curl http://localhost:9090/health

# Test the sites
curl -I https://ggpaintings.com
curl -I https://flynnart.com
```

### Access Points

- Frontend: https://ggpaintings.com and https://flynnart.com (via Cloudflare)
- API: https://\<domain\>/api (via Cloudflare)
- Health: http://localhost:9090/health (local only — do not expose this port)
- NGINX logs: `docker compose -f docker-compose.multi.yml logs nginx`

### Stop the Stack

```bash
docker compose -f docker-compose.multi.yml down
```

## Multi-Site Production Deployment

### Overview

The multi-site architecture runs two independent painting gallery sites on a single server:

| Site | Domain | API Container | Frontend Container | Database |
|------|--------|--------------|-------------------|----------|
| gg | ggpaintings.com | artgallery-api-gg | artgallery-frontend-gg | artgallery_gg |
| flynn | flynnart.com | artgallery-api-flynn | artgallery-frontend-flynn | artgallery_flynn |

**Shared resources:**
- Single PostgreSQL container (separate databases)
- Single NGINX container (per-site server blocks)

### Architecture

```
User Browser → HTTPS → Cloudflare (Edge SSL) → HTTPS → NGINX → HTTP → Per-Site Containers
```

### Prerequisites

- Docker and Docker Compose installed on Linux server (Ubuntu 22.04+ recommended)
- Two domain names configured with Cloudflare as DNS provider
- Cloudflare account with both sites added
- Port 443 open on your server (port 80 optional)
- At least 8GB RAM recommended (two sites + shared PostgreSQL)

> **Adding flynn to a server that already runs gg?** Read
> [Upgrading a Single-Site Server to Multi-Site](#upgrading-a-single-site-server-to-multi-site)
> before running anything. Two things do *not* happen automatically: the `artgallery_flynn`
> database is not created, and the `image-data/flynn/` bind-mount directories are not
> chowned to the API user.

### Step 1: Prepare Server

```bash
# On your Linux server
git clone <repository-url>
cd Painting_Website_Docker_NextJS/docker-compose
```

### Step 2: Configure Environment

**2.1. Copy and Edit Multi-Site Environment File**

```bash
cp .env.multi.example .env.multi
nano .env.multi
```

`.env.multi.example` already carries the correct non-secret values for both sites (theme
colours, fonts, favicons, navbar labels, contact details, OAuth redirect URIs). Copy it and
change **only** the secrets listed in 2.2 — hand-editing the whole file is how theme values
drift between the two sites.

```env
# PostgreSQL password — shared by the postgres container and both API connection strings.
# Read section 2.4 first if you are reusing an existing postgres_data volume.
POSTGRES_PASSWORD=YourSecurePassword123!

# NGINX Ports (production uses standard ports)
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443
NGINX_HEALTH_PORT=9090

# === Site: gg (ggpaintings.com) ===
GG_SITE_NAME="Gloria Gronowicz Fine Art"
GG_SITE_DESCRIPTION="Gloria Gronowicz is an oil painter who combines fine art with conservation. As a former Ph.D. scientist she seeks to tell the story of different species and their varied habitats, particularly in South Florida. Through color and light she creates a vignette of life in nature."
GG_CONTACT_EMAIL="gloriagronowicz@gmail.com"
GG_CONTACT_PHONE="860.670.0799"

# CSS Theme Variables
GG_CSS_BACKGROUND="#3d3d3d"
GG_CSS_FOREGROUND="#ffffff"
GG_CSS_NAVBAR_FOOTER_BG="#2d2d2d"
GG_CSS_TITLE_COLOR="#66b3ff"
GG_CSS_BUTTON_COLOR="#1e3a8a"

# API URLs (relative for NGINX routing)
GG_NEXT_PUBLIC_API_URL=/api
GG_SERVER_API_URL=http://api-gg:8080/api

# CORS
GG_CORS_ALLOWED_ORIGINS=https://ggpaintings.com

# Google OAuth (site gg)
GG_GOOGLE_AUTH_CLIENT_ID=YOUR_GG_CLIENT_ID.apps.googleusercontent.com
GG_GOOGLE_AUTH_CLIENT_SECRET=YOUR_GG_CLIENT_SECRET
GG_GOOGLE_AUTH_REDIRECT_URI=https://ggpaintings.com/admin/login

# Yahoo OAuth (site gg)
GG_YAHOO_AUTH_CLIENT_ID=YOUR_GG_YAHOO_CLIENT_ID
GG_YAHOO_AUTH_CLIENT_SECRET=YOUR_GG_YAHOO_CLIENT_SECRET
GG_YAHOO_AUTH_REDIRECT_URI=https://ggpaintings.com/admin/login

# Admin JWT (site gg)
GG_ADMIN_JWT_SECRET_KEY=CHANGE_THIS_TO_A_RANDOM_SECRET_KEY_GG
GG_ADMIN_JWT_EXPIRY_MINUTES=60
GG_ADMIN_AUTHORIZED_EMAILS=gloriagronowicz@gmail.com

# === Site: flynn (flynnart.com) ===
FLYNN_SITE_NAME="Terri Gray Fine Art"
FLYNN_SITE_DESCRIPTION="Fine art paintings by Flynn"
FLYNN_CONTACT_EMAIL="texasterrigray@yahoo.com"
FLYNN_CONTACT_PHONE="512-461-8075"

# CSS Theme Variables
FLYNN_CSS_BACKGROUND="#d0d0d0"
FLYNN_CSS_FOREGROUND="#000000"
FLYNN_CSS_NAVBAR_FOOTER_BG="#adaa8d"
FLYNN_CSS_TITLE_COLOR="#2d5a4a"
FLYNN_CSS_BUTTON_COLOR="#2d5a4a"

# API URLs (relative for NGINX routing)
FLYNN_NEXT_PUBLIC_API_URL=/api
FLYNN_SERVER_API_URL=http://api-flynn:8080/api

# CORS
FLYNN_CORS_ALLOWED_ORIGINS=https://flynnart.com,https://www.flynnart.com

# Google OAuth (site flynn)
FLYNN_GOOGLE_AUTH_CLIENT_ID=YOUR_FLYNN_CLIENT_ID.apps.googleusercontent.com
FLYNN_GOOGLE_AUTH_CLIENT_SECRET=YOUR_FLYNN_CLIENT_SECRET
FLYNN_GOOGLE_AUTH_REDIRECT_URI=https://flynnart.com/admin/login

# Yahoo OAuth (site flynn)
FLYNN_YAHOO_AUTH_CLIENT_ID=YOUR_FLYNN_YAHOO_CLIENT_ID
FLYNN_YAHOO_AUTH_CLIENT_SECRET=YOUR_FLYNN_YAHOO_CLIENT_SECRET
FLYNN_YAHOO_AUTH_REDIRECT_URI=https://flynnart.com/admin/login

# Admin JWT (site flynn)
FLYNN_ADMIN_JWT_SECRET_KEY=CHANGE_THIS_TO_A_RANDOM_SECRET_KEY_FLYNN
FLYNN_ADMIN_JWT_EXPIRY_MINUTES=60
FLYNN_ADMIN_AUTHORIZED_EMAILS=texasterrigray@yahoo.com
```

The flynn values above are the real production defaults in `.env.multi.example` — keep them
in sync with `.env.multi.arm64.example` if you change either one.

**2.2. OAuth Redirect URIs**

The redirect URI is a **frontend route**, not an API route. Register exactly these four
entries — one OAuth client per site, each with both apex and `www`:

```
https://ggpaintings.com/admin/login
https://www.ggpaintings.com/admin/login
https://flynnart.com/admin/login
https://www.flynnart.com/admin/login
```

The `/admin/login` page reads the `code` query parameter and POSTs it to
`/api/auth/google/callback` (or `/api/auth/yahoo/callback`), a Next.js route handler that
forwards to the .NET API. If you register `/api/auth/google/callback` as the provider
redirect URI instead, the provider sends the browser straight to the API with a `GET`,
which 405s, and login never completes.

**2.3. Generate JWT Secrets**

```bash
openssl rand -base64 32   # -> GG_ADMIN_JWT_SECRET_KEY
openssl rand -base64 32   # -> FLYNN_ADMIN_JWT_SECRET_KEY
```

Use a different value per site — the two sites must not share a signing key.

**2.4. PostgreSQL Password**

`POSTGRES_PASSWORD` in `.env.multi` is the authoritative value. `docker-compose.multi.yml`
passes it to the postgres container *and* interpolates it into both API connection strings,
so a mismatch shows up as API containers that never turn healthy.

```bash
mkdir -p secrets
echo -n 'YourSecurePassword123!' > secrets/postgres_password
chmod 600 secrets/postgres_password
```

`secrets/postgres_password` still has to exist — Compose refuses to start when a declared
secret file is missing, and `deploy-multi.sh` checks for it in its pre-flight pass. But the
official postgres image ignores `*_FILE` variables, so the file is informational only and
must contain exactly the same password as `.env.multi`.

> **Reusing an existing `postgres_data` volume?** The password is already baked into that
> volume and will *not* change just because you edited `.env.multi` — `POSTGRES_PASSWORD`
> is only honoured on first initialisation. It must match the original. Read the live value
> before you write anything:
>
> ```bash
> docker exec artgallery-postgres env | grep POSTGRES_PASSWORD
> ```

### Step 3: Generate Self-Signed SSL Certificates (Per-Site)

Each site needs its own self-signed certificate for Cloudflare Full mode:

```bash
# Site: gg
mkdir -p nginx/ssl/gg
cd nginx/ssl/gg
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout server.key \
  -out server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=ggpaintings.com"
chmod 600 server.key
chmod 644 server.crt

# Site: flynn
cd ../
mkdir -p flynn
cd flynn
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout server.key \
  -out server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=flynnart.com"
chmod 600 server.key
chmod 644 server.crt

cd ../..
```

### Step 4: Configure Cloudflare (Both Sites)

For **each** domain (ggpaintings.com and flynnart.com):

1. **Add site to Cloudflare** (if not already done)
2. **Change DNS nameservers** at your registrar to Cloudflare's
3. **Set SSL/TLS mode to "Full"**:
   - Go to SSL/TLS → Overview
   - Select "Full" (not "Flexible" or "Full (Strict)")
4. **Enable "Always Use HTTPS"**:
   - Go to SSL/TLS → Edge Certificates
5. **Ensure DNS records are proxied** (orange cloud icon):
   - Go to DNS
   - Make sure A record for your domain has orange cloud

### Step 5: Deploy

**Recommended — run the deploy script:**

```bash
chmod +x deploy-multi.sh
./deploy-multi.sh
```

`deploy-multi.sh` handles six things that a bare `docker compose up` does **not**:

| Step | What it does | Why it matters |
|------|--------------|----------------|
| 1 | `chown 101:101` on `nginx/ssl/gg`, `nginx/ssl/flynn`, `nginx/nginx.conf` | NGINX runs as UID 101 and cannot read root-owned key files |
| 2 | Creates `image-data/{gg,flynn}/{original,high-res,thumbnail}` and `chown -R 1:1` them | Docker creates missing bind-mount directories as **root**; the API runs as UID 1, so every painting upload would fail with *permission denied* |
| 3 | `docker compose down` | Clears containers left over from another compose file |
| 4 | Starts PostgreSQL and waits for `healthy` | Avoids racing the API containers |
| 5 | Creates `artgallery_gg` / `artgallery_flynn` if missing | `scripts/init-databases.sql` only runs on an **empty** volume — see the cutover section below |
| 6 | Restores the newest `artgallery_<db>_*.dump` from `$BACKUP_DIR` | Skips silently when no matching backup exists |

> **Backup filenames matter.** `scripts/backup-multi.sh` writes
> `artgallery_artgallery_<site>_<timestamp>.dump`, which is the pattern step 6 globs for. The
> single-site `scripts/backup.sh` writes `artgallery_db_<timestamp>.dump`, so a backup carried
> over from the old gg-only stack is **not** picked up automatically — restore it by hand with
> `scripts/restore-multi.sh gg <file>.dump` once the stack is up.

It also refuses to start if `secrets/postgres_password`, either SSL directory, or
`nginx/nginx.conf` is missing, and finishes by checking that no container serves traffic as
root — `id -u` for the four app containers, `docker top` process owner for PostgreSQL and
NGINX.

**Manual equivalent** (only if you cannot run the script):

```bash
# 1. Image storage — must be owned by UID 1 (the API user)
mkdir -p image-data/gg/original image-data/gg/high-res image-data/gg/thumbnail
mkdir -p image-data/flynn/original image-data/flynn/high-res image-data/flynn/thumbnail
sudo chown -R 1:1 image-data

# 2. NGINX files — must be readable by UID 101 (the nginx user)
sudo chown -R 101:101 nginx/ssl/gg nginx/ssl/flynn nginx/nginx.conf
sudo chmod 644 nginx/nginx.conf nginx/ssl/*/server.crt
sudo chmod 600 nginx/ssl/*/server.key

# 3. Build and start everything
docker compose -f docker-compose.multi.yml up -d --build
```

Skipping step 1 gives you a stack that *looks* healthy but cannot accept an image upload.
Skipping step 2 means NGINX never becomes healthy — and because it `depends_on` all four
app containers being healthy, a single unhealthy container keeps the whole site down.

### Step 6: Verify Deployment

**6.1. Check Container Status**

```bash
docker compose -f docker-compose.multi.yml ps
```

All 6 containers should show `healthy` status:
- artgallery-postgres
- artgallery-api-gg
- artgallery-frontend-gg
- artgallery-api-flynn
- artgallery-frontend-flynn
- artgallery-nginx

**6.2. Verify Non-Root Users**

`docker exec ... whoami` only reflects the real user when the image declares a `USER`. The
API and frontend images do; NGINX and PostgreSQL do not, so use `docker top` for those two:

```bash
# Images that pin a non-root USER
docker exec artgallery-api-gg id -u          # non-zero — the .NET base image's app user
docker exec artgallery-api-flynn id -u       # non-zero — the .NET base image's app user
docker exec artgallery-frontend-gg id -u     # 1001 (nextjs)
docker exec artgallery-frontend-flynn id -u  # 1001 (nextjs)

# Images that start as root and drop privileges at runtime
docker top artgallery-postgres | head -2     # USER column should be postgres
docker top artgallery-nginx | head -2        # USER column should be nginx (101)
```

A `0` / `root` process owner means the image was not rebuilt, or the privilege drop failed.
Re-run with `--build`.

> `deploy-multi.sh` runs this same sweep at the end of the deploy and prints
> `non-root (UID n)` or `non-root (user)` per container. A `WARNING` line there is real: it
> means the image was not rebuilt, or the privilege drop actually failed.

**6.3. Test Health Endpoints**

```bash
# NGINX's own health endpoint (host 9090 -> container 8080)
curl http://localhost:9090/health

# API health, direct to each container (bypasses NGINX, no cert to work around)
docker exec artgallery-api-gg    curl -fsS http://localhost:8080/api/health/health
docker exec artgallery-api-flynn curl -fsS http://localhost:8080/api/health/health

# End-to-end through NGINX (self-signed origin cert, so -k; Host selects the server block)
curl -k https://localhost/api/health/health -H "Host: ggpaintings.com"
curl -k https://localhost/api/health/health -H "Host: flynnart.com"
```

> Port 80 serves **nothing**. The only `listen 80` server block in `nginx.conf` is a
> `301` redirect to `https://$host$request_uri`, so `curl http://localhost:80/...` returns
> a redirect, not a health result. Use port 9090, or go through 443 with `-k` and an
> explicit `Host:` header — without one, SNI/`Host` of `localhost` matches no `server_name`
> and NGINX falls through to the first 443 block (gg), which will silently test the wrong
> site.

**6.4. Test Sites via Cloudflare**

```bash
# Site gg
curl -I https://ggpaintings.com
curl -I https://ggpaintings.com/api/paintings

# Site flynn
curl -I https://flynnart.com
curl -I https://flynnart.com/api/paintings
```

### Access Points

| Site | Frontend | API | Notes |
|------|----------|-----|-------|
| gg | https://ggpaintings.com | https://ggpaintings.com/api | Via Cloudflare |
| flynn | https://flynnart.com | https://flynnart.com/api | Via Cloudflare |
| Health | http://localhost:9090/health | - | Local only |

### Stop Multi-Site Environment

```bash
docker compose -f docker-compose.multi.yml down
```

### Updating Multi-Site Deployment

```bash
# Pull latest code
git pull

# Preferred: re-runs the permission, database and restore steps
./deploy-multi.sh

# Monitor logs
docker compose -f docker-compose.multi.yml logs -f
```

Prefer the script over a bare `up -d --build` for updates too: it re-applies the
`image-data` and SSL ownership fixes (which are lost if you ever recreate those paths)
and re-creates any database that went missing.

## Upgrading a Single-Site Server to Multi-Site

This is the path for a server that already runs ggpaintings.com via
`docker-compose.prod.yml` and is being moved onto `docker-compose.multi.yml` in order to
add flynnart.com. It is **not** a drop-in `up -d`. The two stacks disagree on four things,
and each one takes the existing gg site down if you skip it.

### What changes between the two stacks

| Thing | Single-site (`prod.yml`) | Multi-site (`multi.yml`) | Consequence if skipped |
|-------|--------------------------|--------------------------|------------------------|
| Database name | `artgallery` | `artgallery_gg` + `artgallery_flynn` | gg starts against an empty database and gets re-seeded with demo paintings |
| Postgres container | `artgallery-postgres-prod` | `artgallery-postgres` | Two containers want the same `postgres_data` volume; the second will not start |
| Image storage | named volume `image_data` | bind mount `./image-data/gg` | Every existing painting 404s — the DB rows still point at files that are no longer mounted |
| SSL layout | `./nginx/ssl` mounted whole at `/etc/nginx/ssl` | `./nginx/ssl/gg` and `./nginx/ssl/flynn` mounted separately | `nginx.conf` asks for `/etc/nginx/ssl/gg/server.crt`, which does not exist, and NGINX exits |

Both compose files mount the **same** `./nginx/nginx.conf`, and that file now contains only
per-site server blocks pointing at the `api_gg` / `frontend_gg` / `api_flynn` /
`frontend_flynn` upstreams. Those resolve to service names that exist only in
`multi.yml`, so the two stacks cannot run side by side. Treat this as a **cutover**, not an
addition.

### Cutover procedure

Expect 5–15 minutes of downtime for ggpaintings.com. Schedule a window.

**1. Back up the current gg database and images**

```bash
cd ~/Painting_Website_Docker_NextJS/docker-compose
mkdir -p backups

# Database — note the single-site container and database names
docker exec artgallery-postgres-prod \
  pg_dump -U postgres -Fc -f /tmp/gg.dump artgallery
docker cp artgallery-postgres-prod:/tmp/gg.dump \
  ./backups/artgallery_artgallery_gg_$(date +%Y%m%d_%H%M%S).dump

# Images — move the named volume into the bind mount multi.yml expects
docker volume ls | grep image_data   # confirm the exact volume name first
docker run --rm -v docker-compose_image_data:/from -v "$PWD/image-data:/to" alpine \
  sh -c 'mkdir -p /to/gg && cp -a /from/. /to/gg/'
```

Keep the `artgallery_artgallery_gg_` prefix on the dump filename — that is the exact glob
`deploy-multi.sh` and `restore-multi.sh` search for. Backups produced by the old
`scripts/backup.sh` are named `artgallery_db_*.dump` and will **not** be auto-detected by
the multi-site scripts.

**2. Move the SSL certificates into per-site folders**

```bash
mkdir -p nginx/ssl/gg nginx/ssl/flynn

# Reuse the existing flat cert for gg, if you have one
[ -f nginx/ssl/server.crt ] && cp nginx/ssl/server.crt nginx/ssl/server.key nginx/ssl/gg/

# Generate a fresh self-signed pair for flynn (full command in Step 3 above)
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/flynn/server.key -out nginx/ssl/flynn/server.crt \
  -subj "/C=US/ST=Texas/L=Austin/O=Terri Gray Fine Art/CN=flynnart.com"
```

**3. Stop the single-site stack completely**

```bash
docker compose -f docker-compose.prod.yml down
docker ps --format '{{.Names}}'   # must be empty before continuing
```

If `artgallery-nginx` is still up, port 443 stays bound and the new NGINX cannot start.

**4. Confirm the PostgreSQL password still matches**

```bash
grep POSTGRES_PASSWORD .env.multi
```

The reused `postgres_data` volume already has its password baked in and will ignore a new
one. `.env.multi` must contain the *original* value — see section 2.4.

**5. Run the deploy script**

```bash
./deploy-multi.sh
```

It creates `artgallery_gg` and `artgallery_flynn` (step 5/8), chowns `image-data/` to
UID 1 (step 2/8), and restores the newest matching dump (step 6/8).

**6. Verify before calling it done**

```bash
# Both databases exist
docker exec artgallery-postgres psql -U postgres -lqt | cut -d'|' -f1 | grep artgallery

# gg kept its real data (table names are quoted — they are case-sensitive in PostgreSQL)
docker exec artgallery-postgres psql -U postgres -d artgallery_gg -c 'SELECT COUNT(*) FROM "Paintings";'

# Each site serves its own content, not gg's
curl -k https://localhost/api/paintings -H "Host: ggpaintings.com" | head -c 400
curl -k https://localhost/api/paintings -H "Host: flynnart.com"   | head -c 400
```

A non-zero painting count on gg means the restore worked. If `artgallery_gg` came up empty,
the dump was skipped — re-check the filename against the glob in step 1.

### After the cutover

- Add flynnart.com to Cloudflare and set SSL/TLS to **Full** (Step 4).
- Register the flynn OAuth client with `https://flynnart.com/admin/login` and
  `https://www.flynnart.com/admin/login` (section 2.2).
- Re-point the backup cron: run `scripts/install-backup-cron-multi.sh`, then `crontab -e`
  and remove the old single-site entry so two jobs are not writing to the same directory.
- `image-data/` is **not** gitignored — 167 painting files are currently tracked. Adding
  flynn's paintings will keep growing the repo; consider moving image storage to object
  storage or adding `image-data/` to `.gitignore` before the next commit.

### Multi-Site Local Testing

For local development testing with both sites:

```bash
# Use the local override (requires .env.multi.local)
docker compose -f docker-compose.multi.yml -f docker-compose.multi.local.yml up -d --build
```

**Local access points:**
- Site gg: http://localhost:8181 (via NGINX) or http://localhost:3001 (direct)
- Site flynn: http://localhost:8182 (via NGINX) or http://localhost:3002 (direct)
- Health: http://localhost:9090/health

### Multi-Site Security Checklist

- [ ] PostgreSQL password set in `secrets/postgres_password`
- [ ] Self-signed SSL certificates generated for both sites (`nginx/ssl/gg/` and `nginx/ssl/flynn/`)
- [ ] `.env.multi` file configured with production values
- [ ] JWT secrets are unique random strings for each site
- [ ] Google OAuth client IDs configured separately for each site
- [ ] CORS origins set to production domains
- [ ] `.env.multi` file not committed to git
- [ ] Firewall configured (ports 80, 443 only)
- [ ] Cloudflare SSL/TLS mode set to "Full" for both domains
- [ ] Cloudflare DNS records proxied (orange cloud enabled) for both domains

## Switching Between Modes

### From Development to Production

1. **Stop development:**
   ```bash
   docker-compose down
   ```

2. **Update .env for production:**
    ```env
    NEXT_PUBLIC_API_URL=https://ggpaintings.com/api
    SERVER_API_URL=http://api:8080/api
    CORS_ALLOWED_ORIGINS=https://ggpaintings.com
    ```

3. **Deploy production:**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d --build
   ```

### From Production to Development

1. **Stop production:**
   ```bash
   docker-compose -f docker-compose.prod.yml down
   ```

2. **Update .env for development:**
   ```env
   NEXT_PUBLIC_API_URL=http://localhost:8080/api
   SERVER_API_URL=http://api:8080/api
   CORS_ALLOWED_ORIGINS=http://localhost:3000
   ```

3. **Start development:**
   ```bash
   docker-compose up -d
   ```

## Key Differences

| Feature | Development | Production |
|---------|-------------|------------|
| Compose File | `docker-compose.yml` | `docker-compose.prod.yml` |
| Build Target | `development` | `production` |
| Hot Reload | ✅ Yes | ❌ No |
| NGINX | ❌ No | ✅ Yes |
| HTTPS | ❌ No | ✅ Yes |
| Swagger | ✅ Enabled | ❌ Disabled |
| Logging | Information | Warning |
| Database | PostgreSQL 17, one `artgallery` database | PostgreSQL 17, `artgallery_gg` + `artgallery_flynn` |
| API URL (Browser) | `http://localhost:8080/api` | `/api` |
| CORS Required | ✅ Yes | ❌ No (same origin) |
| Restart Policy | None | `unless-stopped` |

## Configuration Details

### NGINX Configuration

The NGINX reverse proxy handles:
- HTTPS termination
- HTTP to HTTPS redirect
- Routing to API and frontend services
- Security headers (HSTS, X-Frame-Options, etc.)
- Gzip compression

Configuration file: `nginx/nginx.conf`

### API Configuration

Production settings in `ServerApp/ServerApp.Api/appsettings.Production.json`:
- Reduced logging (Warning level)
- Swagger disabled
- Health check endpoints enabled

### Database Configuration

- PostgreSQL 17 (`postgres:17-alpine`) — the same image is used in development and production
- Persistent `postgres_data` volume for data
- A `pg_isready` health check gates API startup (`depends_on: condition: service_healthy`)
- The single-site stack uses one database (`artgallery`); the multi-site stack runs one shared
  PostgreSQL container holding one database per site (`artgallery_gg`, `artgallery_flynn`)
- The password comes from the `POSTGRES_PASSWORD` environment variable, **not** from a Docker
  secret — the official PostgreSQL image ignores `*_FILE` variables. PostgreSQL only reads it
  when the data directory is empty, so on an existing `postgres_data` volume the value must
  match whatever the volume was originally initialised with.

## Updating Application Code

### In Development

1. Make changes to source code
2. Changes automatically hot-reload
3. No rebuild needed

### In Production

1. Pull latest code:
   ```bash
   git pull
   ```

2. Rebuild and redeploy:
   ```bash
   docker-compose -f docker-compose.prod.yml up -d --build
   ```

3. Monitor logs:
   ```bash
   docker-compose -f docker-compose.prod.yml logs -f
   ```

## Monitoring and Maintenance

### View Logs

```bash
# All services
docker-compose -f docker-compose.prod.yml logs -f

# Specific service (use the compose *service* name, not the container name)
docker-compose -f docker-compose.prod.yml logs -f api
docker-compose -f docker-compose.prod.yml logs -f frontend
docker-compose -f docker-compose.prod.yml logs -f postgres
docker-compose -f docker-compose.prod.yml logs -f nginx
```

### Restart Services

```bash
# Restart all services
docker-compose -f docker-compose.prod.yml restart

# Restart specific service
docker-compose -f docker-compose.prod.yml restart api
```

### Database Backup

Use the scripts in `scripts/` rather than ad-hoc commands. Both run `pg_dump -Fc` inside the
PostgreSQL container, copy the dump to the host, write a `.sha256` checksum, and prune dumps
older than the retention window:

```bash
cd docker-compose

# Single-site stack (container artgallery-postgres-prod, database artgallery)
./scripts/backup.sh

# Multi-site stack (container artgallery-postgres, databases artgallery_gg + artgallery_flynn)
./scripts/backup-multi.sh              # both sites
./scripts/backup-multi.sh flynn        # flynn only
```

Manual equivalent for the single-site stack:

```bash
docker exec artgallery-postgres-prod pg_dump -U postgres -Fc -f /tmp/artgallery.dump artgallery
docker cp artgallery-postgres-prod:/tmp/artgallery.dump ./artgallery.dump
```

> **Filename gotcha:** `backup.sh` writes `artgallery_db_<timestamp>.dump`, but
> `deploy-multi.sh` and `restore-multi.sh` only look for `artgallery_<database>_*.dump`
> (e.g. `artgallery_artgallery_gg_20261101_020000.dump`). A single-site backup is therefore
> silently skipped during the multi-site cutover. Either rename it to
> `artgallery_artgallery_gg_<timestamp>.dump` before deploying, or restore it explicitly with
> `./scripts/restore-multi.sh gg <file>.dump`, which accepts an explicit filename.

### Database Restore

```bash
# Copy the dump into the container
docker cp ./artgallery.dump artgallery-postgres-prod:/tmp/artgallery.dump

# Drop, recreate, then restore into the fresh database
docker exec artgallery-postgres-prod psql -U postgres -c "DROP DATABASE IF EXISTS artgallery;"
docker exec artgallery-postgres-prod psql -U postgres -c "CREATE DATABASE artgallery;"
docker exec -i artgallery-postgres-prod pg_restore -U postgres -d artgallery /tmp/artgallery.dump
```

For the multi-site stack prefer `./scripts/restore-multi.sh <gg|flynn> [dumpfile]`. It stops
the affected API container first, verifies the checksum, restores into the correct database,
and restarts the API afterwards.

## Security Considerations

### Environment Variables

- Set `POSTGRES_PASSWORD` to a strong, unique password. When you reuse an existing
  `postgres_data` volume it **must** match the password that volume was initialised with,
  otherwise every API connection fails with `password authentication failed`
- Never commit `.env` / `.env.multi` / `.env.multi.arm64` to version control
- Those files, plus everything under `secrets/`, are already excluded by
  `docker-compose/.gitignore`

### CORS Configuration

Update `CORS_ALLOWED_ORIGINS` with your production domain:
```env
CORS_ALLOWED_ORIGINS=https://ggpaintings.com
```

### SSL/TLS Auto-Renewal

If using Let's Encrypt, configure auto-renewal:
```bash
sudo crontab -e
# Add this line for auto-renewal
0 0 * * * /usr/bin/certbot renew --quiet && docker-compose -f docker-compose.prod.yml restart nginx
```

### Firewall Configuration

Open only necessary ports:
```bash
# Ubuntu/Debian with UFW
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
```

## Troubleshooting

### Frontend Can't Reach API

**Development:**
- Check if API container is running: `docker-compose ps`
- Verify port 8080 is exposed: `docker-compose port api 8080`

**Production:**
- Check NGINX configuration: `docker-compose -f docker-compose.prod.yml logs nginx`
- The `api` hostname only resolves inside the Docker network, so run it from a container:
  `docker exec artgallery-api-prod curl -f http://localhost:8080/api/health/health`
- End-to-end from the host (self-signed cert, so `-k`):
  `curl -k https://localhost/api/health/health -H "Host: ggpaintings.com"`

### CORS Errors in Development

Update `.env`:
```env
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:3001
```

Restart API:
```bash
docker-compose restart api
```

### SSL Certificate Errors

The single-site stack keeps its certificates in `nginx/ssl/` (mounted whole at
`/etc/nginx/ssl`). The multi-site stack expects one directory per site — `nginx/ssl/gg/` and
`nginx/ssl/flynn/` — each containing `server.crt` and `server.key`.

```bash
# Single-site
ls -la nginx/ssl/
chmod 600 nginx/ssl/server.key
chmod 644 nginx/ssl/server.crt

# Multi-site
for d in nginx/ssl/gg nginx/ssl/flynn; do
  chmod 600 "$d/server.key"
  chmod 644 "$d/server.crt"
done
```

`deploy-multi.sh` performs these `chmod`s (and `chown 101:101`) for you, and refuses to
deploy if either site's certificate pair is missing.

Restart NGINX:
```bash
docker-compose -f docker-compose.prod.yml restart nginx
```

### API Won't Start

Check API logs:
```bash
docker-compose -f docker-compose.prod.yml logs api
```

Common issues:
- Database not ready (wait for health check)
- Connection string incorrect
- Port already in use

### Frontend Build Fails

Check frontend logs:
```bash
docker-compose -f docker-compose.prod.yml logs frontend
```

Common issues:
- API URL incorrect
- Missing environment variables
- Node modules cache issues (rebuild)

### Database Connection Issues

Verify PostgreSQL is running and reachable:

```bash
docker-compose -f docker-compose.prod.yml logs postgres

# Should report "accepting connections"
docker exec artgallery-postgres-prod pg_isready -U postgres -d artgallery

# Should print: 1
docker exec artgallery-postgres-prod psql -U postgres -tAc "SELECT 1"
```

If the API logs `password authentication failed`, the `POSTGRES_PASSWORD` in your env file
does not match the one the existing `postgres_data` volume was created with. Change it back
to the original value, or recreate the volume — editing it in place does nothing, because
PostgreSQL only reads `POSTGRES_PASSWORD` while the data directory is empty.

## Performance Optimization

### Application Monitoring (Optional)

The API exposes `/api/health/health` on its internal 8080 listener, and NGINX exposes
`/health` on the same internal port (published to host port 9090). Point monitoring at those
two endpoints, or ship logs with a Docker logging driver, before reaching for an APM product.

### Database Optimization

- Add indexes for frequently queried columns
- Configure connection pooling
- Monitor query performance

### Caching

- NGINX handles static file caching
- Next.js handles page caching (configured in `next.config.ts`)
- Consider adding Redis for session caching

## Scaling Considerations

For high-traffic deployments:

1. **Load Balancing**: Use multiple API instances behind NGINX
2. **Database**: PostgreSQL needs no licence upgrade. Tune `shared_buffers` and
   `max_connections` via a custom `postgresql.conf`, or move PostgreSQL to its own host /
   a managed PostgreSQL service before adding more sites
3. **CDN**: Cloudflare already fronts both sites; extend it to cache static assets
4. **Container Registry**: Build in CI and pull a prebuilt image instead of building on
   the production server

## Quick Reference

### Development Commands

```bash
# Start
docker-compose up -d

# Stop
docker-compose down

# Stop and remove data
docker-compose down -v

# View logs
docker-compose logs -f

# Rebuild
docker-compose up -d --build
```

### Production Commands

```bash
# Deploy
docker-compose -f docker-compose.prod.yml up -d --build

# Stop
docker-compose -f docker-compose.prod.yml down

# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Restart specific service
docker-compose -f docker-compose.prod.yml restart api
```

## Security Checklist for Production

- [ ] `POSTGRES_PASSWORD` set to a strong password that matches the existing `postgres_data` volume
- [ ] SSL certificates installed and configured (self-signed for Cloudflare Full mode)
- [ ] `CORS_ALLOWED_ORIGINS` set to `https://ggpaintings.com`
- [ ] `.env` file not committed to git
- [ ] Firewall configured (ports 80, 443 only)
- [ ] Database backups scheduled
- [ ] Cloudflare SSL/TLS mode set to "Full"
- [ ] Cloudflare DNS records proxied (orange cloud enabled)

### Security Hardening (Already Configured)

The following security features are already enabled in the deployment:

- ✅ API and frontend containers declare a non-root `USER` (API: the .NET base image's
  `$APP_UID`; frontend: `nextjs`, UID 1001, also pinned with `user: "1001:1001"`)
- ✅ NGINX and PostgreSQL start as root but drop privileges before serving traffic — NGINX via
  `su-exec 101` in `nginx/entrypoint.sh`, PostgreSQL via its own init scripts. Neither image
  declares a `USER`, so `docker exec` on those two still lands as `root`
- ✅ Read-only filesystem for the API, frontend, and NGINX containers
- ✅ Health checks on all containers
- ✅ Security headers (HSTS, X-Frame-Options, X-Content-Type-Options, X-XSS-Protection)
- ✅ Cloudflare real IP configuration for accurate logging

Database connections are **not** encrypted. The connection string
(`Host=postgres;Port=5432;Database=...;Username=postgres;Password=...`) carries no `sslmode`,
and PostgreSQL is only reachable over the private `artgallery-network` — no host port is
published. (`TrustServerCertificate=True` was a SQL Server setting and no longer applies.)

### Verify Security Configuration

After deployment, verify the security configuration.

> `docker exec ... whoami` only reflects a container's real user when the image declares a
> `USER`. The API and frontend images do; the NGINX and PostgreSQL images do not, so
> `docker exec` lands as `root` on those two even though their long-running process has
> dropped privileges. Check those with `docker top` instead.

```bash
# Images that pin a non-root USER — docker exec reports the real user
docker exec artgallery-api-prod id -u         # non-zero ($APP_UID from the .NET base image)
docker exec artgallery-frontend-prod id -u    # 1001 (nextjs)

# Images that start as root and drop privileges at runtime — inspect the running process
docker top artgallery-nginx | head -2         # USER column should be nginx (101)
docker top artgallery-postgres-prod | head -2 # USER column should be postgres

# Verify read-only filesystem
docker inspect artgallery-api-prod --format '{{.HostConfig.ReadonlyRootfs}}'       # true
docker inspect artgallery-frontend-prod --format '{{.HostConfig.ReadonlyRootfs}}'   # true
docker inspect artgallery-nginx --format '{{.HostConfig.ReadonlyRootfs}}'           # true

# Verify health checks
docker-compose -f docker-compose.prod.yml ps  # All containers should show healthy status
```

## Support

For issues or questions:
- Check logs: `docker-compose -f docker-compose.prod.yml logs -f`
- Review NGINX config: `nginx/nginx.conf`
- Verify environment variables: `cat .env`