#!/bin/sh
# ============================================================================
# NGINX Entrypoint Script for Read-Only Mode with Non-Root User
# ============================================================================
# This script fixes tmpfs permissions before starting nginx.
# It runs as root initially, then switches to non-root user.
# ============================================================================

set -e

echo "Setting up tmpfs permissions for nginx user (UID 101)..."

# Fix permissions on tmpfs mounts created by Docker
# These are created as root:root when using read_only: true with tmpfs
chown -R 101:101 /var/cache/nginx 2>/dev/null || true
chown -R 101:101 /var/log/nginx 2>/dev/null || true
chown -R 101:101 /run 2>/dev/null || true
chown -R 101:101 /etc/nginx/conf.d 2>/dev/null || true
chown -R 101:101 /tmp 2>/dev/null || true

# Ensure required directories exist with correct ownership
mkdir -p /run/nginx 2>/dev/null || true
chown -R 101:101 /run/nginx 2>/dev/null || true

echo "Permissions set successfully."

# Switch to non-root user and execute the command
exec su-exec 101 "$@"