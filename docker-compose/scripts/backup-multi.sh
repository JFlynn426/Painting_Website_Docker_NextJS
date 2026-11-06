#!/bin/bash
# =============================================================================
# Multi-Site Art Gallery Database Backup Script (PostgreSQL)
# =============================================================================
# Usage: ./backup-multi.sh [site]
#
# Backs up one or both site databases:
#   ./backup-multi.sh          - Backup both sites
#   ./backup-multi.sh gg       - Backup only gg site (artgallery_gg)
#   ./backup-multi.sh flynn    - Backup only flynn site (artgallery_flynn)
#   ./backup-multi.sh all      - Backup both sites (explicit)
#
# Schedule: Run via cron weekly (e.g., every Sunday at 2:00 AM)
#   0 2 * * 0 /opt/artgallery/scripts/backup-multi.sh
# =============================================================================

set -euo pipefail

# --- Configuration ---
CONTAINER_NAME="artgallery-postgres"
BACKUP_DIR="/opt/artgallery/backups"
LOG_FILE="/opt/artgallery/backups/backup.log"
RETENTION_DAYS=120
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Site database mapping
declare -A SITE_DATABASES
SITE_DATABASES[gg]="artgallery_gg"
SITE_DATABASES[flynn]="artgallery_flynn"

# --- Helper Functions ---
log() {
    local message="[$(date '+%Y-%m-%d %H:%M:%S')] $1"
    echo "$message" | tee -a "$LOG_FILE"
}

cleanup() {
    # Remove temporary backup files from container
    for site in "${SITES_TO_BACKUP[@]}"; do
        local db="${SITE_DATABASES[$site]}"
        local file="artgallery_${db}_${TIMESTAMP}.dump"
        docker exec "$CONTAINER_NAME" rm -f "/tmp/$file" 2>/dev/null || true
    done
}

backup_site() {
    local site="$1"
    local db="${SITE_DATABASES[$site]}"
    local backup_file="artgallery_${db}_${TIMESTAMP}.dump"
    local backup_path="$BACKUP_DIR/$backup_file"

    log "Backing up site '$site' (database: $db)..."

    # Step 1: Create backup inside container
    docker exec "$CONTAINER_NAME" pg_dump \
        -U postgres \
        -Fc \
        -f "/tmp/$backup_file" \
        "$db" \
        2>&1 | tee -a "$LOG_FILE"

    if [ ${PIPESTATUS[0]} -ne 0 ]; then
        log "ERROR: Backup failed for site '$site'"
        return 1
    fi

    # Step 2: Copy backup to host
    docker cp "$CONTAINER_NAME:/tmp/$backup_file" "$backup_path"

    if [ ${PIPESTATUS[0]} -ne 0 ]; then
        log "ERROR: Failed to copy backup for site '$site'"
        return 1
    fi

    # Step 3: Clean up temp file
    docker exec "$CONTAINER_NAME" rm -f "/tmp/$backup_file" 2>/dev/null || true

    # Step 4: Verify
    if [ ! -s "$backup_path" ]; then
        log "ERROR: Backup file is empty for site '$site'"
        return 1
    fi

    local file_size=$(du -h "$backup_path" | cut -f1)
    log "Backup file size: $file_size"

    # Step 5: Create checksum
    sha256sum "$backup_path" > "${backup_path}.sha256"
    local checksum=$(cut -d' ' -f1 "${backup_path}.sha256")
    log "SHA256 checksum: $checksum"

    log "Backup completed for site '$site': $backup_file"
    return 0
}

# --- Determine which sites to backup ---
SITE_ARG="${1:-all}"

case "$SITE_ARG" in
    gg|flynn)
        SITES_TO_BACKUP=("$SITE_ARG")
        ;;
    all|"")
        SITES_TO_BACKUP=("gg" "flynn")
        ;;
    *)
        log "ERROR: Unknown site '$SITE_ARG'. Use: gg, flynn, or all"
        exit 1
        ;;
esac

# --- Pre-flight Checks ---
mkdir -p "$BACKUP_DIR"

if ! docker info >/dev/null 2>&1; then
    log "ERROR: Docker is not running"
    exit 1
fi

if ! docker ps --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    log "ERROR: Container '$CONTAINER_NAME' is not running"
    exit 1
fi

# Load configuration
CONFIG_FILE="$(dirname "$0")/backup.config"
if [ -f "$CONFIG_FILE" ]; then
    source "$CONFIG_FILE"
    RETENTION_DAYS="${RETENTION_DAYS:-30}"
fi

if [ -z "${POSTGRES_PASSWORD:-}" ]; then
    log "WARNING: POSTGRES_PASSWORD not set in $CONFIG_FILE (may not be needed for docker exec)"
fi

# --- Backup Execution ---
trap cleanup ERR

log "========================================="
log "Starting multi-site backup: ${SITES_TO_BACKUP[*]}"
log "========================================="

FAILED_SITES=()

for site in "${SITES_TO_BACKUP[@]}"; do
    if ! backup_site "$site"; then
        FAILED_SITES+=("$site")
    fi
done

trap - ERR

# --- Cleanup old backups ---
if [ -n "${RETENTION_DAYS:-}" ]; then
    OLD_BACKUPS=$(find "$BACKUP_DIR" -name "*.dump" -mtime +$RETENTION_DAYS)
    if [ -n "$OLD_BACKUPS" ]; then
        log "Removing backups older than $RETENTION_DAYS days..."
        find "$BACKUP_DIR" -name "*.dump" -mtime +$RETENTION_DAYS -delete
        find "$BACKUP_DIR" -name "*.sha256" -mtime +$RETENTION_DAYS -delete
        log "Old backups removed"
    fi
fi

# --- Summary ---
REMAINING=$(ls -1 "$BACKUP_DIR"/*.dump 2>/dev/null | wc -l)
TOTAL_SIZE=$(du -sh "$BACKUP_DIR" 2>/dev/null | cut -f1)
log "Remaining backups: $REMAINING (total size: $TOTAL_SIZE)"

if [ ${#FAILED_SITES[@]} -gt 0 ]; then
    log "========================================="
    log "Backup completed with errors: ${FAILED_SITES[*]}"
    log "========================================="
    exit 1
fi

log "========================================="
log "All backups completed successfully: ${SITES_TO_BACKUP[*]}"
log "========================================="

exit 0
