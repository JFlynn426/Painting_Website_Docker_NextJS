
# Deployment Guide: Art Gallery Application

**Domain:** ggpaintings.com
**Last Updated:** 2026-04-09

This guide explains how to deploy the Art Gallery application to production with Cloudflare.

## Overview

The solution uses three docker-compose files:
- **`docker-compose.yml`** - Development mode (hot reload, debug mode)
- **`docker-compose.prod.yml`** - Production mode (optimized builds, NGINX reverse proxy with SSL)
- **`docker-compose.cloudflare.yml`** - Cloudflare Full mode (NGINX with self-signed SSL, Cloudflare handles edge SSL)

## Deployment Options

| Mode | Use Case | SSL | Best For |
|------|----------|-----|----------|
| Development | Local development | None | Coding, testing |
| Production (NGINX) | Self-hosted with valid SSL | Let's Encrypt or commercial | Public servers with domain |
| **Cloudflare Full** | **Home server with Cloudflare** | **Self-signed (NGINX) + Cloudflare** | **Home servers, dynamic IP** |

## Security Features

This deployment includes the following security hardening:

- ✅ **All containers run as non-root users** (API: UID 1, Frontend: UID 1001, NGINX: UID 101, SQL Server: UID 10001)
- ✅ **Read-only filesystem** for API, Frontend, and NGINX containers
- ✅ **Health checks** on all containers for monitoring
- ✅ **Docker secrets** for SQL Server password (not exposed in `docker inspect`)
- ✅ **TLS encryption** for database connections
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
                                ┌────────────┐
                                │  SQL Server│
                                │  (UID 10001)│
                                └────────────┘
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

**3.2. Create Docker Secret for SQL Server Password**

For enhanced security, the SQL Server password is stored in a Docker secret file instead of environment variables. This prevents password exposure in `docker inspect` output.

```bash
mkdir -p secrets
echo -n 'YourSecurePassword123!' > secrets/sqlserver_sa_password
chmod 600 secrets/sqlserver_sa_password
```

**Important:**
- Use `echo -n` to avoid trailing newline
- The file permissions should be `600` (read/write for owner only)
- The `secrets/` directory is already excluded from git (see `.gitignore`)
- Never commit the actual password file to version control

**Why Docker Secrets?**
- Password not visible in `docker inspect container_name --format '{{.Config.Env}}'`
- Password not visible in process environment
- More secure than environment variables for sensitive data

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

```bash
docker exec artgallery-api whoami        # Should return: app
docker exec artgallery-frontend whoami   # Should return: node
docker exec artgallery-nginx whoami      # Should return: nginx
```

**6.3. Verify Resource Limits**

```bash
# Check API container limits
docker inspect artgallery-api --format '{{.HostConfig.NanoCpus}}'        # Should show: 2000000000 (2 CPUs)
docker inspect artgallery-api --format '{{.HostConfig.Memory}}'          # Should show: 4294967296 (4GB)

# Check Frontend container limits
docker inspect artgallery-frontend --format '{{.HostConfig.NanoCpus}}'   # Should show: 1000000000 (1 CPU)
docker inspect artgallery-frontend --format '{{.HostConfig.Memory}}'     # Should show: 2147483648 (2GB)

# Check NGINX container limits
docker inspect artgallery-nginx --format '{{.HostConfig.NanoCpus}}'      # Should show: 1000000000 (1 CPU)
docker inspect artgallery-nginx --format '{{.HostConfig.Memory}}'        # Should show: 1073741824 (1GB)

# Check SQL Server container limits
docker inspect artgallery-sql-prod --format '{{.HostConfig.NanoCpus}}'   # Should show: 4000000000 (4 CPUs)
docker inspect artgallery-sql-prod --format '{{.HostConfig.Memory}}'     # Should show: 8589934592 (8GB)
```

**6.4. Verify Docker Secrets (Password Not Exposed)**

```bash
# Verify password is NOT in environment variables
docker inspect artgallery-sql-prod --format '{{.Config.Env}}' | grep -i password
# Should return: (empty - no password in env)

# Verify secret file exists
docker exec artgallery-sql-prod cat /run/secrets/sqlserver_sa_password
# Should return: YourSecurePassword123!
```

**6.5. Verify Read-Only Filesystems**

```bash
docker inspect artgallery-api --format '{{.HostConfig.ReadonlyRootfs}}'      # Should return: true
docker inspect artgallery-frontend --format '{{.HostConfig.ReadonlyRootfs}}'  # Should return: true
docker inspect artgallery-nginx --format '{{.HostConfig.ReadonlyRootfs}}'     # Should return: true
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
- SQL Server: localhost:1433

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

**2.2. Create Docker Secret for SQL Server Password**

For enhanced security, create a Docker secret file for the SQL Server password:

```bash
mkdir -p secrets
echo -n 'YourSecurePassword123!' > secrets/sqlserver_sa_password
chmod 600 secrets/sqlserver_sa_password
```

**Important:**
- Use `echo -n` to avoid trailing newline
- File permissions should be `600` (read/write for owner only)
- The `secrets/` directory is excluded from git
- Never commit the actual password file to version control

**Why Docker Secrets?**
- Password not visible in `docker inspect` output
- Password not exposed in process environment variables
- More secure than environment variables for sensitive data

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
curl https://ggpaintings.com/api/health/health
curl http://localhost:8080/health
```

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
- Health: http://localhost:8080/health
- Swagger: Disabled in production

### Stop Production Environment

```bash
docker-compose -f docker-compose.prod.yml down
```

## Cloudflare Full Mode (Home Server Deployment)

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
# SQL Server password (change to secure password)
SQLSERVER_SA_PASSWORD=YourSecurePassword123!

# CORS - your production domain
CORS_ALLOWED_ORIGINS=https://ggpaintings.com

# API URLs for Cloudflare Full mode
NEXT_PUBLIC_API_URL=https://ggpaintings.com/api
SERVER_API_URL=http://api:8080/api
```

#### Step 3: Generate Self-Signed SSL Certificate

```bash
cd nginx/ssl

openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout server.key \
  -out server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=ggpaintings.com"

chmod 600 server.key
chmod 644 server.crt
```

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
cd ../..
docker-compose -f docker-compose.cloudflare.yml up -d --build
```

#### Step 6: Verify

```bash
# Check container status
docker-compose -f docker-compose.cloudflare.yml ps

# View logs
docker-compose -f docker-compose.cloudflare.yml logs -f

# Test health endpoint
curl http://localhost:8080/health

# Test your site
curl -I https://ggpaintings.com
```

### Access Points

- Frontend: https://ggpaintings.com (via Cloudflare)
- API: https://ggpaintings.com/api (via Cloudflare)
- Health: http://localhost:8080/health (local only)
- NGINX logs: `docker-compose -f docker-compose.cloudflare.yml logs nginx`

### Stop Cloudflare Environment

```bash
docker-compose -f docker-compose.cloudflare.yml down
```

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
| SQL Server | Developer edition | Web edition |
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

- SQL Server Web edition (lighter than Developer edition)
- Persistent volume for data
- Health checks for startup ordering

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

# Specific service
docker-compose -f docker-compose.prod.yml logs -f api
docker-compose -f docker-compose.prod.yml logs -f frontend
docker-compose -f docker-compose.prod.yml logs -f sqlserver
```

### Restart Services

```bash
# Restart all services
docker-compose -f docker-compose.prod.yml restart

# Restart specific service
docker-compose -f docker-compose.prod.yml restart api
```

### Database Backup

```bash
# Backup database
docker-compose -f docker-compose.prod.yml exec sqlserver \
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SQLSERVER_SA_PASSWORD} \
  -Q "BACKUP DATABASE ArtGallery TO DISK='/var/opt/mssql/backups/ArtGallery.bak'"

# Copy backup to host
docker-compose -f docker-compose.prod.yml cp sqlserver:/var/opt/mssql/backups/ArtGallery.bak ./ArtGallery.bak
```

### Database Restore

```bash
# Copy backup to container
docker-compose -f docker-compose.prod.yml cp ./ArtGallery.bak sqlserver:/var/opt/mssql/backups/ArtGallery.bak

# Restore database
docker-compose -f docker-compose.prod.yml exec sqlserver \
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SQLSERVER_SA_PASSWORD} \
  -Q "RESTORE DATABASE ArtGallery FROM DISK='/var/opt/mssql/backups/ArtGallery.bak'"
```

## Security Considerations

### Environment Variables

- Change `SQLSERVER_SA_PASSWORD` to a strong, unique password
- Never commit `.env` file to version control
- Add `.env` to `.gitignore`

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
- Verify API health: `curl http://api:8080/api/health/health`

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

Verify certificates exist and have correct permissions:
```bash
ls -la nginx/ssl/
chmod 600 nginx/ssl/server.key
chmod 644 nginx/ssl/server.crt
```

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

Verify SQL Server is running and accessible:
```bash
docker-compose -f docker-compose.prod.yml logs sqlserver
docker-compose -f docker-compose.prod.yml exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${SQLSERVER_SA_PASSWORD} -Q "SELECT 1"
```

## Performance Optimization

### Application Insights (Optional)

For production monitoring, consider adding Application Insights:
1. Create App Service in Azure Portal
2. Add connection string to `.env`
3. Update `appsettings.Production.json` with instrumentation key

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
2. **Database**: Upgrade to SQL Server Standard/Enterprise
3. **CDN**: Use CloudFlare or Azure CDN for static assets
4. **Container Registry**: Use Azure Container Registry or Docker Hub

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

- [ ] Changed `SQLSERVER_SA_PASSWORD` to strong password
- [ ] SSL certificates installed and configured (self-signed for Cloudflare Full mode)
- [ ] `CORS_ALLOWED_ORIGINS` set to `https://ggpaintings.com`
- [ ] `.env` file not committed to git
- [ ] Firewall configured (ports 80, 443 only)
- [ ] Database backups scheduled
- [ ] Cloudflare SSL/TLS mode set to "Full"
- [ ] Cloudflare DNS records proxied (orange cloud enabled)

### Security Hardening (Already Configured)

The following security features are already enabled in the deployment:

- ✅ All containers run as non-root users (API: UID 1, Frontend: UID 1001, NGINX: UID 101, SQL Server: UID 10001)
- ✅ Read-only filesystem for API, Frontend, and NGINX containers
- ✅ Health checks on all containers
- ✅ TLS encryption for database connections (`TrustServerCertificate=True`)
- ✅ Security headers (HSTS, X-Frame-Options, X-Content-Type-Options, X-XSS-Protection)
- ✅ Cloudflare real IP configuration for accurate logging

### Verify Security Configuration

After deployment, verify the security configuration:

```bash
# Verify containers run as non-root
docker exec artgallery-api whoami        # Should return: app
docker exec artgallery-frontend whoami  # Should return: node
docker exec artgallery-nginx whoami     # Should return: nginx
docker exec artgallery-sql-prod whoami  # Should return: mssql

# Verify container users
docker exec artgallery-api id           # Should return: uid=1(app)
docker exec artgallery-frontend id      # Should return: uid=1001(node)
docker exec artgallery-nginx id         # Should return: uid=101(nginx)

# Verify read-only filesystem
docker inspect artgallery-api --format '{{.HostConfig.ReadonlyRootfs}}'  # Should return: true
docker inspect artgallery-frontend --format '{{.HostConfig.ReadonlyRootfs}}'  # Should return: true
docker inspect artgallery-nginx --format '{{.HostConfig.ReadonlyRootfs}}'  # Should return: true

# Verify health checks
docker-compose -f docker-compose.prod.yml ps  # All containers should show healthy status
```

## Support

For issues or questions:
- Check logs: `docker-compose -f docker-compose.prod.yml logs -f`
- Review NGINX config: `nginx/nginx.conf`
- Verify environment variables: `cat .env`