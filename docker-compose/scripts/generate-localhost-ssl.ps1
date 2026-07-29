# ============================================================================
# Generate Self-Signed SSL Certificates for Localhost
# ============================================================================
# This script creates self-signed SSL certificates for localhost testing.
# Supports both single-site and multi-site configurations.
#
# Run this script once before starting the local Docker stack.
#
# Usage:
#   .\scripts\generate-localhost-ssl.ps1
# ============================================================================

$baseSslDir = Join-Path $PSScriptRoot "..\nginx\ssl\localhost"

# Helper function to generate SSL cert using Alpine Docker container
function Generate-SslCert {
    param(
        [string]$CertDir,
        [string]$CommonName,
        [string]$Organization
    )
    
    New-Item -ItemType Directory -Force -Path $CertDir | Out-Null
    $certDirUnix = $CertDir -replace '\\', '/'
    
    Write-Host "Generating self-signed SSL certificate for $CommonName..." -ForegroundColor Cyan
    
    docker run --rm `
        -v "${certDirUnix}:/certs" `
        alpine:3.18 `
        sh -c "apk add --no-cache openssl && openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /certs/server.key -out /certs/server.crt -subj '/C=US/ST=Florida/L=Local/O=$Organization/CN=$CommonName' -addext 'subjectAltName=DNS:localhost,IP:127.0.0.1' && chmod 644 /certs/server.key /certs/server.crt"
    
    Write-Host "  Certificate: $CertDir\server.crt" -ForegroundColor Gray
    Write-Host "  Private Key: $CertDir\server.key" -ForegroundColor Gray
}

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "SSL Certificate Generator for Localhost" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Single-site certificate (nginx.local.conf)
Generate-SslCert -CertDir $baseSslDir -CommonName "localhost" -Organization "Local Dev"

# 2. Multi-site certificates (nginx.multi.local.conf)
$ggSslDir = Join-Path $baseSslDir "gg"
$flynnSslDir = Join-Path $baseSslDir "flynn"

Generate-SslCert -CertDir $ggSslDir -CommonName "localhost" -Organization "GG Paintings Local"
Generate-SslCert -CertDir $flynnSslDir -CommonName "localhost" -Organization "Flynn Art Local"

Write-Host ""
Write-Host "All SSL certificates generated successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Single-site:" -ForegroundColor Cyan
Write-Host "  docker compose -f docker-compose.prod.yml -f docker-compose.yml up -d --build" -ForegroundColor White
Write-Host ""
Write-Host "Multi-site (ARM64):" -ForegroundColor Cyan
Write-Host "  docker compose -f docker-compose.multi.arm64.yml up -d --build" -ForegroundColor White
Write-Host ""
Write-Host "Multi-site (x86_64):" -ForegroundColor Cyan
Write-Host "  docker compose -f docker-compose.multi.yml -f docker-compose.multi.local.yml up -d --build" -ForegroundColor White
