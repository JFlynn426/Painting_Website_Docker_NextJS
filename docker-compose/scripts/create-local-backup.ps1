<#
.SYNOPSIS
    Creates a SQL Server backup file locally for testing backup-based deployment.

.DESCRIPTION
    This script:
    1. Ensures SQL Server container is running and healthy
    2. Creates a backup directory at docker-compose/backups/
    3. Runs BACKUP DATABASE inside the SQL Server container
    4. Copies the .bak file out of the container to the local backups directory
    5. Generates a SHA256 checksum for verification

.PREREQUISITES
    - Docker Desktop running
    - artgallery-sql-prod container running and healthy
    - Run from the project root directory

.EXAMPLE
    .\docker-compose\scripts\create-local-backup.ps1
#>

param(
    [string]$ContainerName = "artgallery-sql-prod",
    [string]$DatabaseName = "ArtGallery"
)

$ErrorActionPreference = "Stop"

# Resolve paths relative to docker-compose directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$composeDir = Split-Path -Parent $scriptDir
$backupDir = Join-Path $composeDir "backups"

# Load .env file
$envFile = Join-Path $composeDir ".env"
if (-not (Test-Path $envFile)) {
    Write-Error ".env file not found at $envFile"
    exit 1
}

# Parse SQLSERVER_SA_PASSWORD from .env
$saPassword = $null
foreach ($line in Get-Content $envFile) {
    if ($line -match '^SQLSERVER_SA_PASSWORD=(.+)$') {
        $saPassword = $Matches[1]
        break
    }
}

if (-not $saPassword) {
    Write-Error "SQLSERVER_SA_PASSWORD not found in .env file"
    exit 1
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Local Backup Creation Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Ensure backup directory exists
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir | Out-Null
    Write-Host "Created backup directory: $backupDir"
}

# Check if container is running
Write-Host "Checking container '$ContainerName'..."
$containerStatus = docker inspect -f '{{.State.Status}}' $ContainerName 2>$null
if ($containerStatus -ne "running") {
    Write-Host "Container is not running. Starting it..." -ForegroundColor Yellow
    Set-Location $composeDir
    docker-compose -f docker-compose.prod.yml up -d sqlserver
    Set-Location $PSScriptRoot

    # Wait for healthy
    Write-Host "Waiting for SQL Server to be healthy..." -ForegroundColor Yellow
    $maxWait = 120
    $elapsed = 0
    do {
        Start-Sleep -Seconds 3
        $elapsed += 3
        $health = docker inspect -f '{{.State.Health.Status}}' $ContainerName 2>$null
        if ($health -eq "healthy") { break }
        if ($elapsed -ge $maxWait) {
            Write-Error "Timeout waiting for SQL Server to become healthy"
            exit 1
        }
        Write-Host "  Waiting... ($elapsed seconds)" -NoNewline
        Write-Host "`r" -NoNewline
    } while ($true)
    Write-Host ""
    Write-Host "SQL Server is healthy!" -ForegroundColor Green
}

# Generate timestamp and backup filename
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = "artgallery_db_$timestamp.bak"
$backupPath = Join-Path $backupDir $backupFile

Write-Host ""
Write-Host "Creating backup: $backupFile" -ForegroundColor Cyan
Write-Host ""

# Step 1: Create backup inside container
Write-Host "[1/3] Creating database backup inside container..."
$backupResult = docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd `
    -S localhost `
    -U sa `
    -P $saPassword `
    -C `
    -Q "BACKUP DATABASE [$DatabaseName] TO DISK = N'/tmp/$backupFile' WITH INIT, STATS = 10" `
    2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error "Backup command failed:`n$backupResult"
    exit 1
}

# Step 2: Copy backup file out of container
Write-Host "[2/3] Copying backup file to host..."
docker cp "${ContainerName}:/tmp/$backupFile" $backupPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to copy backup file from container"
    exit 1
}

# Step 3: Clean up temp file inside container
Write-Host "[3/3] Cleaning up..."
docker exec $ContainerName rm -f "/tmp/$backupFile" 2>$null | Out-Null

# Verify backup file
if (-not (Test-Path $backupPath)) {
    Write-Error "Backup file not found at $backupPath"
    exit 1
}

$fileSize = (Get-Item $backupPath).Length
$fileSizeMB = [math]::Round($fileSize / 1MB, 2)

# Generate checksum
$checksum = Get-FileHash $backupPath -Algorithm SHA256
Set-Content -Path "${backupPath}.sha256" -Value "$($checksum.Hash)  $backupFile"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Backup Created Successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "File:      $backupFile"
Write-Host "Size:      $fileSizeMB MB"
Write-Host "SHA256:    $($checksum.Hash)"
Write-Host "Location:  $backupPath"
Write-Host ""
Write-Host "You can now test restore with:" -ForegroundColor Yellow
Write-Host "  .\docker-compose\scripts\restore-local-backup.ps1"
