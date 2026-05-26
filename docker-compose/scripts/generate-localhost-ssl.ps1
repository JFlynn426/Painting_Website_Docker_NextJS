# ============================================================================
# Generate Self-Signed SSL Certificate for Localhost
# ============================================================================
# This script creates a self-signed SSL certificate for localhost testing.
# Run this script once before starting the local Docker stack.
#
# Usage:
#   .\scripts\generate-localhost-ssl.ps1
# ============================================================================

$sslDir = Join-Path $PSScriptRoot "..\nginx\ssl\localhost"
New-Item -ItemType Directory -Force -Path $sslDir | Out-Null

Write-Host "Generating self-signed SSL certificate for localhost..." -ForegroundColor Cyan

# Generate self-signed certificate using Alpine Docker container with OpenSSL
# This works on Windows without requiring OpenSSL or mkcert to be installed locally
$sslDirUnix = $sslDir -replace '\\', '/'

docker run --rm `
    -v "${sslDirUnix}:/certs" `
    alpine:3.18 `
    sh -c "apk add --no-cache openssl && openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /certs/server.key -out /certs/server.crt -subj '/C=US/ST=Florida/L=Local/O=Local Dev/CN=localhost' -addext 'subjectAltName=DNS:localhost,IP:127.0.0.1' && chmod 644 /certs/server.key /certs/server.crt"

Write-Host ""
Write-Host "SSL certificate generated successfully!" -ForegroundColor Green
Write-Host "Certificate: $sslDir\server.crt" -ForegroundColor Gray
Write-Host "Private Key: $sslDir\server.key" -ForegroundColor Gray
Write-Host ""
Write-Host "You can now run:" -ForegroundColor Cyan
Write-Host "  docker-compose -f docker-compose.prod.yml -f docker-compose.yml up -d --build" -ForegroundColor White
