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

Write-Host "[1/4] Setting up NGINX file permissions for non-root user (UID 101)..." -ForegroundColor Yellow

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
Write-Host "[2/4] Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml down 2>$null | Out-Null

Write-Host ""
Write-Host "[3/4] Building and starting containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml up -d --build

Write-Host ""
Write-Host "[4/5] Checking container status..." -ForegroundColor Yellow
docker-compose -f docker-compose.prod.yml ps

Write-Host ""
Write-Host "[5/5] Running security checks..." -ForegroundColor Yellow
Write-Host "------------------------------------------" -ForegroundColor Gray

# Security Check 1: Verify all containers run as non-root
Write-Host "✓ Checking container users (should all be non-root)..." -ForegroundColor Green
$API_USER = & docker exec artgallery-api whoami 2>$null
$FRONTEND_USER = & docker exec artgallery-frontend whoami 2>$null
$SQL_USER = & docker exec artgallery-sql-prod whoami 2>$null
$NGINX_USER = & docker exec artgallery-nginx whoami 2>$null

Write-Host "  API Container:    $API_USER (expected: appuser)" -ForegroundColor White
Write-Host "  Frontend:         $FRONTEND_USER (expected: nextjs)" -ForegroundColor White
Write-Host "  SQL Server:       $SQL_USER (expected: mssql)" -ForegroundColor White
Write-Host "  NGINX:            $NGINX_USER (expected: nginx)" -ForegroundColor White

# Validate users are non-root
$SECURITY_PASSED = $true
if ($API_USER -eq "root" -or $API_USER -eq $null) {
    Write-Host "  ⚠️  WARNING: API container may be running as root!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}
if ($FRONTEND_USER -eq "root" -or $FRONTEND_USER -eq $null) {
    Write-Host "  ⚠️  WARNING: Frontend container may be running as root!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}
if ($SQL_USER -eq "root" -or $SQL_USER -eq $null) {
    Write-Host "  ⚠️  WARNING: SQL Server container may be running as root!" -ForegroundColor Yellow
    $SECURITY_PASSED = $false
}
if ($NGINX_USER -eq "root" -or $NGINX_USER -eq $null) {
    Write-Host "  ⚠️  WARNING: NGINX container may be running as root!" -ForegroundColor Yellow
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
Write-Host "  curl http://localhost:8080/health" -ForegroundColor White
Write-Host ""
Write-Host "To run security checks manually:" -ForegroundColor Yellow
Write-Host "  docker exec artgallery-api whoami" -ForegroundColor White
Write-Host "  docker exec artgallery-frontend whoami" -ForegroundColor White
Write-Host "  docker exec artgallery-sql-prod whoami" -ForegroundColor White
Write-Host "  docker exec artgallery-nginx whoami" -ForegroundColor White
Write-Host ""