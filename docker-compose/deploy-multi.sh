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

echo "[1/6] Setting up NGINX file permissions for non-root user (UID 101)..."

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
echo "[2/6] Stopping existing containers..."
$COMPOSE -f docker-compose.multi.yml down || true

echo ""
echo "[3/6] Starting PostgreSQL..."
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
echo "[4/6] Restoring databases from backups..."

for site in gg flynn; do
    DATABASE_NAME="${SITE_DATABASES[$site]}"
    echo "  Checking backups for site '$site' (database: $DATABASE_NAME)..."

    if [ -d "$BACKUP_DIR" ]; then
        # Find latest backup for this specific site
        LATEST_BACKUP=$(ls -1t "$BACKUP_DIR"/artgallery_${DATABASE_NAME}_*.dump 2>/dev/null | head -1)
        if [ -n "$LATEST_BACKUP" ]; then
            BACKUP_NAME=$(basename "$LATEST_BACKUP")
            echo "    Restoring from backup: $BACKUP_NAME"

            # Copy backup file into container
            docker cp "$LATEST_BACKUP" "$CONTAINER_NAME:/tmp/$BACKUP_NAME" 2>&1

            if [ $? -eq 0 ]; then
                # Drop and recreate database, then restore
                docker exec "$CONTAINER_NAME" psql -U postgres -c "DROP DATABASE IF EXISTS $DATABASE_NAME;" 2>/dev/null || true
                docker exec "$CONTAINER_NAME" psql -U postgres -c "CREATE DATABASE $DATABASE_NAME;" 2>/dev/null || true

                docker exec -i "$CONTAINER_NAME" pg_restore \
                    -U postgres \
                    -d "$DATABASE_NAME" \
                    "/tmp/$BACKUP_NAME" \
                    2>&1

                # Clean up temp file in container
                docker exec "$CONTAINER_NAME" rm -f "/tmp/$BACKUP_NAME" 2>/dev/null || true

                if [ ${PIPESTATUS[0]:-$?} -eq 0 ]; then
                    echo "    Database restored successfully for site '$site'!"
                else
                    echo "    WARNING: Database restore failed for site '$site'. Seeder will populate data."
                fi
            else
                echo "    WARNING: Failed to copy backup to container for site '$site'. Seeder will populate data."
            fi
        else
            echo "    No backup files found for site '$site' in $BACKUP_DIR. Database will be seeded."
        fi
    else
        echo "    Backup directory $BACKUP_DIR not found for site '$site'. Database will be seeded."
    fi
done

echo ""
echo "[5/6] Building and starting remaining containers..."
$COMPOSE -f docker-compose.multi.yml up -d --build

echo ""
echo "[6/6] Checking container status..."
$COMPOSE -f docker-compose.multi.yml ps

echo ""
echo "Running security checks..."
echo "------------------------------------------"

# Security Check: Verify all containers run as non-root
echo "Checking container users (should all be non-root)..."
API_GG_USER=$(docker exec artgallery-api-gg whoami 2>/dev/null || echo "FAILED")
API_FLYNN_USER=$(docker exec artgallery-api-flynn whoami 2>/dev/null || echo "FAILED")
FRONTEND_GG_USER=$(docker exec artgallery-frontend-gg whoami 2>/dev/null || echo "FAILED")
FRONTEND_FLYNN_USER=$(docker exec artgallery-frontend-flynn whoami 2>/dev/null || echo "FAILED")
POSTGRES_USER=$(docker exec artgallery-postgres whoami 2>/dev/null || echo "FAILED")
NGINX_USER=$(docker exec artgallery-nginx whoami 2>/dev/null || echo "FAILED")

echo "  API (GG):         $API_GG_USER (expected: appuser)"
echo "  API (Flynn):      $API_FLYNN_USER (expected: appuser)"
echo "  Frontend (GG):    $FRONTEND_GG_USER (expected: nextjs)"
echo "  Frontend (Flynn): $FRONTEND_FLYNN_USER (expected: nextjs)"
echo "  PostgreSQL:       $POSTGRES_USER (expected: postgres)"
echo "  NGINX:            $NGINX_USER (expected: nginx)"

# Validate users are non-root
SECURITY_PASSED=true
for user_check in "$API_GG_USER" "$API_FLYNN_USER" "$FRONTEND_GG_USER" "$FRONTEND_FLYNN_USER" "$POSTGRES_USER" "$NGINX_USER"; do
    if [ "$user_check" = "root" ] || [ "$user_check" = "FAILED" ]; then
        echo "  WARNING: A container may be running as root or failed to check!"
        SECURITY_PASSED=false
    fi
done

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
echo "  curl http://localhost:8080/health"
echo ""
echo "To run security checks manually:"
echo "  docker exec artgallery-api-gg whoami"
echo "  docker exec artgallery-api-flynn whoami"
echo "  docker exec artgallery-frontend-gg whoami"
echo "  docker exec artgallery-frontend-flynn whoami"
echo "  docker exec artgallery-postgres whoami"
echo "  docker exec artgallery-nginx whoami"
echo ""
