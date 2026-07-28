<#
.SYNOPSIS
    Creates PostgreSQL backup files for multi-site deployment.

.DESCRIPTION
    This script:
    1. Ensures PostgreSQL container is running and healthy
    2. Creates a backup directory at docker-compose/backups/
    3. Runs pg_dump inside the PostgreSQL container for specified site(s)
    4. Copies the .dump files out of the container to the local backups directory
    5. Generates SHA256 checksums for verification

.PARAMETER Site
    Which site(s) to backup. Options: "gg", "flynn", "all" (default: "all")

.PREREQUISITES
    - Docker Desktop running
    - artgallery-postgres container running and healthy
    - Run from the project root directory

.EXAMPLE
    .\docker-compose\scripts\create-local-backup-multi.ps1
    .\docker-compose\scripts\create-local-backup-multi.ps1 -Site gg
    .\docker-compose\scripts\create-local-backup-multi.ps1 -Site flynn
#>

param(
    [string]$Site = "all"
)

$ErrorActionPreference = "Stop"

# Resolve paths relative to docker-compose directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$composeDir = Split-Path -Parent $scriptDir
$backupDir = Join-Path $composeDir "backups"

# Site database mapping
$siteDatabases = @{
    gg    = "artgallery_gg"
    flynn = "artgallery_flynn"
}

# Container names
$containerName = "artgallery-postgres"

# Determine which sites to backup
$sitesToBackup = @()
switch ($Site) {
    "gg" { $sitesToBackup = @("gg") }
    "flynn" { $sitesToBackup = @("flynn") }
    "all" { $sitesToBackup = @("gg", "flynn") }
    default {
        Write-Error "Unknown site '$Site'. Use: gg, flynn, or all"
        exit 1
    }
}

# Load .env file (try .env.multi.local first, then .env)
$envFile = Join-Path $composeDir ".env.multi.local"
if (-not (Test-Path $envFile)) {
    $envFile = Join-Path $composeDir ".env"
}

if (-not (Test-Path $envFile)) {
    Write-Error ".env file not found at $envFile"
    exit 1
}

# Parse POSTGRES_PASSWORD from .env
$postgresPassword = $null
foreach ($line in Get-Content $envFile) {
    if ($line -match '^POSTGRES_PASSWORD=(.+)$') {
        $postgresPassword = $Matches[1]
        break
    }
}

if (-not $postgresPassword) {
    Write-Error "POSTGRES_PASSWORD not found in .env file"
    exit 1
}

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Multi-Site Local Backup Creation Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Ensure backup directory exists
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir | Out-Null
    Write-Host "Created backup directory: $backupDir"
}

# Check if container is running
Write-Host "Checking container '$containerName'..."
$containerStatus = docker inspect -f '{{.State.Status}}' $containerName 2>$null
if ($containerStatus -ne "running") {
    Write-Host "Container is not running. Starting it..." -ForegroundColor Yellow
    Set-Location $composeDir
    docker compose -f docker-compose.multi.yml up -d postgres
    Set-Location $PSScriptRoot

    # Wait for healthy
    Write-Host "Waiting for PostgreSQL to be healthy..." -ForegroundColor Yellow
    $maxWait = 120
    $elapsed = 0
    do {
        Start-Sleep -Seconds 3
        $elapsed += 3
        $health = docker inspect -f '{{.State.Health.Status}}' $containerName 2>$null
        if ($health -eq "healthy") { break }
        if ($elapsed -ge $maxWait) {
            Write-Error "Timeout waiting for PostgreSQL to become healthy"
            exit 1
        }
        Write-Host "  Waiting... ($elapsed seconds)" -NoNewline
        Write-Host "`r" -NoNewline
    } while ($true)
    Write-Host ""
    Write-Host "PostgreSQL is healthy!" -ForegroundColor Green
}

# Generate timestamp
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

# Backup each site
$failedSites = @()

foreach ($s in $sitesToBackup) {
    $dbName = $siteDatabases[$s]
    $backupFile = "artgallery_${dbName}_${timestamp}.dump"
    $backupPath = Join-Path $backupDir $backupFile

    Write-Host ""
    Write-Host "Backing up site '$s' (database: $dbName)..." -ForegroundColor Cyan

    # Step 1: Create backup inside container
    Write-Host "[1/3] Creating database backup inside container..."
    $env:PGPASSWORD = $postgresPassword
    $backupResult = docker exec $containerName pg_dump `
        -U postgres `
        -Fc `
        -f "/tmp/$backupFile" `
        $dbName `
        2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Backup command failed for site '$s':`n$backupResult"
        $failedSites += $s
        continue
    }

    # Step 2: Copy backup file out of container
    Write-Host "[2/3] Copying backup file to host..."
    docker cp "${containerName}:/tmp/$backupFile" $backupPath

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to copy backup file for site '$s'"
        $failedSites += $s
        continue
    }

    # Step 3: Clean up temp file
    Write-Host "[3/3] Cleaning up..."
    docker exec $containerName rm -f "/tmp/$backupFile" 2>$null | Out-Null

    # Verify backup file
    if (-not (Test-Path $backupPath)) {
        Write-Error "Backup file not found at $backupPath"
        $failedSites += $s
        continue
    }

    $fileSize = (Get-Item $backupPath).Length
    $fileSizeMB = [math]::Round($fileSize / 1MB, 2)

    # Generate checksum
    $checksum = Get-FileHash $backupPath -Algorithm SHA256
    Set-Content -Path "${backupPath}.sha256" -Value "$($checksum.Hash)  $backupFile"

    Write-Host ""
    Write-Host "Site '$s' backup completed!" -ForegroundColor Green
    Write-Host "  File:      $backupFile"
    Write-Host "  Size:      $fileSizeMB MB"
    Write-Host "  Checksum:  $($checksum.Hash)"
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Backup Summary" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

if ($failedSites.Count -gt 0) {
    Write-Host "Failed sites: $($failedSites -join ', ')" -ForegroundColor Red
    exit 1
}

Write-Host "All sites backed up successfully!" -ForegroundColor Green
Write-Host "Backup directory: $backupDir"
exit 0
