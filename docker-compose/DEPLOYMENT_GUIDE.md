ENCRYPT CONN STRINGS, RUN CONTAINERS AS ROOT
MAKE SURE DB CONT PORTS ARE NOT AVAILABLE BEFORE DEPLOY!!!

# Deployment Guide: Development vs Production

This guide explains how to switch between development and production modes while maintaining your local Docker development workflow.

## Overview

The solution uses two docker-compose files:
- **`docker-compose.yml`** - Development mode (hot reload, debug mode)
- **`docker-compose.prod.yml`** - Production mode (optimized builds, NGINX reverse proxy)

## Prerequisites (Production)

- Docker and Docker Compose installed on Linux server
- Domain name configured and pointing to server IP
- SSL certificates (self-signed or from Let's Encrypt)
- At least 4GB RAM recommended

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

```bash
cp .env.example .env
nano .env
```

Update these values in `.env`:
```env
# SQL Server password (change to secure password)
SQLSERVER_SA_PASSWORD=YourSecurePassword123!

# CORS - your production domain
CORS_ALLOWED_ORIGINS=https://yourdomain.com

# API URLs for production (NGINX routing)
NEXT_PUBLIC_API_URL=/api
SERVER_API_URL=http://api:8080/api

# NGINX ports
NGINX_HTTP_PORT=80
NGINX_HTTPS_PORT=443
```

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

# Option B: Self-signed (for testing)
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/server.key \
  -out nginx/ssl/server.crt \
  -subj "/C=US/ST=State/L=City/O=Organization/CN=yourdomain.com"

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
curl https://yourdomain.com/api/health/health
curl http://localhost:8080/health
```

### Production Configuration

**Environment Variables** (in `.env`):
```env
NEXT_PUBLIC_API_URL=/api
SERVER_API_URL=http://api:8080/api
CORS_ALLOWED_ORIGINS=https://yourdomain.com
```

**How it works:**
- Browser accesses everything at `https://yourdomain.com`
- NGINX routes `/api/*` to API container
- NGINX routes `/*` to Frontend container
- Frontend container accesses API at `http://api:8080/api` (Docker network)
- No CORS issues (same origin)

### Access Points
- Frontend: https://yourdomain.com
- API: https://yourdomain.com/api
- Health: http://localhost:8080/health
- Swagger: Disabled in production

### Stop Production Environment

```bash
docker-compose -f docker-compose.prod.yml down
```

## Switching Between Modes

### From Development to Production

1. **Stop development:**
   ```bash
   docker-compose down
   ```

2. **Update .env for production:**
   ```env
   NEXT_PUBLIC_API_URL=/api
   SERVER_API_URL=http://api:8080/api
   CORS_ALLOWED_ORIGINS=https://yourdomain.com
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
CORS_ALLOWED_ORIGINS=https://yourdomain.com
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
- [ ] SSL certificates installed and configured
- [ ] `CORS_ALLOWED_ORIGINS` set to production domain only
- [ ] `.env` file not committed to git
- [ ] Firewall configured (ports 80, 443 only)
- [ ] Database backups scheduled
- [ ] SSL certificate auto-renewal configured (if using Let's Encrypt)

## Support

For issues or questions:
- Check logs: `docker-compose -f docker-compose.prod.yml logs -f`
- Review NGINX config: `nginx/nginx.conf`
- Verify environment variables: `cat .env`