<#
.SYNOPSIS
    Restores backup files to the multi-site PostgreSQL container.

.DESCRIPTION
    This script:
    1. Finds the specified backup file (or latest for the site)
    2. Stops the site's API container (if running)
    3. Copies the backup into the PostgreSQL container
    4. Drops and recreates the site's database
    5. Restores the database using pg_restore
    6. Restarts the API container

.PARAMETER Site
    Which site to restore. Options: "gg", "flynn" (required)

.PARAMETER BackupFile
    Optional. Specify a specific .dump file to restore. If omitted, uses the latest for the site.

.EXAMPLE
    .\docker-compose\scripts\restore-local-backup-multi.ps1 -Site gg
    .\docker-compose\scripts\restore-local-backup-multi.ps1 -Site flynn -BackupFile "artgallery_artgallery_flynn_20260101_120000.dump"
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("gg", "flynn")]
    [string]$Site,

    [string]$BackupFile = ""
)

$ErrorActionPreference = "Stop"

# Resolve paths
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$composeDir = Split-Path -Parent $scriptDir
$backupDir = Join-Path $composeDir "backups"

# Site database mapping
$siteDatabases = @{
    gg    = "artgallery_gg"
    flynn = "artgallery_flynn"
}

# Site API container mapping
$siteApiContainers = @{
    gg    = "artgallery-api-gg"
    flynn = "artgallery-api-flynn"
}

$containerName = "artgallery-postgres"
$dbName = $siteDatabases[$Site]
$apiContainerName = $siteApiContainers[$Site]

# Load .env file
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
Write-Host "Multi-Site Local Database Restore Script" -ForegroundColor Cyan
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
    $dumpFiles = Get-ChildItem -Path $backupDir -Filter "artgallery_${dbName}_*.dump" | Sort-Object LastWriteTime -Descending
    if (-not $dumpFiles) {
        Write-Error "No backup files found for site '$Site' in $backupDir"
        Write-Host "Create one first with: .\docker-compose\scripts\create-local-backup-multi.ps1 -Site $Site" -ForegroundColor Yellow
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
Write-Host "Site:      $Site" -ForegroundColor White
Write-Host "Database:  $dbName" -ForegroundColor White
Write-Host "Backup:    $backupFileName ($fileSizeMB MB)" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "This will REPLACE the '$dbName' database. Type 'RESTORE' to confirm:"
if ($confirm -ne 'RESTORE') {
    Write-Host "Restore cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Starting restore..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Stop API container
Write-Host "[1/5] Stopping API container '$apiContainerName'..."
docker stop $apiContainerName 2>$null | Out-Null
Write-Host "  API container stopped" -ForegroundColor Green

# Step 2: Copy backup into container
Write-Host "[2/5] Copying backup to container..."
docker cp $bakPath "${containerName}:/tmp/$backupFileName"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to copy backup to container"
    docker start $apiContainerName 2>$null | Out-Null
    exit 1
}
Write-Host "  Backup copied" -ForegroundColor Green

# Step 3: Drop and recreate database
Write-Host "[3/5] Dropping and recreating database '$dbName'..."
$env:PGPASSWORD = $postgresPassword
docker exec $containerName psql -U postgres -c "DROP DATABASE IF EXISTS $dbName;" 2>&1
docker exec $containerName psql -U postgres -c "CREATE DATABASE $dbName;" 2>&1
Write-Host "  Database recreated" -ForegroundColor Green

# Step 4: Restore database
Write-Host "[4/5] Restoring database..."
$restoreResult = docker exec -i $containerName pg_restore `
    -U postgres `
    -d $dbName `
    "/tmp/$backupFileName" `
    2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed:`n$restoreResult"
    # Clean up and restart API
    docker exec $containerName rm -f "/tmp/$backupFileName" 2>$null | Out-Null
    docker start $apiContainerName 2>$null | Out-Null
    exit 1
}
Write-Host "  Database restored!" -ForegroundColor Green

# Step 5: Clean up and restart API
Write-Host "[5/5] Cleaning up and restarting API..."
docker exec $containerName rm -f "/tmp/$backupFileName" 2>$null | Out-Null
docker start $apiContainerName 2>$null | Out-Null
Write-Host "  API container restarted" -ForegroundColor Green

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Restore completed successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Site:      $Site" -ForegroundColor White
Write-Host "Database:  $dbName" -ForegroundColor White
Write-Host "API:       $apiContainerName" -ForegroundColor White
exit 0
