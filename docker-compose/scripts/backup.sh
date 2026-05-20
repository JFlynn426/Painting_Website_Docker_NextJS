#!/bin/bash
# =============================================================================
# Art Gallery Database Backup Script
# =============================================================================
# Usage: ./backup.sh
# Schedule: Run via cron weekly (e.g., every Sunday at 2:00 AM)
#
# Cron example (run at 2:00 AM every Sunday):
#   0 2 * * 0 /opt/artgallery/scripts/backup.sh
# =============================================================================

set -euo pipefail

# --- Configuration ---
CONTAINER_NAME="artgallery-sql-prod"
DATABASE_NAME="ArtGallery"
BACKUP_DIR="/opt/artgallery/backups"
LOG_FILE="/opt/artgallery/backups/backup.log"
RETENTION_DAYS=120
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="artgallery_db_$TIMESTAMP.bak"
BACKUP_PATH="$BACKUP_DIR/$BACKUP_FILE"

# --- Helper Functions ---
log() {
    local message="[$(date '+%Y-%m-%d %H:%M:%S')] $1"
    echo "$message" | tee -a "$LOG_FILE"
}

cleanup() {
    # Remove temporary backup file from container if it exists
    docker exec "$CONTAINER_NAME" rm -f "/tmp/$BACKUP_FILE" 2>/dev/null || true
}

# --- Pre-flight Checks ---

# Ensure backup directory exists
mkdir -p "$BACKUP_DIR"

# Check if Docker is running
if ! docker info >/dev/null 2>&1; then
    log "ERROR: Docker is not running"
    exit 1
fi

# Check if SQL Server container is running
if ! docker ps --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    log "ERROR: Container '$CONTAINER_NAME' is not running"
    exit 1
fi

# Load configuration
CONFIG_FILE="$(dirname "$0")/backup.config"
if [ ! -f "$CONFIG_FILE" ]; then
    log "ERROR: Configuration file not found: $CONFIG_FILE"
    log "Please run install-backup-cron.sh to create the configuration"
    exit 1
fi
source "$CONFIG_FILE"

# Use defaults if not overridden in config
CONTAINER_NAME="${CONTAINER_NAME:-artgallery-sql-prod}"
DATABASE_NAME="${DATABASE_NAME:-ArtGallery}"
RETENTION_DAYS="${RETENTION_DAYS:-30}"

if [ -z "$SQLSERVER_SA_PASSWORD" ]; then
    log "ERROR: SQLSERVER_SA_PASSWORD not set in $CONFIG_FILE"
    exit 1
fi
SA_PASSWORD="$SQLSERVER_SA_PASSWORD"

# --- Backup Execution ---

log "========================================="
log "Starting backup: $BACKUP_FILE"
log "========================================="

# Trap to clean up container temp file on failure
trap cleanup ERR

# Step 1: Create backup inside the Docker container (use /tmp/ since backup volume is read-only)
log "Creating database backup inside container..."
docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U SA \
    -P "$SA_PASSWORD" \
    -C \
    -Q "BACKUP DATABASE [$DATABASE_NAME] TO DISK = N'/tmp/$BACKUP_FILE' WITH INIT, STATS = 10" \
    2>&1 | tee -a "$LOG_FILE"

if [ ${PIPESTATUS[0]} -ne 0 ]; then
    log "ERROR: Database backup command failed"
    exit 1
fi

# Step 2: Copy the .bak file out of the container to the host
log "Copying backup file to host..."
docker cp "$CONTAINER_NAME:/tmp/$BACKUP_FILE" "$BACKUP_PATH"

if [ ${PIPESTATUS[0]} -ne 0 ]; then
    log "ERROR: Failed to copy backup file from container"
    exit 1
fi

# Step 3: Clean up temporary file inside container
cleanup
trap - ERR

# Step 4: Verify the backup file
if [ ! -s "$BACKUP_PATH" ]; then
    log "ERROR: Backup file is empty or missing"
    exit 1
fi

FILE_SIZE=$(du -h "$BACKUP_PATH" | cut -f1)
log "Backup file size: $FILE_SIZE"

# Step 5: Create checksum for verification
sha256sum "$BACKUP_PATH" > "${BACKUP_PATH}.sha256"
CHECKSUM=$(cut -d' ' -f1 "${BACKUP_PATH}.sha256")
log "SHA256 checksum: $CHECKSUM"

# Step 6: Clean up old backups (older than RETENTION_DAYS)
OLD_BACKUPS=$(find "$BACKUP_DIR" -name "*.bak" -mtime +$RETENTION_DAYS)
if [ -n "$OLD_BACKUPS" ]; then
    log "Removing backups older than $RETENTION_DAYS days..."
    find "$BACKUP_DIR" -name "*.bak" -mtime +$RETENTION_DAYS -delete
    find "$BACKUP_DIR" -name "*.sha256" -mtime +$RETENTION_DAYS -delete
    log "Old backups removed"
fi

# Step 7: List remaining backups
REMAINING=$(ls -1 "$BACKUP_DIR"/*.bak 2>/dev/null | wc -l)
TOTAL_SIZE=$(du -sh "$BACKUP_DIR" 2>/dev/null | cut -f1)
log "Remaining backups: $REMAINING (total size: $TOTAL_SIZE)"

log "========================================="
log "Backup completed successfully: $BACKUP_FILE"
log "========================================="

exit 0
