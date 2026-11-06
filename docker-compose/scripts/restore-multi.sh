#!/bin/bash
# =============================================================================
# Multi-Site Art Gallery Database Restore Script (PostgreSQL)
# =============================================================================
# Usage: ./restore-multi.sh [site] [backup_file.dump]
#
# Restores a specific site's database from backup:
#   ./restore-multi.sh gg artgallery_artgallery_gg_20260101_120000.dump
#   ./restore-multi.sh flynn artgallery_artgallery_flynn_20260101_120000.dump
#
# If no backup file specified, lists available backups for the site.
#
# WARNING: This will REPLACE the specified site's database. All unbacked
# changes will be lost. The site's API container will be briefly stopped.
# =============================================================================

set -euo pipefail

# --- Configuration ---
CONTAINER_NAME="artgallery-postgres"
BACKUP_DIR="/opt/artgallery/backups"
LOG_FILE="/opt/artgallery/backups/restore.log"

# Site database mapping
declare -A SITE_DATABASES
SITE_DATABASES[gg]="artgallery_gg"
SITE_DATABASES[flynn]="artgallery_flynn"

# Site API container mapping
declare -A SITE_API_CONTAINERS
SITE_API_CONTAINERS[gg]="artgallery-api-gg"
SITE_API_CONTAINERS[flynn]="artgallery-api-flynn"

# --- Helper Functions ---
log() {
    local message="[$(date '+%Y-%m-%d %H:%M:%S')] $1"
    echo "$message" | tee -a "$LOG_FILE"
}

usage() {
    echo "Usage: $0 [site] [backup_file.dump]"
    echo ""
    echo "Sites: gg, flynn"
    echo ""
    echo "Available backups:"
    ls -lh "$BACKUP_DIR"/*.dump 2>/dev/null || echo "  No backups found in $BACKUP_DIR"
    exit 1
}

# --- Pre-flight Checks ---
mkdir -p "$BACKUP_DIR"

# Check arguments
SITE_ARG="${1:-}"
BACKUP_FILE="${2:-}"

if [ -z "$SITE_ARG" ]; then
    log "ERROR: Site argument required (gg or flynn)"
    usage
fi

if [ -z "${SITE_DATABASES[$SITE_ARG]+x}" ]; then
    log "ERROR: Unknown site '$SITE_ARG'. Use: gg or flynn"
    usage
fi

DATABASE_NAME="${SITE_DATABASES[$SITE_ARG]}"
API_CONTAINER="${SITE_API_CONTAINERS[$SITE_ARG]}"

# Find backup file
if [ -z "$BACKUP_FILE" ]; then
    # List available backups for this site
    SITE_BACKUPS=$(ls -1t "$BACKUP_DIR"/artgallery_${DATABASE_NAME}_*.dump 2>/dev/null || true)
    if [ -z "$SITE_BACKUPS" ]; then
        log "ERROR: No backup files found for site '$SITE_ARG' in $BACKUP_DIR"
        exit 1
    fi

    echo "========================================="
    echo "Available Backups for '$SITE_ARG' (newest first):"
    echo "========================================="
    ls -lht "$BACKUP_DIR"/artgallery_${DATABASE_NAME}_*.dump

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
if [[ "$BACKUP_FILE" == *.dump ]]; then
    if [ -f "$BACKUP_FILE" ]; then
        BACKUP_PATH="$BACKUP_FILE"
    elif [ -f "$BACKUP_DIR/$BACKUP_FILE" ]; then
        BACKUP_PATH="$BACKUP_DIR/$BACKUP_FILE"
    else
        log "ERROR: Backup file not found: $BACKUP_FILE"
        exit 1
    fi
else
    log "ERROR: File must be a .dump file"
    usage
fi

# Verify backup file
if [ ! -s "$BACKUP_PATH" ]; then
    log "ERROR: Backup file is empty or missing: $BACKUP_PATH"
    exit 1
fi

# Verify checksum
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
if [ -f "$CONFIG_FILE" ]; then
    source "$CONFIG_FILE"
fi

# --- Confirmation ---
FILE_SIZE=$(du -h "$BACKUP_PATH" | cut -f1)

echo ""
echo "========================================="
echo "Restore Confirmation"
echo "========================================="
echo "Site:      $SITE_ARG"
echo "Database:  $DATABASE_NAME"
echo "Backup:    $(basename "$BACKUP_PATH") ($FILE_SIZE)"
echo ""
read -r -p "This will REPLACE the '$DATABASE_NAME' database. Type 'RESTORE' to confirm:" CONFIRM

if [ "$CONFIRM" != "RESTORE" ]; then
    log "Restore cancelled by user"
    exit 0
fi

# --- Restore Execution ---
log "========================================="
log "Starting restore for site '$SITE_ARG'"
log "========================================="

# Step 1: Stop API container
log "Stopping API container '$API_CONTAINER'..."
docker stop "$API_CONTAINER" 2>/dev/null || true
log "API container stopped"

# Step 2: Copy backup into container
BACKUP_FILENAME=$(basename "$BACKUP_PATH")
log "Copying backup to container..."
docker cp "$BACKUP_PATH" "$CONTAINER_NAME:/tmp/$BACKUP_FILENAME"

if [ ${PIPESTATUS[0]} -ne 0 ]; then
    log "ERROR: Failed to copy backup to container"
    docker start "$API_CONTAINER" 2>/dev/null || true
    exit 1
fi

# Step 3: Drop and recreate database
log "Dropping and recreating database '$DATABASE_NAME'..."
docker exec "$CONTAINER_NAME" psql -U postgres -c "DROP DATABASE IF EXISTS $DATABASE_NAME;" 2>&1 | tee -a "$LOG_FILE"
docker exec "$CONTAINER_NAME" psql -U postgres -c "CREATE DATABASE $DATABASE_NAME;" 2>&1 | tee -a "$LOG_FILE"

# Step 4: Restore database
log "Restoring database from backup..."
docker exec -i "$CONTAINER_NAME" pg_restore \
    -U postgres \
    -d "$DATABASE_NAME" \
    "/tmp/$BACKUP_FILENAME" \
    2>&1 | tee -a "$LOG_FILE"

if [ ${PIPESTATUS[0]} -ne 0 ]; then
    log "ERROR: Database restore failed"
    # Clean up and restart API
    docker exec "$CONTAINER_NAME" rm -f "/tmp/$BACKUP_FILENAME" 2>/dev/null || true
    docker start "$API_CONTAINER" 2>/dev/null || true
    exit 1
fi

# Step 5: Clean up temp file
docker exec "$CONTAINER_NAME" rm -f "/tmp/$BACKUP_FILENAME" 2>/dev/null || true

# Step 6: Restart API container
log "Restarting API container '$API_CONTAINER'..."
docker start "$API_CONTAINER" 2>/dev/null || true
log "API container restarted"

log "========================================="
log "Restore completed successfully for site '$SITE_ARG'"
log "========================================="

exit 0
