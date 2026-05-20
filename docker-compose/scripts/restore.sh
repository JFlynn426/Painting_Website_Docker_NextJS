#!/bin/bash
# =============================================================================
# Art Gallery Database Restore Script
# =============================================================================
# Usage: ./restore.sh [backup_file.bak]
#
# If no backup file specified, lists available backups and prompts for selection.
#
# WARNING: This will REPLACE the current database. All unbacked changes will be
# lost. The API container will be briefly stopped during restore.
# =============================================================================

set -euo pipefail

# --- Configuration ---
CONTAINER_NAME="artgallery-sql-prod"
DATABASE_NAME="ArtGallery"
BACKUP_DIR="/opt/artgallery/backups"
LOG_FILE="/opt/artgallery/backups/restore.log"

# --- Helper Functions ---
log() {
    local message="[$(date '+%Y-%m-%d %H:%M:%S')] $1"
    echo "$message" | tee -a "$LOG_FILE"
}

usage() {
    echo "Usage: $0 [backup_file.bak]"
    echo ""
    echo "Available backups:"
    ls -lh "$BACKUP_DIR"/*.bak 2>/dev/null || echo "  No backups found in $BACKUP_DIR"
    exit 1
}

# --- Pre-flight Checks ---

mkdir -p "$BACKUP_DIR"

# Check arguments
BACKUP_FILE="${1:-}"

if [ -z "$BACKUP_FILE" ]; then
    # No file specified - list available backups
    BACKUPS=$(ls -1t "$BACKUP_DIR"/*.bak 2>/dev/null || true)
    if [ -z "$BACKUPS" ]; then
        log "ERROR: No backup files found in $BACKUP_DIR"
        exit 1
    fi

    echo "========================================="
    echo "Available Backups (newest first):"
    echo "========================================="
    ls -lht "$BACKUP_DIR"/*.bak

    echo ""
    echo "Enter backup filename to restore (or 'q' to quit):"
    read -r BACKUP_FILE
    echo ""

    if [ "$BACKUP_FILE" = "q" ] || [ -z "$BACKUP_FILE" ]; then
        log "Restore cancelled by user"
        exit 0
    fi
fi

# Resolve full path
if [[ "$BACKUP_FILE" == *.bak ]]; then
    if [ -f "$BACKUP_FILE" ]; then
        BACKUP_PATH="$BACKUP_FILE"
    elif [ -f "$BACKUP_DIR/$BACKUP_FILE" ]; then
        BACKUP_PATH="$BACKUP_DIR/$BACKUP_FILE"
    else
        log "ERROR: Backup file not found: $BACKUP_FILE"
        exit 1
    fi
else
    log "ERROR: File must be a .bak file"
    usage
fi

# Verify backup file exists and is not empty
if [ ! -s "$BACKUP_PATH" ]; then
    log "ERROR: Backup file is empty or missing: $BACKUP_PATH"
    exit 1
fi

# Verify checksum if available
CHECKSUM_FILE="${BACKUP_PATH}.sha256"
if [ -f "$CHECKSUM_FILE" ]; then
    log "Verifying backup checksum..."
    if ! sha256sum -c "$CHECKSUM_FILE" >/dev/null 2>&1; then
        log "WARNING: Checksum verification failed! Backup may be corrupted."
        read -r -p "Continue anyway? (y/N): " CONFIRM
        if [ "$CONFIRM" != "y" ] && [ "$CONFIRM" != "Y" ]; then
            log "Restore cancelled by user"
            exit 0
        fi
    else
        log "Checksum verified OK"
    fi
fi

# Load configuration
CONFIG_FILE="$(dirname "$0")/backup.config"
if [ ! -f "$CONFIG_FILE" ]; then
    log "ERROR: Configuration file not found: $CONFIG_FILE"
    exit 1
fi
source "$CONFIG_FILE"

# Use defaults if not overridden in config
CONTAINER_NAME="${CONTAINER_NAME:-artgallery-sql-prod}"
DATABASE_NAME="${DATABASE_NAME:-ArtGallery}"

if [ -z "$SQLSERVER_SA_PASSWORD" ]; then
    log "ERROR: SQLSERVER_SA_PASSWORD not set in $CONFIG_FILE"
    exit 1
fi
SA_PASSWORD="$SQLSERVER_SA_PASSWORD"

# --- Confirmation ---
FILENAME=$(basename "$BACKUP_PATH")
FILE_SIZE=$(du -h "$BACKUP_PATH" | cut -f1)

echo ""
echo "========================================="
echo "DATABASE RESTORE CONFIRMATION"
echo "========================================="
echo "Database:    $DATABASE_NAME"
echo "Backup:      $FILENAME"
echo "Size:        $FILE_SIZE"
echo ""
echo "WARNING: This will REPLACE the current database."
echo "All changes since this backup will be lost."
echo "========================================="
echo ""
read -r -p "Type 'RESTORE' to confirm: " CONFIRM

if [ "$CONFIRM" != "RESTORE" ]; then
    log "Restore cancelled by user"
    exit 0
fi

# --- Restore Execution ---

log "========================================="
log "Starting restore from: $FILENAME"
log "========================================="

# Step 1: Stop API container to prevent connections during restore
log "Stopping API container..."
docker stop artgallery-api-prod 2>/dev/null || true
log "API container stopped"

# Step 2: Copy backup file into the SQL Server container (use /tmp/ since backup volume is read-only)
log "Copying backup file to container..."
docker cp "$BACKUP_PATH" "$CONTAINER_NAME:/tmp/$FILENAME"

# Step 3: Set database to SINGLE_USER mode to kill existing connections
log "Setting database to SINGLE_USER mode..."
docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U SA \
    -P "$SA_PASSWORD" \
    -C \
    -Q "ALTER DATABASE [$DATABASE_NAME] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"

# Step 4: Restore the database
log "Restoring database..."
docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U SA \
    -P "$SA_PASSWORD" \
    -C \
    -Q "RESTORE DATABASE [$DATABASE_NAME] FROM DISK = N'/tmp/$FILENAME' WITH REPLACE, RECOVERY" \
    2>&1 | tee -a "$LOG_FILE"

RESTORE_EXIT=${PIPESTATUS[0]}

# Step 5: Set database back to MULTI_USER mode
log "Setting database to MULTI_USER mode..."
docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U SA \
    -P "$SA_PASSWORD" \
    -C \
    -Q "ALTER DATABASE [$DATABASE_NAME] SET MULTI_USER"

# Step 6: Clean up temp file in container
docker exec "$CONTAINER_NAME" rm -f "/tmp/$FILENAME" 2>/dev/null || true

# Step 7: Restart API container
log "Starting API container..."
docker start artgallery-api-prod 2>/dev/null || true
log "API container started"

# --- Result ---
if [ $RESTORE_EXIT -ne 0 ]; then
    log "ERROR: Database restore failed (exit code: $RESTORE_EXIT)"
    log "API container has been restarted"
    exit 1
fi

log "========================================="
log "Restore completed successfully!"
log "Database: $DATABASE_NAME"
log "From: $FILENAME"
log "========================================="

exit 0
