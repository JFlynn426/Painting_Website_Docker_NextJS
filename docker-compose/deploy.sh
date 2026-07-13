#!/bin/bash
# ============================================================================
# Deployment Script for Art Gallery Application
# ============================================================================
# This script sets up proper file permissions for non-root NGINX user
# and deploys the application using docker-compose.prod.yml
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
echo "Art Gallery Deployment Script"
echo "=========================================="
echo ""

# Navigate to docker-compose directory
cd "$(dirname "$0")"

# Load environment variables from .env file
if [ -f ".env" ]; then
    set -a  # Export all variables
    source .env
    set +a
    echo "Loaded environment variables from .env"
fi

echo "[1/4] Setting up NGINX file permissions for non-root user (UID 101)..."

# Set ownership of SSL directory and files to UID 101 (nginx user)
if [ -d "nginx/ssl" ]; then
    chown -R 101:101 nginx/ssl
    chmod 755 nginx/ssl
    if [ -f "nginx/ssl/server.crt" ]; then
        chmod 644 nginx/ssl/server.crt
    fi
    if [ -f "nginx/ssl/server.key" ]; then
        chmod 600 nginx/ssl/server.key
    fi
    echo "      SSL directory permissions set"
else
    echo "      WARNING: nginx/ssl directory not found. Please add SSL certificates."
fi

# Set ownership of nginx.conf to UID 101
if [ -f "nginx/nginx.conf" ]; then
    chown 101:101 nginx/nginx.conf
    chmod 644 nginx/nginx.conf
    echo "      nginx.conf permissions set"
else
    echo "      WARNING: nginx/nginx.conf not found."
fi

echo ""
echo "[2/6] Stopping existing containers..."
$COMPOSE -f docker-compose.prod.yml down || true

echo ""
echo "[3/6] Starting PostgreSQL..."
$COMPOSE -f docker-compose.prod.yml up -d postgres

# Wait for PostgreSQL to be healthy
echo "  Waiting for PostgreSQL to be healthy..."
MAX_WAIT=180
ELAPSED=0
CONTAINER_NAME="artgallery-postgres-prod"
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

# Restore latest backup if available
# Uses BACKUP_DIR from .env (sourced via docker-compose) with Linux production fallback
BACKUP_DIR="${BACKUP_DIR:-/opt/artgallery/backups}"
DATABASE_NAME="artgallery"
if [ -d "$BACKUP_DIR" ]; then
    LATEST_BACKUP=$(ls -1t "$BACKUP_DIR"/*.dump 2>/dev/null | head -1)
    if [ -n "$LATEST_BACKUP" ]; then
        BACKUP_NAME=$(basename "$LATEST_BACKUP")
        echo "[4/6] Restoring database from backup: $BACKUP_NAME"
        
        # Copy backup file into container
        docker cp "$LATEST_BACKUP" "$CONTAINER_NAME:/tmp/$BACKUP_NAME" 2>&1
        
        if [ $? -eq 0 ]; then
            # Restore using pg_restore
            docker exec -i "$CONTAINER_NAME" pg_restore \
                -U postgres \
                -c \
                -d "$DATABASE_NAME" \
                "/tmp/$BACKUP_NAME" \
                2>&1
            
            # Clean up temp file in container
            docker exec "$CONTAINER_NAME" rm -f "/tmp/$BACKUP_NAME" 2>/dev/null || true
            
            if [ ${PIPESTATUS[0]:-$?} -eq 0 ]; then
                echo "  Database restored successfully!"
            else
                echo "  WARNING: Database restore failed. Seeder will populate data."
            fi
        else
            echo "  WARNING: Failed to copy backup to container. Seeder will populate data."
        fi
    else
        echo "[4/6] No backup files found in $BACKUP_DIR. Database will be seeded."
    fi
else
    echo "[4/6] Backup directory $BACKUP_DIR not found. Database will be seeded."
fi

echo ""
echo "[5/6] Building and starting remaining containers..."
$COMPOSE -f docker-compose.prod.yml up -d --build

echo ""
echo "[6/6] Checking container status..."
$COMPOSE -f docker-compose.prod.yml ps

echo ""
echo "Running security checks..."
echo "------------------------------------------"

# Security Check 1: Verify all containers run as non-root
echo "✓ Checking container users (should all be non-root)..."
API_USER=$(docker exec artgallery-api-prod whoami 2>/dev/null || echo "FAILED")
FRONTEND_USER=$(docker exec artgallery-frontend-prod whoami 2>/dev/null || echo "FAILED")
POSTGRES_USER=$(docker exec artgallery-postgres-prod whoami 2>/dev/null || echo "FAILED")
NGINX_USER=$(docker exec artgallery-nginx whoami 2>/dev/null || echo "FAILED")

echo "  API Container:    $API_USER (expected: appuser)"
echo "  Frontend:         $FRONTEND_USER (expected: nextjs)"
echo "  PostgreSQL:       $POSTGRES_USER (expected: postgres)"
echo "  NGINX:            $NGINX_USER (expected: nginx)"

# Validate users are non-root
SECURITY_PASSED=true
if [ "$API_USER" = "root" ] || [ "$API_USER" = "FAILED" ]; then
    echo "  ⚠️  WARNING: API container may be running as root!"
    SECURITY_PASSED=false
fi
if [ "$FRONTEND_USER" = "root" ] || [ "$FRONTEND_USER" = "FAILED" ]; then
    echo "  ⚠️  WARNING: Frontend container may be running as root!"
    SECURITY_PASSED=false
fi
if [ "$POSTGRES_USER" = "root" ] || [ "$POSTGRES_USER" = "FAILED" ]; then
    echo "  ⚠️  WARNING: PostgreSQL container may be running as root!"
    SECURITY_PASSED=false
fi
if [ "$NGINX_USER" = "root" ] || [ "$NGINX_USER" = "FAILED" ]; then
    echo "  ⚠️  WARNING: NGINX container may be running as root!"
    SECURITY_PASSED=false
fi

if [ "$SECURITY_PASSED" = true ]; then
    echo "  ✓ All containers running as non-root users"
fi

echo ""
echo "✓ Checking for exposed sensitive files in git..."
SENSITIVE_FILES=$(git ls-files 2>/dev/null | grep -E "(password|secret|key|token)\.txt" || echo "")
if [ -z "$SENSITIVE_FILES" ]; then
    echo "  ✓ No sensitive files tracked in git"
else
    echo "  ⚠️  WARNING: Potential sensitive files found in git:"
    echo "$SENSITIVE_FILES"
fi

echo ""
echo "✓ Verifying .env file is not tracked..."
if git ls-files 2>/dev/null | grep -q "docker-compose/.env"; then
    echo "  ⚠️  WARNING: docker-compose/.env is tracked in git!"
else
    echo "  ✓ .env file properly excluded from git"
fi

echo "------------------------------------------"
echo ""
echo "=========================================="
if [ "$SECURITY_PASSED" = true ]; then
    echo "Deployment Complete - All Security Checks Passed!"
    echo "✓ Security verification successful"
else
    echo "Deployment Complete - Security Checks Have Warnings"
    echo "⚠ Review warnings above"
fi
echo "=========================================="
echo ""
echo "To view logs:"
echo "  $COMPOSE -f docker-compose.prod.yml logs -f"
echo ""
echo "To check health:"
echo "  curl http://localhost:8080/health"
echo ""
echo "To run security checks manually:"
echo "  docker exec artgallery-api-prod whoami"
echo "  docker exec artgallery-frontend-prod whoami"
echo "  docker exec artgallery-postgres-prod whoami"
echo "  docker exec artgallery-nginx whoami"
echo ""