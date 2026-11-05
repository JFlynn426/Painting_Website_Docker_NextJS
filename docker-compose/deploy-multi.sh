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
echo "[2/8] Setting up image-data ownership for the API user (UID 1654)..."

# The API containers run as the .NET base image's non-root user `app`, whose UID comes from
# $APP_UID in mcr.microsoft.com/dotnet/aspnet:10.0 and is 1654. Neither multi-site compose
# file overrides `user:` for api-gg / api-flynn, so 1654 is what actually writes to
# ./image-data/<site>. Two things make the hand-over necessary: Docker creates missing
# bind-mount directories as root, and the API image's own `chown $APP_UID /app/images` is
# shadowed by the mount. Without this, painting uploads fail with "permission denied".
# Re-check the UID after a base-image bump with:
#   docker run --rm --entrypoint sh mcr.microsoft.com/dotnet/aspnet:10.0 -c "id -u app"
API_UID=1654
mkdir -p image-data/gg/original image-data/gg/high-res image-data/gg/thumbnail
mkdir -p image-data/flynn/original image-data/flynn/high-res image-data/flynn/thumbnail
for site_dir in image-data/gg image-data/flynn; do
    chown -R "$API_UID:$API_UID" "$site_dir"
    chmod 755 "$site_dir"
    echo "  $site_dir owned by UID $API_UID"
done

echo ""
echo "[3/8] Stopping existing containers..."

# The single-site stack (docker-compose.prod.yml) and the multi-site stack cannot run side by
# side: both bind host ports 80/443/9090, both claim the container name 'artgallery-nginx', and
# both mount a postgres_data volume. On a server that already runs ggpaintings.com the prod
# stack is normally still up, so it has to be torn down before the new stack can start.
$COMPOSE -f docker-compose.multi.yml down || true
if [ -f "docker-compose.prod.yml" ]; then
    echo "  Also stopping the single-site stack (docker-compose.prod.yml)..."
    $COMPOSE -f docker-compose.prod.yml down || true
fi

# 'compose down' only reaches containers belonging to its own project. If the single-site stack
# was originally deployed under a different project name (Visual Studio Docker tooling, an
# explicit -p flag, or a different checkout path) its containers survive the down above and keep
# ports 80/443/9090 bound, so the new NGINX would die with 'port is already allocated'.
BLOCKING=$(docker ps -a --format '{{.Names}}' 2>/dev/null | grep -E '^artgallery-(nginx|postgres-prod|api-prod|frontend-prod)$' || true)
if [ -n "$BLOCKING" ]; then
    echo "ERROR: These containers still exist and would block the multi-site stack:"
    echo "$BLOCKING" | sed 's/^/  - /'
    echo "Remove them first with:"
    echo "  docker compose -f docker-compose.prod.yml down"
    echo "or, if they belong to an older project name:"
    echo "  docker rm -f $(echo "$BLOCKING" | tr '\n' ' ')"
    exit 1
fi

echo ""
echo "  Checking PostgreSQL data volumes..."
# Both compose files declare a named 'postgres_data' volume, and Docker namespaces named volumes
# after the compose project name. More than one match means the multi-site stack may come up on
# a volume that does not hold the live gg data, and the seeder would then quietly rebuild gg
# from seed data. Pin the project name with COMPOSE_PROJECT_NAME if that happens.
PG_VOLUMES=$(docker volume ls --format '{{.Name}}' 2>/dev/null | grep -E 'postgres_data$' || true)
if [ -n "$PG_VOLUMES" ]; then
    echo "$PG_VOLUMES" | sed 's/^/    found: /'
    if [ "$(printf '%s\n' "$PG_VOLUMES" | wc -l)" -gt 1 ]; then
        echo "  WARNING: several postgres_data volumes exist. Check that the stack below lands on"
        echo "  the one holding your existing data before trusting this deploy."
    fi
else
    echo "    none - a fresh empty volume will be created (databases will be seeded, not restored)"
fi

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

# Ownership of the image bind mounts is not proof the API can write them, so check from inside
# the container, which is where uploads actually run as UID 1654.
check_writable() {
    local name="$1"
    local dir="$2"
    local result
    result=$(docker exec "$name" sh -c "test -w '$dir' && echo writable || echo blocked" 2>/dev/null || echo "unreachable")
    if [ "$result" = "writable" ]; then
        echo "  $name: can write $dir"
    else
        echo "  $name: WARNING - cannot write $dir ($result) - painting uploads will fail"
        echo "           fix with: chown -R 1654:1654 image-data/<site>"
        SECURITY_PASSED=false
    fi
}

check_uid artgallery-api-gg
check_uid artgallery-api-flynn
check_uid artgallery-frontend-gg
check_uid artgallery-frontend-flynn
check_proc_owner artgallery-postgres
check_proc_owner artgallery-nginx

echo ""
echo "Checking that the API can write its image storage..."
check_writable artgallery-api-gg /app/images/original
check_writable artgallery-api-flynn /app/images/original

if [ "$SECURITY_PASSED" = true ]; then
    echo "  All containers non-root, image storage writable"
fi

echo ""
echo "Checking for exposed sensitive files in git..."
# 'git ls-files' prints paths relative to the current directory, so the pathspecs have to be
# relative to docker-compose/ as well - 'docker-compose/.env.multi' can never match here. The
# real secret lives at secrets/postgres_password, not in a *.txt file.
SENSITIVE_FILES=$(git ls-files -- '.env.multi' '.env.multi.arm64' 'secrets/postgres_password' 2>/dev/null || true)
if [ -z "$SENSITIVE_FILES" ]; then
    echo "  No sensitive files tracked in git"
else
    echo "  WARNING: Sensitive files are tracked in git:"
    echo "$SENSITIVE_FILES"
fi

echo ""
echo "Verifying .env.multi file is not tracked..."
if [ -n "$(git ls-files -- '.env.multi' '.env.multi.arm64' 2>/dev/null || true)" ]; then
    echo "  WARNING: a .env.multi file is tracked in git!"
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
echo "  - ggpaintings.com      -> artgallery-frontend-gg   + artgallery-api-gg"
echo "  - earthelementsart.com -> artgallery-frontend-flynn + artgallery-api-flynn"
echo ""
echo "To view logs:"
echo "  $COMPOSE -f docker-compose.multi.yml logs -f"
echo ""
echo "To check health:"
echo "  curl http://localhost:9090/health"
echo ""
echo "To run security checks manually:"
echo "  docker exec artgallery-api-gg id -u          # 1654 (app, from \$APP_UID)"
echo "  docker exec artgallery-api-flynn id -u       # 1654 (app)"
echo "  docker exec artgallery-frontend-gg id -u     # 1001 (nextjs)"
echo "  docker exec artgallery-frontend-flynn id -u  # 1001 (nextjs)"
echo "  docker top artgallery-postgres | head -2     # USER column should be postgres"
echo "  docker top artgallery-nginx | head -2        # USER column should be nginx (101)"
echo "  docker exec artgallery-api-gg sh -c 'test -w /app/images/original && echo writable'"
echo ""
