<#
.SYNOPSIS
    Restores the latest backup file to the local PostgreSQL container.

.DESCRIPTION
    This script:
    1. Finds the latest .dump file in docker-compose/backups/
    2. Stops the API container (if running)
    3. Copies the backup into the PostgreSQL container
    4. Restores the database using pg_restore with clean flag
    5. Restarts the API container

    This simulates the production deployment flow where backup restore
    happens before the API container starts.

.PARAMETER BackupFile
    Optional. Specify a specific .dump file to restore. If omitted, uses the latest.

.EXAMPLE
    .\docker-compose\scripts\restore-local-backup.ps1
    .\docker-compose\scripts\restore-local-backup.ps1 -BackupFile "artgallery_db_20260520_135236.dump"
#>

param(
    [string]$BackupFile = "",
    [string]$ContainerName = "artgallery-postgres-prod",
    [string]$ApiContainerName = "artgallery-api-prod",
    [string]$DatabaseName = "artgallery"
)

$ErrorActionPreference = "Stop"

# Resolve paths
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$composeDir = Split-Path -Parent $scriptDir
$backupDir = Join-Path $composeDir "backups"

# Load .env file
$envFile = Join-Path $composeDir ".env"
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
Write-Host "Local Database Restore Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Find backup file
if ($BackupFile) {
    $bakPath = Join-Path $backupDir $BackupFile
    if (-not (Test-Path $bakPath)) {
        Write-Error "Backup file not found: $bakPath"
        exit 1
    }
}
else {
    $dumpFiles = Get-ChildItem -Path $backupDir -Filter "*.dump" | Sort-Object LastWriteTime -Descending
    if (-not $dumpFiles) {
        Write-Error "No backup files found in $backupDir"
        Write-Host "Create one first with: .\docker-compose\scripts\create-local-backup.ps1" -ForegroundColor Yellow
        exit 1
    }
    $bakPath = $dumpFiles[0].FullName
}

$backupFileName = [System.IO.Path]::GetFileName($bakPath)
$fileSize = (Get-Item $bakPath).Length
$fileSizeMB = [math]::Round($fileSize / 1MB, 2)

# Verify checksum if available
$checksumFile = "${bakPath}.sha256"
if (Test-Path $checksumFile) {
    Write-Host "Verifying checksum..." -ForegroundColor Yellow
    $storedHash = (Get-Content $checksumFile).Split(' ')[0]
    $actualHash = (Get-FileHash $bakPath -Algorithm SHA256).Hash
    if ($storedHash -ne $actualHash) {
        Write-Warning "Checksum mismatch! Backup may be corrupted."
        $confirm = Read-Host "Continue anyway? (y/N)"
        if ($confirm -ne 'y' -and $confirm -ne 'Y') { exit 1 }
    }
    else {
        Write-Host "Checksum verified OK" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Database:    $DatabaseName" -ForegroundColor White
Write-Host "Backup:      $backupFileName ($fileSizeMB MB)" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "This will REPLACE the current database. Type 'RESTORE' to confirm:"
if ($confirm -ne 'RESTORE') {
    Write-Host "Restore cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Starting restore..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Stop API container
Write-Host "[1/4] Stopping API container..."
docker stop $ApiContainerName 2>$null | Out-Null
Write-Host "  API container stopped" -ForegroundColor Green

# Step 2: Copy backup into container (use /tmp/ since backup volume is read-only)
Write-Host "[2/4] Copying backup to container..."
docker cp $bakPath "${ContainerName}:/tmp/$backupFileName"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to copy backup to container"
    docker start $ApiContainerName 2>$null | Out-Null
    exit 1
}
Write-Host "  Backup copied" -ForegroundColor Green

# Step 3: Restore database using pg_restore
Write-Host "[3/4] Restoring database..."
$env:PGPASSWORD = $postgresPassword
$restoreResult = docker exec -i $ContainerName pg_restore `
    -U postgres `
    -c `
    -d $DatabaseName `
    "/tmp/$backupFileName" `
    2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed:`n$restoreResult"
    # Clean up and restart API
    docker exec $ContainerName rm -f "/tmp/$backupFileName" 2>$null | Out-Null
    docker start $ApiContainerName 2>$null | Out-Null
    exit 1
}
Write-Host "  Database restored!" -ForegroundColor Green

# Step 4: Cleanup temp file and restart API
Write-Host "[4/4] Cleaning up and restarting API..."
docker exec $ContainerName rm -f "/tmp/$backupFileName" 2>$null | Out-Null

# Restart API container
docker start $ApiContainerName 2>$null | Out-Null
Write-Host "  API container restarted" -ForegroundColor Green

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Restore Completed Successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Database:  $DatabaseName"
Write-Host "From:      $backupFileName"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Restart remaining containers:"
Write-Host "     docker-compose -f docker-compose.prod.yml up -d"
Write-Host "  2. Check health:"
Write-Host "     docker-compose -f docker-compose.prod.yml ps"
