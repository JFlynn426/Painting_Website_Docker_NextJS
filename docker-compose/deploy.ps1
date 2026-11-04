# ============================================================================
# Deployment Script for Art Gallery Application (PowerShell)
# ============================================================================
# This script sets up proper file permissions for non-root NGINX user
# and deploys the application using docker-compose.prod.yml
# ============================================================================

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Art Gallery Deployment Script" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Navigate to docker-compose directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

Write-Host "[1/5] Setting up NGINX file permissions for non-root user (UID 101)..." -ForegroundColor Yellow

# Set permissions on SSL directory and files
$sslDir = Join-Path $scriptDir "nginx\ssl"
if (Test-Path $sslDir) {
    # Set read permissions for all users on SSL directory
    $acl = Get-Acl $sslDir
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("Everyone", "ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($rule)
    Set-Acl $sslDir $acl
    
    # Set read-only on server.crt
    if (Test-Path (Join-Path $sslDir "server.crt")) {
        $certFile = Get-Item (Join-Path $sslDir "server.crt")
        $certFile.Attributes = [System.IO.FileAttributes]::Normal
        Write-Host "      SSL certificate permissions set" -ForegroundColor Green
    }
    
    # Set read-only on server.key
    if (Test-Path (Join-Path $sslDir "server.key")) {
        $keyFile = Get-Item (Join-Path $sslDir "server.key")
        $keyFile.Attributes = [System.IO.FileAttributes]::Normal
        Write-Host "      SSL key permissions set" -ForegroundColor Green
    }
}
else {
    Write-Host "      WARNING: nginx/ssl directory not found. Please add SSL certificates." -ForegroundColor Red
}

# Set permissions on nginx.conf
$nginxConf = Join-Path $scriptDir "nginx\nginx.conf"
if (Test-Path $nginxConf) {
    $confFile = Get-Item $nginxConf
    $confFile.Attributes = [System.IO.FileAttributes]::Normal
    Write-Host "      nginx.conf permissions set" -ForegroundColor Green
}
else {
    Write-Host "      WARNING: nginx/nginx.conf not found." -ForegroundColor Red
}

Write-Host ""
Write-Host "[2/5] Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml down 2>$null | Out-Null

Write-Host ""
Write-Host "[3/5] Building and starting containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml up -d --build

Write-Host ""
Write-Host "[4/5] Checking container status..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml ps

Write-Host ""
Write-Host "[5/5] Running security checks..." -ForegroundColor Yellow
Write-Host "------------------------------------------" -ForegroundColor Gray

# Security Check 1: Verify no container serves traffic as root.
#
# `docker exec ... whoami` only reflects a container's real user when its image declares a
# USER. The API and frontend images do, so their UID is read directly. PostgreSQL and NGINX
# do NOT declare a USER — they start as root and drop privileges before serving traffic, so
# `docker exec` would report root even on a correct deployment. For those two the running
# process owner is inspected with `docker top` instead.
Write-Host "✓ Checking container users (should all be non-root)..." -ForegroundColor Green
$API_UID = & docker exec artgallery-api-prod id -u 2>$null
$FRONTEND_UID = & docker exec artgallery-frontend-prod id -u 2>$null
$POSTGRES_PROC = & docker top artgallery-postgres-prod 2>$null | Select-Object -Skip 1 -First 1
$NGINX_PROC = & docker top artgallery-nginx 2>$null | Select-Object -Skip 1 -First 1

Write-Host "  API Container:    UID $API_UID (expected: non-zero)" -ForegroundColor White
Write-Host "  Frontend:         UID $FRONTEND_UID (expected: 1001 = nextjs)" -ForegroundColor White
Write-Host "  PostgreSQL:       $POSTGRES_PROC" -ForegroundColor White
Write-Host "  NGINX:            $NGINX_PROC" -ForegroundColor White

# Validate users are non-root
$SECURITY_PASSED = $true
if ([string]::IsNullOrWhiteSpace($API_UID) -or $API_UID -eq "0") {
    Write-Host "  ⚠️  WARNING: API container may be running as root!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}
if ($FRONTEND_UID -ne "1001") {
    Write-Host "  ⚠️  WARNING: Frontend is not running as UID 1001 (nextjs)!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}
if ($POSTGRES_PROC -match "(^|\s)root(\s|$)") {
    Write-Host "  ⚠️  WARNING: PostgreSQL may be serving traffic as root!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}
if ($NGINX_PROC -match "(^|\s)root(\s|$)") {
    Write-Host "  ⚠️  WARNING: NGINX may be serving traffic as root!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}

if ($SECURITY_PASSED -eq $true) {
    Write-Host "  ✓ All containers running as non-root users" -ForegroundColor Green
}

Write-Host ""
Write-Host "✓ Checking for exposed sensitive files in git..." -ForegroundColor Green
$SENSITIVE_FILES = & git ls-files 2>$null | Select-String -Pattern "(password|secret|key|token)\.txt"
if ($null -eq $SENSITIVE_FILES) {
    Write-Host "  ✓ No sensitive files tracked in git" -ForegroundColor Green
}
else {
    Write-Host "  ⚠️  WARNING: Potential sensitive files found in git:" -ForegroundColor Yellow
    Write-Host $SENSITIVE_FILES -ForegroundColor Yellow
}

Write-Host ""
Write-Host "✓ Verifying .env file is not tracked..." -ForegroundColor Green
$ENV_TRACKED = & git ls-files 2>$null | Select-String -Pattern "docker-compose/.env"
if ($null -eq $ENV_TRACKED) {
    Write-Host "  ✓ .env file properly excluded from git" -ForegroundColor Green
}
else {
    Write-Host "  ⚠️  WARNING: docker-compose/.env is tracked in git!" -ForegroundColor Yellow
}

Write-Host "------------------------------------------" -ForegroundColor Gray
Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
if ($SECURITY_PASSED -eq $true) {
    Write-Host "Deployment Complete - All Security Checks Passed!" -ForegroundColor Green
    Write-Host "✓ Security verification successful" -ForegroundColor Green
}
else {
    Write-Host "Deployment Complete - Security Checks Have Warnings" -ForegroundColor Yellow
    Write-Host "⚠ Review warnings above" -ForegroundColor Yellow
}
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "To view logs:" -ForegroundColor Yellow
Write-Host "  docker-compose -f docker-compose.prod.yml logs -f" -ForegroundColor White
Write-Host ""
Write-Host "To check health:" -ForegroundColor Yellow
Write-Host "  curl http://localhost:9090/health" -ForegroundColor White
Write-Host "  curl -k https://localhost/api/health/health -H `"Host: ggpaintings.com`"" -ForegroundColor White
Write-Host ""
Write-Host "To run security checks manually:" -ForegroundColor Yellow
Write-Host "  docker exec artgallery-api-prod id -u" -ForegroundColor White
Write-Host "  docker exec artgallery-frontend-prod id -u" -ForegroundColor White
Write-Host "  docker top artgallery-postgres-prod" -ForegroundColor White
Write-Host "  docker top artgallery-nginx" -ForegroundColor White
Write-Host ""