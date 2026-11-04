#!/usr/bin/env bash
# ============================================================================
# Generate Self-Signed SSL Certificates for Production (Cloudflare Full mode)
# ============================================================================
# Creates per-site self-signed certificates for the NGINX origin server.
#
#   Visitor  --HTTPS-->  Cloudflare  --HTTPS (self-signed)-->  NGINX (origin)
#
# Cloudflare terminates visitor SSL at the edge. These certs only secure the
# Cloudflare -> Origin hop, which is why Cloudflare "Full" SSL mode (NOT
# "Full (strict)") is used: Cloudflare does NOT validate the origin cert, so
# a self-signed cert is sufficient. The CN/SAN just need to be plausible.
#
# NGINX expects the files at:
#   nginx/ssl/gg/server.{crt,key}
#   nginx/ssl/flynn/server.{crt,key}
#
# These files are gitignored (*.crt / *.key) and therefore do NOT travel via
# git. Run this script ONCE on the target server before the first deploy.
#
# Usage:
#   ./scripts/generate-prod-ssl.sh
#
# Requirements: openssl (preinstalled on most Linux distros)
# ============================================================================

set -euo pipefail

# Resolve the docker-compose directory (parent of scripts/)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
SSL_ROOT="${COMPOSE_DIR}/nginx/ssl"

# Days the self-signed cert is valid. Cloudflare Full mode does not validate
# the origin cert, so this is purely a maintenance convenience (10 years).
VALIDITY_DAYS=3650

generate_cert() {
    local cert_dir="$1"
    local common_name="$2"
    local san="$3"
    local organization="$4"

    echo "==> Generating certificate for ${common_name}"
    mkdir -p "${cert_dir}"

    openssl req -x509 -nodes -newkey rsa:2048 \
        -days "${VALIDITY_DAYS}" \
        -keyout "${cert_dir}/server.key" \
        -out "${cert_dir}/server.crt" \
        -subj "/C=US/ST=Florida/L=Local/O=${organization}/CN=${common_name}" \
        -addext "subjectAltName=${san}"

    chmod 600 "${cert_dir}/server.key"
    chmod 644 "${cert_dir}/server.crt"

    echo "    cert: ${cert_dir}/server.crt"
    echo "    key:  ${cert_dir}/server.key"
}

echo "=========================================="
echo "Production SSL Certificate Generator"
echo "=========================================="
echo ""

# Site: gg (ggpaintings.com)
generate_cert \
    "${SSL_ROOT}/gg" \
    "ggpaintings.com" \
    "DNS:ggpaintings.com,DNS:www.ggpaintings.com" \
    "GG Paintings"

# Site: flynn (flynnart.com)
generate_cert \
    "${SSL_ROOT}/flynn" \
    "flynnart.com" \
    "DNS:flynnart.com,DNS:www.flynnart.com" \
    "Flynn Art"

echo ""
echo "All production SSL certificates generated successfully!"
echo ""
echo "Next steps on the target server:"
echo "  1. cp .env.multi.example .env.multi   # then fill in real values"
echo "  2. echo -n '<strong-password>' > secrets/postgres_password"
echo "     # and set the SAME value as POSTGRES_PASSWORD in .env.multi"
echo "  3. ./deploy-multi.sh"
