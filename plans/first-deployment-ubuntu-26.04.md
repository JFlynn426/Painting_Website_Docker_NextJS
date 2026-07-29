# First-Time Deployment Guide - Ubuntu Server 26.04 LTS

**Domains:** ggpaintings.com, flynnart.com
**Target OS:** Ubuntu Server 26.04 LTS
**Last Updated:** 2026-07-29

This guide provides the commands needed for first-time deployment on a fresh Ubuntu Server 26.04 LTS instance. Assumes the file structure is pulled from git.

## Deployment Modes

| Mode | Script | Compose File | Use Case |
|------|--------|-------------|----------|
| **Multi-Site** (Recommended) | `deploy-multi.sh` | `docker-compose.multi.yml` | Host ggpaintings.com + flynnart.com on same server |
| **Single-Site** | `deploy.sh` | `docker-compose.prod.yml` | Host a single site |

> **This guide focuses on multi-site deployment.** For single-site, use `deploy.sh` and `docker-compose.prod.yml` instead.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    INTERNET                                  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              CLOUDFLARE EDGE (Security Layer)               │
│  • DDoS Protection                                          │
│  • WAF + Rate Limiting                                      │
│  • SSL/TLS Termination (Edge)                               │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTPS (Encrypted)
                         ▼
┌─────────────────────────────────────────────────────────────┐
│         Ubuntu Server 26.04 LTS (ggpaintings.com)           │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  NGINX Reverse Proxy (UID 101, Read-only)             │  │
│  │  • Self-signed SSL Termination                        │  │
│  │  • Routes /api/* → API container                      │  │
│  │  • Routes /* → Frontend container                     │  │
│  └─────────────┬───────────────────────┬─────────────────┘  │
│                │                       │                    │
│     ┌──────────▼──────────┐   ┌───────▼────────┐           │
│     │  Frontend (Next.js) │   │  API (.NET 8)  │           │
│     │  UID 1001           │   │  UID 1 (app)   │           │
│     │  Read-only          │   │  Read-only     │           │
│     └─────────────────────┘   └────────┬───────┘           │
│                                        │                    │
│                              ┌─────────▼────────┐           │
│                              │  PostgreSQL 17   │           │
│                              │  Alpine          │           │
│                              └──────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

## Container Names (Multi-Site Deployment)

| Service | Container Name | User | Port |
|---------|---------------|------|------|
| PostgreSQL | `artgallery-postgres` | postgres | 5432 (internal) |
| API (GG) | `artgallery-api-gg` | appuser | 8080 (internal) |
| API (Flynn) | `artgallery-api-flynn` | appuser | 8080 (internal) |
| Frontend (GG) | `artgallery-frontend-gg` | nextjs (UID 1001) | 3000 (internal) |
| Frontend (Flynn) | `artgallery-frontend-flynn` | nextjs (UID 1001) | 3000 (internal) |
| NGINX | `artgallery-nginx` | nginx (UID 101) | 80, 443, 9090 (health) |

> **Single-site deployment** uses `artgallery-postgres-prod`, `artgallery-api-prod`, `artgallery-frontend-prod` instead.

## Prerequisites

- Fresh Ubuntu Server 26.04 LTS installation
- SSH access with sudo privileges
- Domain `ggpaintings.com` pointing to server IP (via Cloudflare DNS)
- Cloudflare account with domain added
- Git repository access

---

# STEP 1: System Update and Prerequisites

```bash
# Update system packages
sudo apt update && sudo apt upgrade -y

# Install required dependencies
sudo apt install -y git curl wget ca-certificates lsb-release software-properties-common ufw

# Install Docker GPG key and repository
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker Engine and Docker Compose Plugin (v2)
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Start and enable Docker
sudo systemctl start docker
sudo systemctl enable docker

# Verify Docker installation
docker --version
docker compose version

# Add current user to docker group (avoid sudo for docker commands)
sudo usermod -aG docker $USER

# Apply group changes (logout and login, or run:)
newgrp docker

# Verify docker works without sudo
docker ps
```

---

# STEP 2: Firewall Configuration

```bash
# Enable UFW
sudo ufw default deny incoming
sudo ufw default allow outgoing

# Allow SSH (critical - do not lock yourself out)
sudo ufw allow ssh

# Allow HTTP (port 80) - for Cloudflare health checks and HTTP→HTTPS redirect
sudo ufw allow 80/tcp

# Allow HTTPS (port 443) - for Cloudflare Full SSL mode
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status verbose
```

**Note:** Do NOT open ports 3000, 8080, or 5432. These are internal Docker network ports and should not be exposed to the internet.

---

# STEP 3: Clone Repository and Setup Directory Structure

```bash
# Navigate to home directory
cd ~

# Clone the repository
git clone <repository-url> Painting_Website_Docker_NextJS

# Navigate to docker-compose directory
cd ~/Painting_Website_Docker_NextJS/docker-compose

# Create /opt/artgallery directory structure (used by backup scripts)
sudo mkdir -p /opt/artgallery/backups
sudo mkdir -p /opt/artgallery/scripts
sudo chmod 755 /opt/artgallery/backups

# Create Docker volumes directory
sudo mkdir -p /opt/artgallery/volumes/postgres
sudo mkdir -p /opt/artgallery/volumes/images
```

---

# STEP 4: Configure Environment File

```bash
# Navigate to docker-compose directory
cd ~/Painting_Website_Docker_NextJS/docker-compose

# For multi-site deployment, copy the multi-site template:
cp .env.multi.example .env.multi

# Edit the multi-site environment file
nano .env.multi
```

**Required `.env.multi` configuration for production:**

The `.env.multi.example` template contains placeholder values for both sites. Update these sections:

### Site: gg (ggpaintings.com)
```env
GG_SITE_NAME="Gloria Gronowicz Fine Art"
GG_SITE_DESCRIPTION="Gloria Gronowicz is an oil painter who creates works inspired by nature"
GG_CONTACT_EMAIL="gloriagronowicz@gmail.com"
GG_CONTACT_PHONE="860.670.0799"

# Google OAuth - Get from Google Cloud Console
GG_GOOGLE_AUTH_CLIENT_ID=YOUR_GG_CLIENT_ID.apps.googleusercontent.com
GG_GOOGLE_AUTH_CLIENT_SECRET=YOUR_GG_CLIENT_SECRET
GG_GOOGLE_AUTH_REDIRECT_URI=https://ggpaintings.com/api/auth/google/callback

# Admin JWT - Generate unique secret per site
GG_ADMIN_JWT_SECRET_KEY=$(openssl rand -base64 32)
GG_ADMIN_AUTHORIZED_EMAILS=gloriagronowicz@gmail.com

# CORS and API URLs
GG_CORS_ALLOWED_ORIGINS=https://ggpaintings.com
GG_NEXT_PUBLIC_API_URL=/api
GG_SERVER_API_URL=http://api-gg:8080/api
```

### Site: flynn (flynnart.com)
```env
FLYNN_SITE_NAME="Flynn Art Gallery"
FLYNN_SITE_DESCRIPTION="Fine art paintings by Flynn"
FLYNN_CONTACT_EMAIL="flynn@example.com"
FLYNN_CONTACT_PHONE="555.123.4567"

# Google OAuth - Separate client ID from Google Cloud Console
FLYNN_GOOGLE_AUTH_CLIENT_ID=YOUR_FLYNN_CLIENT_ID.apps.googleusercontent.com
FLYNN_GOOGLE_AUTH_CLIENT_SECRET=YOUR_FLYNN_CLIENT_SECRET
FLYNN_GOOGLE_AUTH_REDIRECT_URI=https://flynnart.com/api/auth/google/callback

# Admin JWT - Generate unique secret per site
FLYNN_ADMIN_JWT_SECRET_KEY=$(openssl rand -base64 32)
FLYNN_ADMIN_AUTHORIZED_EMAILS=flynn@example.com

# CORS and API URLs
FLYNN_CORS_ALLOWED_ORIGINS=https://flynnart.com
FLYNN_NEXT_PUBLIC_API_URL=/api
FLYNN_SERVER_API_URL=http://api-flynn:8080/api
```

### NGINX Ports (Production)
```env
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443
NGINX_HEALTH_PORT=9090
```

> **Single-site deployment:** Use `.env.example` → `.env` instead with `CORS_ALLOWED_ORIGINS`, `GOOGLE_CLIENT_ID`, `ADMIN_JWT_SECRET_KEY`, etc.

**Save and exit nano:** `Ctrl+X`, then `Y`, then `Enter`

---

# STEP 5: Create Docker Secret for PostgreSQL Password

```bash
# Navigate to docker-compose directory
cd ~/Painting_Website_Docker_NextJS/docker-compose

# Create secrets directory
mkdir -p secrets

# Generate a strong PostgreSQL password
# Option A: Generate random password
openssl rand -base64 32 | head -c 32 > secrets/postgres_password

# Option B: Use your own password (replace with your password)
# echo -n 'YourStrongPassword123!' > secrets/postgres_password

# Set secure permissions (owner read/write only)
chmod 600 secrets/postgres_password

# Verify the secret file
cat secrets/postgres_password
echo ""

# IMPORTANT: Note down the password for backup configuration later
```

**Important:**
- The `secrets/` directory is excluded from git (`.gitignore`)
- Never commit the password file to version control
- Use `echo -n` to avoid trailing newline if using Option B

---

# STEP 6: Generate Self-Signed SSL Certificates

```bash
# Navigate to nginx directory
cd ~/Painting_Website_Docker_NextJS/docker-compose/nginx

# Create SSL directories for both sites
mkdir -p ssl/gg
mkdir -p ssl/flynn

# Generate self-signed certificate for ggpaintings.com
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout ssl/gg/server.key \
  -out ssl/gg/server.crt \
  -subj "/C=US/ST=New York/L=New York/O=GG Paintings/CN=ggpaintings.com"

# Generate self-signed certificate for flynnart.com
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout ssl/flynn/server.key \
  -out ssl/flynn/server.crt \
  -subj "/C=US/ST=New York/L=New York/O=Flynn Art/CN=flynnart.com"

# Set secure permissions
chmod 600 ssl/gg/server.key
chmod 644 ssl/gg/server.crt
chmod 600 ssl/flynn/server.key
chmod 644 ssl/flynn/server.crt

# Verify certificates
ls -la ssl/gg/
ls -la ssl/flynn/
```

**Note:** Self-signed certificates are used because Cloudflare handles edge SSL. The connection between Cloudflare and your server uses these self-signed certs in "Full" SSL mode. Each site needs its own certificate matching its domain.

---

# STEP 7: Deploy with deploy-multi.sh

```bash
# Navigate to docker-compose directory
cd ~/Painting_Website_Docker_NextJS/docker-compose

# Make deploy script executable
chmod +x deploy-multi.sh

# Run the deployment script
bash deploy-multi.sh
```

The `deploy-multi.sh` script will:
1. Load environment variables from `.env.multi`
2. Run pre-flight checks (secrets, SSL certs, nginx config)
3. Detect Docker Compose v2 (`docker compose`) or v1 (`docker-compose`)
4. Build all images using `docker-compose.multi.yml`
5. Wait for PostgreSQL to be healthy
6. Start all containers (API-GG, API-Flynn, Frontend-GG, Frontend-Flynn, NGINX)
7. Run security checks (non-root users, read-only filesystems)
8. Check for existing backups and restore if available

**Expected output:**
```
========================================
Art Gallery Multi-Site Production Deployment
========================================

Loading environment from .env.multi...
Running pre-flight checks...
  ✓ secrets/postgres_password found
  ✓ SSL certificates found for gg
  ✓ SSL certificates found for flynn
  ✓ nginx/nginx.multi.conf found

Docker Compose version: docker compose (v2)
Building images...
[Build output...]

Starting containers...
[Container start output...]

Waiting for PostgreSQL to be ready...
PostgreSQL is ready!

Running security checks...
✓ API-GG container running as non-root user
✓ API-Flynn container running as non-root user
✓ Frontend-GG container running as non-root user
✓ Frontend-Flynn container running as non-root user
✓ NGINX container running as non-root user
✓ Read-only filesystems enabled

Deployment complete!
```

**Alternative: Manual deployment**
```bash
# If you prefer manual control:
docker compose -f docker-compose.multi.yml up -d --build
```

---

# STEP 8: Verify Deployment

```bash
# Navigate to docker-compose directory
cd ~/Painting_Website_Docker_NextJS/docker-compose

# Check container status
docker compose -f docker-compose.multi.yml ps

# Expected output:
# NAME                             STATUS
# artgallery-postgres              Up X minutes, healthy
# artgallery-api-gg                Up X minutes, healthy
# artgallery-api-flynn             Up X minutes, healthy
# artgallery-frontend-gg           Up X minutes, healthy
# artgallery-frontend-flynn        Up X minutes, healthy
# artgallery-nginx                 Up X minutes, healthy
```

## Verify Non-Root Users

```bash
# API-GG container
docker exec artgallery-api-gg whoami
# Expected: appuser

# API-Flynn container
docker exec artgallery-api-flynn whoami
# Expected: appuser

# Frontend-GG container
docker exec artgallery-frontend-gg whoami
# Expected: nextjs

# Frontend-Flynn container
docker exec artgallery-frontend-flynn whoami
# Expected: nextjs

# NGINX container
docker exec artgallery-nginx whoami
# Expected: nginx
```

## Verify Read-Only Filesystems

```bash
docker inspect artgallery-api-gg --format '{{.HostConfig.ReadonlyRootfs}}'
docker inspect artgallery-api-flynn --format '{{.HostConfig.ReadonlyRootfs}}'
docker inspect artgallery-frontend-gg --format '{{.HostConfig.ReadonlyRootfs}}'
docker inspect artgallery-frontend-flynn --format '{{.HostConfig.ReadonlyRootfs}}'
docker inspect artgallery-nginx --format '{{.HostConfig.ReadonlyRootfs}}'
# All should return: true
```

## Verify Docker Secrets

```bash
# Password should NOT be in environment variables
docker inspect artgallery-postgres --format '{{.Config.Env}}' | grep -i password
# Should return: (empty)

# Secret file should exist in container
docker exec artgallery-postgres cat /run/secrets/postgres_password
# Should return: your postgres password
```

## Test Endpoints

```bash
# Test NGINX health endpoint
curl -I http://localhost:9090/health

# Test API health endpoint for GG (internal)
curl -I http://localhost/api/health/health

# Test frontend (internal)
curl -I http://localhost/
```

## View Logs

```bash
# View all logs
docker compose -f docker-compose.multi.yml logs -f

# View specific service logs
docker compose -f docker-compose.multi.yml logs -f api-gg
docker compose -f docker-compose.multi.yml logs -f api-flynn
docker compose -f docker-compose.multi.yml logs -f frontend-gg
docker compose -f docker-compose.multi.yml logs -f frontend-flynn
docker compose -f docker-compose.multi.yml logs -f nginx
docker compose -f docker-compose.multi.yml logs -f postgres
```

---

# STEP 9: Configure Cloudflare

## 9.1: Add Site to Cloudflare

1. Login to [Cloudflare Dashboard](https://dash.cloudflare.com)
2. Add site `ggpaintings.com`
3. Select the free plan
4. Copy Cloudflare nameservers

## 9.2: Update Domain Nameservers

At your domain registrar, change nameservers to Cloudflare's provided nameservers.

## 9.3: Configure DNS Records

In Cloudflare Dashboard → DNS → Records:

| Type | Name | Content | Proxy Status |
|------|------|---------|--------------|
| A | @ | Your server IP | **Proxied** (orange cloud ON) |
| CNAME | www | ggpaintings.com | **Proxied** (orange cloud ON) |

## 9.4: Configure SSL/TLS

In Cloudflare Dashboard → SSL/TLS → Overview:

1. Set encryption mode to **Full** (not Flexible, not Full Strict)
2. Go to Edge Certificates:
   - Enable **Always Use HTTPS**
   - Enable **Automatic HTTPS Rewrites**
   - Enable **Minimum TLS Version** → TLS 1.2

## 9.5: Configure Security Settings

In Cloudflare Dashboard → Security → Overview:

1. **Security Level:** Medium
2. **Bot Fight Mode:** On

## 9.6: Wait for DNS Propagation

DNS changes typically propagate within 5-10 minutes. Verify with:

```bash
dig ggpaintings.com
```

---

# STEP 10: Setup Automated Backups (Multi-Site)

```bash
# Navigate to scripts directory
cd ~/Painting_Website_Docker_NextJS/docker-compose/scripts

# Install multi-site backup cron job (requires sudo)
sudo bash install-backup-cron-multi.sh
```

The multi-site installation script will:
1. Copy multi-site backup/restore scripts to `/opt/artgallery/scripts/`
2. Create `/opt/artgallery/backups/` directory
3. Install cron job for weekly backup of **both sites** (Sunday 2:00 AM)
4. Run initial test backup for both `artgallery_gg` and `artgallery_flynn` databases

## Configure Multi-Site Backup Settings

```bash
# Edit backup configuration
sudo nano /opt/artgallery/scripts/backup.config
```

**Ensure these values are correct:**
```
POSTGRES_PASSWORD=your-postgres-password-here
CONTAINER_NAME=artgallery-postgres
RETENTION_DAYS=30
```

> **Note:** Multi-site uses `CONTAINER_NAME=artgallery-postgres` (not `artgallery-postgres-prod`).

```bash
# Set secure permissions on config
sudo chmod 600 /opt/artgallery/scripts/backup.config

# Verify cron job is installed
crontab -l
# Should show: 0 2 * * 0 /opt/artgallery/scripts/backup-multi.sh
```

## Multi-Site Manual Backup Test

```bash
# Backup both sites
/opt/artgallery/scripts/backup-multi.sh

# Backup only ggpaintings.com
/opt/artgallery/scripts/backup-multi.sh gg

# Backup only flynnart.com
/opt/artgallery/scripts/backup-multi.sh flynn

# Check backups were created
ls -lh /opt/artgallery/backups/

# Check backup log
cat /opt/artgallery/backups/backup.log
```

## Multi-Site Restore

```bash
# Restore ggpaintings.com (interactive - lists available backups)
/opt/artgallery/scripts/restore-multi.sh gg

# Restore flynnart.com from specific backup
/opt/artgallery/scripts/restore-multi.sh flynn artgallery_artgallery_flynn_20260728_020000.dump
```

---

# STEP 11: Post-Deployment Checklist

## Verify Site Access

```bash
# Test HTTPS access (after DNS propagation)
curl -I https://ggpaintings.com

# Test API access
curl -I https://ggpaintings.com/api/paintings

# Test specific painting category
curl -I https://ggpaintings.com/paintings/landscapes
```

## Verify Database Seeding

The API containers should automatically seed their respective databases on first start. Verify:

```bash
# Check API-GG logs for seeding
docker logs artgallery-api-gg | grep -i seed

# Check API-Flynn logs for seeding
docker logs artgallery-api-flynn | grep -i seed

# Should see: "Database seeding completed"
```

## Verify Admin User Creation

The first admin user is created via Google OAuth login. The email configured in `.env` (`ADMIN_EMAIL`) will be the first admin.

1. Navigate to `https://ggpaintings.com/admin`
2. Click "Login with Google"
3. Login with the email configured in `ADMIN_EMAIL`
4. You should be redirected to the admin dashboard

---

# STEP 12: Ongoing Maintenance

## Update Deployment

```bash
# Navigate to docker-compose directory
cd ~/Painting_Website_Docker_NextJS/docker-compose

# Pull latest changes
git pull

# Backup databases before update
/opt/artgallery/scripts/backup-multi.sh

# Redeploy
bash deploy-multi.sh
```

## View Container Resources

```bash
# Check resource usage
docker stats

# Check disk usage
docker system df

# Clean up unused images and containers
docker system prune -a
```

## Rotate SSL Certificate

```bash
# Self-signed cert expires in 365 days
cd ~/Painting_Website_Docker_NextJS/docker-compose/nginx/ssl

# Regenerate
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout server.key \
  -out server.crt \
  -subj "/C=US/ST=New York/L=New York/O=GG Paintings/CN=ggpaintings.com"

# Restart NGINX container
docker restart artgallery-nginx
```

## Backup and Restore

```bash
# List available backups
ls -lh /opt/artgallery/backups/*.dump

# Restore ggpaintings.com (interactive)
/opt/artgallery/scripts/restore-multi.sh gg

# Restore flynnart.com from specific backup
/opt/artgallery/scripts/restore-multi.sh flynn artgallery_artgallery_flynn_20260728_020000.dump
```

---

# Troubleshooting

## Container Not Starting

```bash
# Check logs
docker logs artgallery-api-gg
docker logs artgallery-api-flynn
docker logs artgallery-frontend-gg
docker logs artgallery-frontend-flynn
docker logs artgallery-nginx
docker logs artgallery-postgres

# Check container health
docker inspect --format='{{.State.Health.Status}}' artgallery-postgres
```

## Database Connection Failed

```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Check PostgreSQL logs
docker logs artgallery-postgres

# Verify password secret
cat secrets/postgres_password
```

## NGINX 502 Bad Gateway

```bash
# Check upstream services for GG site
docker exec artgallery-nginx curl -s http://api-gg:8080/api/health/health
docker exec artgallery-nginx curl -s http://frontend-gg:3000/

# Check upstream services for Flynn site
docker exec artgallery-nginx curl -s http://api-flynn:8080/api/health/health
docker exec artgallery-nginx curl -s http://frontend-flynn:3000/

# Check NGINX logs
docker logs artgallery-nginx
```

## Cloudflare Connection Issues

```bash
# Check if Cloudflare can reach your server (GG site)
curl -H "CF-Connecting-IP: 1.2.3.4" https://ggpaintings.com

# Check if Cloudflare can reach your server (Flynn site)
curl -H "CF-Connecting-IP: 1.2.3.4" https://flynnart.com

# Verify SSL certificate for GG
openssl s_client -connect ggpaintings.com:443 -servername ggpaintings.com

# Verify SSL certificate for Flynn
openssl s_client -connect flynnart.com:443 -servername flynnart.com
```

## Reset Database (Development Only)

```bash
# WARNING: This will delete all data!
docker compose -f docker-compose.multi.yml down -v
docker compose -f docker-compose.multi.yml up -d --build
```

---

# File Reference

| File | Purpose |
|------|---------|
| `deploy.sh` | Single-site deployment script |
| `deploy-multi.sh` | Multi-site deployment script (with database restore and security checks) |
| `docker-compose.prod.yml` | Production compose configuration |
| `docker-compose.yml` | Base compose configuration |
| `docker-compose.multi.yml` | Multi-site production compose configuration |
| `docker-compose.multi.local.yml` | Multi-site local development override |
| `.env.example` | Environment variable template |
| `nginx/nginx.conf` | NGINX configuration for Cloudflare Full mode |
| `nginx/Dockerfile` | Custom NGINX image with non-root user |
| `scripts/backup.sh` | Single-site database backup script |
| `scripts/restore.sh` | Single-site database restore script |
| `scripts/install-backup-cron.sh` | Single-site backup cron installation |
| `scripts/backup-multi.sh` | Multi-site database backup script |
| `scripts/restore-multi.sh` | Multi-site database restore script |
| `scripts/install-backup-cron-multi.sh` | Multi-site backup cron installation |

---

# Security Notes

1. **Never expose internal ports** (3000, 8080, 5432) to the internet
2. **Use Docker secrets** for sensitive data (database passwords)
3. **Keep `.env` file secure** - do not commit to git
4. **Run containers as non-root** - already configured
5. **Use read-only filesystems** - already configured
6. **Enable Cloudflare security features** - WAF, rate limiting, bot protection
7. **Regular backups** - automated via cron job
8. **Keep system updated** - `sudo apt update && sudo apt upgrade`
