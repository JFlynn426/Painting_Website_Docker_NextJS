#!/bin/bash
# ============================================================================
# Multi-Site Deployment Script for Art Gallery Application
# ============================================================================
# This script sets up proper file permissions for non-root NGINX user
# and deploys the multi-site application using docker-compose.multi.yml
# ============================================================================

set -e

# Detect Docker Compose command (v2 'docker compose' or v1 'docker-compose')
if docker compose version >/dev/null 2>&1; then
    COMPOSE="docker compose"
elif docker-compose version >/dev/null 2>&1; then
    COMPOSE="docker-compose"
else
    echo "ERROR: Neither 'docker compose' nor 'docker-compose' found"
    exit 1
fi

echo "=========================================="
echo "Multi-Site Art Gallery Deployment"
echo "=========================================="
echo ""

# Navigate to docker-compose directory
cd "$(dirname "$0")"

# Load multi-site environment variables
# Prefer .env.multi over .env.multi.example
if [ -f ".env.multi" ]; then
    set -a  # Export all variables
    source .env.multi
    set +a
    echo "Loaded multi-site environment from .env.multi"
elif [ -f ".env.multi.example" ]; then
    echo "ERROR: .env.multi not found. Copy .env.multi.example to .env.multi and configure."
    exit 1
else
    echo "ERROR: Neither .env.multi nor .env.multi.example found."
    exit 1
fi

# Pre-flight checks
echo ""
echo "Running pre-flight checks..."
echo "------------------------------------------"

PREFLIGHT_PASSED=true

# Check PostgreSQL password secret file
if [ ! -f "secrets/postgres_password" ]; then
    echo "  ERROR: secrets/postgres_password not found!"
    echo "  Create it with: echo -n 'your_password' > secrets/postgres_password"
    PREFLIGHT_PASSED=false
else
    echo "  PostgreSQL password secret file found"
fi

# Check SSL certificate directories and files
for site_dir in nginx/ssl/gg nginx/ssl/flynn; do
    if [ -d "$site_dir" ]; then
        if [ ! -f "$site_dir/server.crt" ] || [ ! -f "$site_dir/server.key" ]; then
            echo "  ERROR: SSL certificate files missing in $site_dir/"
            echo "  Generate with: openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout $site_dir/server.key -out $site_dir/server.crt -subj '/CN=localhost'"
            PREFLIGHT_PASSED=false
        else
            echo "  SSL certificates found in $site_dir/"
        fi
    else
        echo "  ERROR: SSL directory $site_dir/ not found!"
        echo "  Create it with: mkdir -p $site_dir"
        PREFLIGHT_PASSED=false
    fi
done

# Check NGINX config file
if [ ! -f "nginx/nginx.conf" ]; then
    echo "  ERROR: nginx/nginx.conf not found!"
    PREFLIGHT_PASSED=false
else
    echo "  NGINX config file found"
fi

if [ "$PREFLIGHT_PASSED" = false ]; then
    echo ""
    echo "ERROR: Pre-flight checks failed. Fix the issues above before deploying."
    exit 1
fi

echo "------------------------------------------"
echo "All pre-flight checks passed!"
echo ""

echo "[1/8] Setting up NGINX file permissions for non-root user (UID 101)..."

# Set ownership of SSL directories and files to UID 101 (nginx user)
for site_dir in nginx/ssl/gg nginx/ssl/flynn; do
    if [ -d "$site_dir" ]; then
        chown -R 101:101 "$site_dir"
        chmod 755 "$site_dir"
        if [ -f "$site_dir/server.crt" ]; then
            chmod 644 "$site_dir/server.crt"
        fi
        if [ -f "$site_dir/server.key" ]; then
            chmod 600 "$site_dir/server.key"
        fi
        echo "  $site_dir permissions set"
    else
        echo "  WARNING: $site_dir not found. Please add SSL certificates."
    fi
done

# Set ownership of nginx.conf to UID 101
if [ -f "nginx/nginx.conf" ]; then
    chown 101:101 nginx/nginx.conf
    chmod 644 nginx/nginx.conf
    echo "  nginx.conf permissions set"
else
    echo "  WARNING: nginx/nginx.conf not found."
fi

echo ""
echo "[2/8] Setting up image-data ownership for API user (UID 1)..."

# The API containers run as UID 1 and mount ./image-data/<site> read-write.
# Docker creates missing bind-mount directories as root, which the API cannot write to,
# so painting uploads fail with "permission denied". Create and hand them to UID 1.
mkdir -p image-data/gg/original image-data/gg/high-res image-data/gg/thumbnail
mkdir -p image-data/flynn/original image-data/flynn/high-res image-data/flynn/thumbnail
for site_dir in image-data/gg image-data/flynn; do
    chown -R 1:1 "$site_dir"
    chmod 755 "$site_dir"
    echo "  $site_dir owned by UID 1"
done

echo ""
echo "[3/8] Stopping existing containers..."
$COMPOSE -f docker-compose.multi.yml down || true

echo ""
echo "[4/8] Starting PostgreSQL..."
$COMPOSE -f docker-compose.multi.yml up -d postgres

# Wait for PostgreSQL to be healthy
echo "  Waiting for PostgreSQL to be healthy..."
MAX_WAIT=180
ELAPSED=0
CONTAINER_NAME="artgallery-postgres"
while [ $ELAPSED -lt $MAX_WAIT ]; do
    HEALTH=$(docker inspect -f '{{.State.Health.Status}}' "$CONTAINER_NAME" 2>/dev/null || echo "starting")
    if [ "$HEALTH" = "healthy" ]; then
        echo "  PostgreSQL is healthy!"
        break
    fi
    sleep 5
    ELAPSED=$((ELAPSED + 5))
    echo -n "."
done
echo ""

if [ "$HEALTH" != "healthy" ]; then
    echo "ERROR: PostgreSQL did not become healthy in time"
    exit 1
fi

# Restore latest backups for each site if available
# Uses BACKUP_DIR from .env.multi (sourced above) with Linux production fallback
BACKUP_DIR="${BACKUP_DIR:-/opt/artgallery/backups}"

# Site database mapping
declare -A SITE_DATABASES
SITE_DATABASES[gg]="artgallery_gg"
SITE_DATABASES[flynn]="artgallery_flynn"

echo ""
echo "[5/8] Ensuring site databases exist..."

# init-databases.sql only runs when the postgres_data volume is empty. On an existing
# deployment (for example when adding the flynn site to a server that already runs gg)
# the new site's database does not exist yet, and the API container fails on startup
# because EF Core cannot migrate a missing database. Because nginx waits for both API
# containers to be healthy, that single failure would take the whole site down.
for site in gg flynn; do
    DATABASE_NAME="${SITE_DATABASES[$site]}"
    DB_EXISTS=$(docker exec "$CONTAINER_NAME" psql -U postgres -tAc \
        "SELECT 1 FROM pg_database WHERE datname = '$DATABASE_NAME'" 2>/dev/null || echo "")
    if [ "$DB_EXISTS" = "1" ]; then
        echo "  Database $DATABASE_NAME already exists"
    else
        echo "  Creating database $DATABASE_NAME ..."
        docker exec "$CONTAINER_NAME" createdb -U postgres "$DATABASE_NAME"
    fi
done

echo ""
echo "[6/8] Restoring databases from backups..."

for site in gg flynn; do
    DATABASE_NAME="${SITE_DATABASES[$site]}"
    echo "  Checking backups for site '$site' (database: $DATABASE_NAME)..."

    if [ ! -d "$BACKUP_DIR" ]; then
        echo "    Backup directory $BACKUP_DIR not found for site '$site'. Database will be seeded."
        continue
    fi

    # This glob only matches dumps written by scripts/backup-multi.sh, which prefixes files
    # with 'artgallery_<database>_'. Single-site dumps from scripts/backup.sh are named
    # 'artgallery_db_<timestamp>.dump' and are deliberately not restored here.
    LATEST_BACKUP=$(ls -1t "$BACKUP_DIR"/artgallery_${DATABASE_NAME}_*.dump 2>/dev/null | head -1)
    if [ -z "$LATEST_BACKUP" ]; then
        echo "    No backup files found for site '$site' in $BACKUP_DIR. Database will be seeded."
        continue
    fi

    BACKUP_NAME=$(basename "$LATEST_BACKUP")
    echo "    Restoring from backup: $BACKUP_NAME"

    if ! docker cp "$LATEST_BACKUP" "$CONTAINER_NAME:/tmp/$BACKUP_NAME"; then
        echo "    WARNING: Failed to copy backup to container for site '$site'. Seeder will populate data."
        continue
    fi

    # Drop and recreate database, then restore. `set -e` is active, so every step has to opt
    # out explicitly: run bare, a failing pg_restore aborts the script here and leaves the
    # stack with only PostgreSQL up, instead of falling through to the seeder as intended.
    docker exec "$CONTAINER_NAME" psql -U postgres -c "DROP DATABASE IF EXISTS $DATABASE_NAME;" >/dev/null 2>&1 || true
    docker exec "$CONTAINER_NAME" psql -U postgres -c "CREATE DATABASE $DATABASE_NAME;" >/dev/null 2>&1 || true

    if docker exec -i "$CONTAINER_NAME" pg_restore -U postgres -d "$DATABASE_NAME" "/tmp/$BACKUP_NAME"; then
        echo "    Database restored successfully for site '$site'!"
    else
        echo "    WARNING: Database restore failed for site '$site'. Seeder will populate data."
    fi

    # Clean up temp file in container
    docker exec "$CONTAINER_NAME" rm -f "/tmp/$BACKUP_NAME" 2>/dev/null || true
done

echo ""
echo "[7/8] Building and starting remaining containers..."
$COMPOSE -f docker-compose.multi.yml up -d --build

echo ""
echo "[8/8] Checking container status..."
$COMPOSE -f docker-compose.multi.yml ps

echo ""
echo "Running security checks..."
echo "------------------------------------------"

# Security Check: Verify all containers run as non-root
#
# `docker exec ... whoami` only reflects a container's real user when its image declares a
# USER. The API and frontend images do, so their UID is read directly. PostgreSQL and NGINX
# start as root and drop privileges at runtime, so `docker exec` still reports root for
# them; their actual process owner is read with `docker top` instead.
echo "Checking container users (should all be non-root)..."
SECURITY_PASSED=true

check_uid() {
    local name="$1"
    local uid
    uid=$(docker exec "$name" id -u 2>/dev/null || echo "")
    if [ -z "$uid" ]; then
        echo "  $name: WARNING - could not check (is the container running?)"
        SECURITY_PASSED=false
    elif [ "$uid" = "0" ]; then
        echo "  $name: WARNING - running as root (UID 0)"
        SECURITY_PASSED=false
    else
        echo "  $name: non-root (UID $uid)"
    fi
}

check_proc_owner() {
    local name="$1"
    local owner
    owner=$(docker top "$name" 2>/dev/null | awk 'NR==1 && $1=="UID" {next} {print $1; exit}')
    if [ -z "$owner" ]; then
        echo "  $name: WARNING - could not check (is the container running?)"
        SECURITY_PASSED=false
    elif [ "$owner" = "root" ]; then
        echo "  $name: WARNING - process owner is root"
        SECURITY_PASSED=false
    else
        echo "  $name: non-root ($owner)"
    fi
}

check_uid artgallery-api-gg
check_uid artgallery-api-flynn
check_uid artgallery-frontend-gg
check_uid artgallery-frontend-flynn
check_proc_owner artgallery-postgres
check_proc_owner artgallery-nginx

if [ "$SECURITY_PASSED" = true ]; then
    echo "  All containers running as non-root users"
fi

echo ""
echo "Checking for exposed sensitive files in git..."
SENSITIVE_FILES=$(git ls-files 2>/dev/null | grep -E "(password|secret|key|token)\.txt" || echo "")
if [ -z "$SENSITIVE_FILES" ]; then
    echo "  No sensitive files tracked in git"
else
    echo "  WARNING: Potential sensitive files found in git:"
    echo "$SENSITIVE_FILES"
fi

echo ""
echo "Verifying .env.multi file is not tracked..."
if git ls-files 2>/dev/null | grep -q "docker-compose/.env.multi"; then
    echo "  WARNING: docker-compose/.env.multi is tracked in git!"
else
    echo "  .env.multi file properly excluded from git"
fi

echo "------------------------------------------"
echo ""
echo "=========================================="
if [ "$SECURITY_PASSED" = true ]; then
    echo "Multi-Site Deployment Complete - All Security Checks Passed!"
    echo "Security verification successful"
else
    echo "Multi-Site Deployment Complete - Security Checks Have Warnings"
    echo "Review warnings above"
fi
echo "=========================================="
echo ""
echo "Deployed Sites:"
echo "  - ggpaintings.com -> artgallery-frontend-gg + artgallery-api-gg"
echo "  - flynnart.com    -> artgallery-frontend-flynn + artgallery-api-flynn"
echo ""
echo "To view logs:"
echo "  $COMPOSE -f docker-compose.multi.yml logs -f"
echo ""
echo "To check health:"
echo "  curl http://localhost:9090/health"
echo ""
echo "To run security checks manually:"
echo "  docker exec artgallery-api-gg id -u          # non-zero"
echo "  docker exec artgallery-api-flynn id -u       # non-zero"
echo "  docker exec artgallery-frontend-gg id -u     # 1001 (nextjs)"
echo "  docker exec artgallery-frontend-flynn id -u  # 1001 (nextjs)"
echo "  docker top artgallery-postgres | head -2     # USER column should be postgres"
echo "  docker top artgallery-nginx | head -2        # USER column should be nginx (101)"
echo ""
