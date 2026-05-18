# Art Gallery Database Backup

Automated backup and restore scripts for the Art Gallery SQL Server database.

## Quick Start

### Install on Production Server

```bash
# Run installation script (requires sudo)
sudo ./install-backup-cron.sh
```

This will:
1. Create `/opt/artgallery/` directory structure
2. Copy backup scripts to `/opt/artgallery/scripts/`
3. Install cron job for weekly backup (Sunday 2:00 AM)
4. Run a test backup to verify everything works

### Manual Backup

```bash
/opt/artgallery/scripts/backup.sh
```

### Restore from Backup

```bash
# List available backups
ls -lh /opt/artgallery/backups/*.bak

# Restore specific backup
/opt/artgallery/scripts/restore.sh artgallery_db_20260518_020000.bak

# Interactive restore (prompts for backup selection)
/opt/artgallery/scripts/restore.sh
```

## File Structure

```
/opt/artgallery/
├── scripts/
│   ├── backup.sh              # Backup script
│   ├── restore.sh             # Restore script
│   └── install-backup-cron.sh # Installation script
└── backups/
    ├── artgallery_db_20260511_020000.bak   # Backup file
    ├── artgallery_db_20260511_020000.sha256 # Checksum
    ├── artgallery_db_20260518_020000.bak
    ├── artgallery_db_20260518_020000.sha256
    └── backup.log                          # Execution log
```

## Cron Schedule

Default: **Every Sunday at 2:00 AM**

```
0 2 * * 0 /opt/artgallery/scripts/backup.sh
```

### Change Schedule

Edit crontab:
```bash
crontab -e
```

Common schedules:
```
# Every day at 3:00 AM
0 3 * * *

# Every Saturday at 1:00 AM
0 1 * * 6

# Twice weekly (Monday and Thursday at 2:00 AM)
0 2 * * 1,4
```

## Backup Retention

Backups older than **30 days** are automatically deleted.

To change retention, edit `backup.sh`:
```bash
RETENTION_DAYS=30  # Change this value
```

## Monitoring

### Check Backup Log

```bash
# View full log
cat /opt/artgallery/backups/backup.log

# Follow log in real-time
tail -f /opt/artgallery/backups/backup.log

# Check last backup
tail -5 /opt/artgallery/backups/backup.log
```

### Verify Cron is Running

```bash
# List cron jobs
crontab -l

# Check cron service
systemctl status cron

# View cron execution logs
grep CRON /var/log/syslog | grep artgallery
```

### Check Available Backups

```bash
ls -lh /opt/artgallery/backups/*.bak
```

## Restore Process

### Full Restore (Replaces Current Database)

```bash
/opt/artgallery/scripts/restore.sh artgallery_db_20260511_020000.bak
```

The restore script will:
1. Verify backup checksum
2. Prompt for confirmation
3. Stop API container briefly
4. Restore database
5. Restart API container

**Downtime:** ~10-30 seconds (API container stop/start)

### Verify Restore

After restore, check the API health:
```bash
curl http://localhost:8080/api/health/health
```

## Troubleshooting

### Backup Fails: "Container not running"

```bash
# Check container status
docker ps | grep artgallery

# Start containers if stopped
cd docker-compose
docker-compose -f docker-compose.prod.yml up -d
```

### Backup Fails: "Password not found"

The script reads the password from Docker secrets. Ensure secrets are properly mounted:

```bash
# Check if secret file exists (inside container context)
docker exec artgallery-sql-prod cat /run/secrets/sqlserver_sa_password
```

### Backup Fails: "Permission denied"

```bash
# Ensure scripts are executable
sudo chmod +x /opt/artgallery/scripts/*.sh

# Ensure backup directory is writable
sudo chmod 755 /opt/artgallery/backups
```

### Cron Not Running

```bash
# Start cron service
sudo systemctl start cron

# Enable cron on boot
sudo systemctl enable cron

# Check cron logs
sudo grep CRON /var/log/syslog
```

## Security Notes

- Backup files contain sensitive data
- Backup directory should have restricted permissions: `chmod 700 /opt/artgallery/backups`
- Consider encrypting backup files for offsite storage
- SHA256 checksums are generated for each backup to verify integrity

## Offsite Backup (Optional)

To sync backups to cloud storage using rclone:

```bash
# Install rclone
sudo apt install rclone

# Configure remote (e.g., Google Drive, S3)
rclone config

# Add to backup.sh before the cleanup step:
rclone copy /opt/artgallery/backups/ remote:artgallery-backups/ --checksum
```

## Disaster Recovery Checklist

1. [ ] Access server via SSH
2. [ ] List available backups: `ls -lh /opt/artgallery/backups/*.bak`
3. [ ] Choose appropriate backup file
4. [ ] Run restore: `/opt/artgallery/scripts/restore.sh <backup_file>`
5. [ ] Confirm "RESTORE" when prompted
6. [ ] Verify API health: `curl http://localhost:8080/api/health/health`
7. [ ] Test site functionality
8. [ ] Check logs: `tail -f /opt/artgallery/backups/restore.log`
