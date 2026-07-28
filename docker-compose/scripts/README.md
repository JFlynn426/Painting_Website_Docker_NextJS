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

---

## Multi-Site Backup and Restore

For multi-site deployments (ggpaintings.com + flynnart.com), use the multi-site backup scripts.

### Install on Production Server

```bash
# Run multi-site installation script (requires sudo)
sudo ./install-backup-cron-multi.sh
```

This will:
1. Create `/opt/artgallery/` directory structure
2. Copy multi-site backup scripts to `/opt/artgallery/scripts/`
3. Install cron job for weekly backup (Sunday 2:00 AM)
4. Run a test backup for both sites

### Multi-Site Manual Backup

```bash
# Backup both sites
/opt/artgallery/scripts/backup-multi.sh

# Backup only ggpaintings.com (artgallery_gg database)
/opt/artgallery/scripts/backup-multi.sh gg

# Backup only flynnart.com (artgallery_flynn database)
/opt/artgallery/scripts/backup-multi.sh flynn
```

### Multi-Site Restore

```bash
# Restore ggpaintings.com from specific backup
/opt/artgallery/scripts/restore-multi.sh gg artgallery_artgallery_gg_20260101_120000.dump

# Restore flynnart.com (interactive - lists available backups)
/opt/artgallery/scripts/restore-multi.sh flynn
```

The multi-site restore script will:
1. Verify backup checksum
2. Prompt for confirmation
3. Stop the site-specific API container briefly
4. Drop and recreate the site database
5. Restore database from backup
6. Restart the site-specific API container

**Downtime per site:** ~10-30 seconds (one API container stop/start)

### Multi-Site File Structure

```
/opt/artgallery/
├── scripts/
│   ├── backup-multi.sh              # Multi-site backup script
│   ├── restore-multi.sh             # Multi-site restore script
│   ├── install-backup-cron-multi.sh # Multi-site installation script
│   ├── backup.sh                    # Single-site backup (compatibility)
│   ├── restore.sh                   # Single-site restore (compatibility)
│   └── backup.config                # Shared configuration (password, retention)
└── backups/
    ├── artgallery_artgallery_gg_20260101_020000.dump      # GG site backup
    ├── artgallery_artgallery_gg_20260101_020000.sha256    # GG checksum
    ├── artgallery_artgallery_flynn_20260101_020000.dump   # Flynn site backup
    ├── artgallery_artgallery_flynn_20260101_020000.sha256 # Flynn checksum
    └── backup.log                                         # Execution log
```

### Multi-Site Database Architecture

| Site | Container | Database | Backup Pattern |
|------|-----------|----------|----------------|
| ggpaintings.com | artgallery-api-gg | artgallery_gg | artgallery_artgallery_gg_*.dump |
| flynnart.com | artgallery-api-flynn | artgallery_flynn | artgallery_artgallery_flynn_*.dump |
| Shared | artgallery-postgres | (both databases) | - |

### Multi-Site Deployment with Backup Restore

The `deploy-multi.sh` script automatically restores the latest backup for each site during deployment:

```bash
# Deploy with automatic backup restore
./deploy-multi.sh
```

The deployment process:
1. Sets up NGINX permissions for SSL certificates
2. Stops existing containers
3. Starts PostgreSQL and waits for health
4. **Restores latest backup for each site** (if backups exist in `/opt/artgallery/backups/`)
5. Builds and starts remaining containers
6. Runs security checks

If no backups exist, the database seeder will populate initial data.

### Multi-Site Troubleshooting

#### Backup Fails: "Container not running"

```bash
# Check container status
docker ps | grep artgallery

# Start containers if stopped
cd docker-compose
docker compose -f docker-compose.multi.yml up -d
```

#### Verify Multi-Site Cron

```bash
# List cron jobs (should show backup-multi.sh)
crontab -l

# Check cron service
systemctl status cron

# View cron execution logs
grep CRON /var/log/syslog | grep artgallery
```

### Multi-Site Disaster Recovery Checklist

1. [ ] Access server via SSH
2. [ ] List available backups: `ls -lh /opt/artgallery/backups/*.dump`
3. [ ] For GG site: `/opt/artgallery/scripts/restore-multi.sh gg <backup_file>`
4. [ ] For Flynn site: `/opt/artgallery/scripts/restore-multi.sh flynn <backup_file>`
5. [ ] Confirm "RESTORE" when prompted for each site
6. [ ] Verify API health: `curl http://localhost:8080/health`
7. [ ] Test both site functionalities
8. [ ] Check logs: `tail -f /opt/artgallery/backups/restore.log`
