#!/bin/bash
# ============================================================================
# Deployment Script for Art Gallery Application
# ============================================================================
# This script sets up proper file permissions for non-root NGINX user
# and deploys the application using docker-compose.prod.yml
# ============================================================================

set -e

echo "=========================================="
echo "Art Gallery Deployment Script"
echo "=========================================="
echo ""

# Navigate to docker-compose directory
cd "$(dirname "$0")"

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
echo "[2/4] Stopping existing containers..."
docker-compose -f docker-compose.prod.yml down || true

echo ""
echo "[3/4] Building and starting containers..."
docker-compose -f docker-compose.prod.yml up -d --build

echo ""
echo "[4/5] Checking container status..."
docker-compose -f docker-compose.prod.yml ps

echo ""
echo "[5/5] Running security checks..."
echo "------------------------------------------"

# Security Check 1: Verify all containers run as non-root
echo "✓ Checking container users (should all be non-root)..."
API_USER=$(docker exec artgallery-api whoami 2>/dev/null || echo "FAILED")
FRONTEND_USER=$(docker exec artgallery-frontend whoami 2>/dev/null || echo "FAILED")
SQL_USER=$(docker exec artgallery-sql-prod whoami 2>/dev/null || echo "FAILED")
NGINX_USER=$(docker exec artgallery-nginx whoami 2>/dev/null || echo "FAILED")

echo "  API Container:    $API_USER (expected: appuser)"
echo "  Frontend:         $FRONTEND_USER (expected: nextjs)"
echo "  SQL Server:       $SQL_USER (expected: mssql)"
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
if [ "$SQL_USER" = "root" ] || [ "$SQL_USER" = "FAILED" ]; then
    echo "  ⚠️  WARNING: SQL Server container may be running as root!"
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
echo "  docker-compose -f docker-compose.prod.yml logs -f"
echo ""
echo "To check health:"
echo "  curl http://localhost:8080/health"
echo ""
echo "To run security checks manually:"
echo "  docker exec artgallery-api whoami"
echo "  docker exec artgallery-frontend whoami"
echo "  docker exec artgallery-sql-prod whoami"
echo "  docker exec artgallery-nginx whoami"
echo ""