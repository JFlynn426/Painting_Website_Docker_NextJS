<#
.SYNOPSIS
    Restores the latest backup file to the local SQL Server container.

.DESCRIPTION
    This script:
    1. Finds the latest .bak file in docker-compose/backups/
    2. Stops the API container (if running)
    3. Copies the backup into the SQL Server container
    4. Restores the database with REPLACE
    5. Restarts the API container

    This simulates the production deployment flow where backup restore
    happens before the API container starts.

.PARAMETER BackupFile
    Optional. Specify a specific .bak file to restore. If omitted, uses the latest.

.EXAMPLE
    .\docker-compose\scripts\restore-local-backup.ps1
    .\docker-compose\scripts\restore-local-backup.ps1 -BackupFile "artgallery_db_20260520_135236.bak"
#>

param(
    [string]$BackupFile = "",
    [string]$ContainerName = "artgallery-sql-prod",
    [string]$ApiContainerName = "artgallery-api-prod",
    [string]$DatabaseName = "ArtGallery"
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
    $bakFiles = Get-ChildItem -Path $backupDir -Filter "*.bak" | Sort-Object LastWriteTime -Descending
    if (-not $bakFiles) {
        Write-Error "No backup files found in $backupDir"
        Write-Host "Create one first with: .\docker-compose\scripts\create-local-backup.ps1" -ForegroundColor Yellow
        exit 1
    }
    $bakPath = $bakFiles[0].FullName
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
Write-Host "[1/5] Stopping API container..."
docker stop $ApiContainerName 2>$null | Out-Null
Write-Host "  API container stopped" -ForegroundColor Green

# Step 2: Copy backup into container (use /tmp/ since backup volume is read-only)
Write-Host "[2/5] Copying backup to container..."
docker cp $bakPath "${ContainerName}:/tmp/$backupFileName"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to copy backup to container"
    docker start $ApiContainerName 2>$null | Out-Null
    exit 1
}
Write-Host "  Backup copied" -ForegroundColor Green

# Step 3: Set SINGLE_USER mode
Write-Host "[3/5] Setting database to SINGLE_USER mode..."
docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd `
    -S localhost -U sa -P $saPassword -C `
    -Q "ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE" 2>&1 | Out-Null
Write-Host "  Database in SINGLE_USER mode" -ForegroundColor Green

# Step 4: Restore database
Write-Host "[4/5] Restoring database..."
$restoreResult = docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd `
    -S localhost -U sa -P $saPassword -C `
    -Q "RESTORE DATABASE [$DatabaseName] FROM DISK = N'/tmp/$backupFileName' WITH REPLACE, RECOVERY" `
    2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed:`n$restoreResult"
    # Set back to multi-user and restart API
    docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P $saPassword -C `
        -Q "ALTER DATABASE [$DatabaseName] SET MULTI_USER" 2>$null | Out-Null
    docker start $ApiContainerName 2>$null | Out-Null
    exit 1
}
Write-Host "  Database restored!" -ForegroundColor Green

# Step 5: Set MULTI_USER mode and cleanup
Write-Host "[5/5] Setting MULTI_USER mode and cleaning up..."
docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd `
    -S localhost -U sa -P $saPassword -C `
    -Q "ALTER DATABASE [$DatabaseName] SET MULTI_USER" 2>&1 | Out-Null
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
