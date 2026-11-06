# Art Gallery Database Backup & Restore

`pg_dump -Fc` / `pg_restore` scripts in custom format (`.dump` files), plus SHA256
checksums and retention pruning.

There are **two independent script families**, and they do not share a backup directory:

| Family | Runs on | Backup directory | Target stack |
|--------|---------|------------------|--------------|
| `*-multi.sh`, `create-local-backup-multi.ps1`, `restore-local-backup-multi.ps1` | current | see below | **`docker-compose.multi.yml`** — container `artgallery-postgres`, databases `artgallery_gg` + `artgallery_flynn` |
| `backup.sh`, `restore.sh`, `create-local-backup.ps1`, `restore-local-backup.ps1` | — | see below | **retired** `docker-compose.prod.yml` — container `artgallery-postgres-prod`, database `artgallery` |

Use the multi-site scripts unless the host is still running the single-site stack. The
retired scripts are kept only so an old gg-only host can still be backed up; see
[Legacy single-site scripts](#legacy-single-site-scripts-retired) at the end.

## Which stack am I on?

```bash
docker ps --format '{{.Names}}' | grep -E 'artgallery-(api|frontend|nginx|postgres)' || echo "nothing running"
```

`artgallery-api-gg` / `artgallery-api-flynn` / `artgallery-postgres` means multi-site.
A `-prod` suffix means a single-site stack. `artgallery-nginx` exists in every stack and
tells you nothing on its own.

## Cutover preparation (`prepare-cutover.sh`)

Bridges a single-site host onto the multi-site stack. Run it from `docker-compose/` **before**
`deploy-multi.sh`.

```bash
cd docker-compose
./scripts/prepare-cutover.sh --check    # read-only report, safe to run any time
./scripts/prepare-cutover.sh --fix      # apply the config fixes
./scripts/prepare-cutover.sh --backup   # dump artgallery + copy images out of image_data
```

`--fix` covers the four things that otherwise fail silently: the missing exec bit on the shell
scripts, a missing `.env.multi`, placeholder `Admin__JwtSecretKey` values, and OAuth redirect
URIs still pointing at `localhost`. It also repoints CORS, tops up the flynn admin allowlist,
and copies the live Postgres password into `.env.multi` + `secrets/postgres_password` so a
reused data volume keeps authenticating.

It cannot register redirect URIs with Google/Yahoo — do that first, or admin login fails with
`redirect_uri_mismatch` regardless of what `.env.multi` says.

Then finish with:

```bash
./scripts/generate-prod-ssl.sh   # self-signed origin certs for Cloudflare Full mode
./deploy-multi.sh
```

## File layout

```
/opt/artgallery/                     # server (Linux, cron)
├── scripts/
│   ├── prepare-cutover.sh           # single-site -> multi-site cutover checks + fixes
│   ├── backup-multi.sh              # multi-site backup  -> /opt/artgallery/backups
│   ├── restore-multi.sh             # multi-site restore <- /opt/artgallery/backups
│   ├── install-backup-cron-multi.sh # installer (also copies the legacy scripts)
│   ├── backup.sh                    # legacy single-site, do not run on a multi host
│   ├── restore.sh                   # legacy single-site
│   └── backup.config                # password + retention (created by the installer, chmod 600)
└── backups/
    ├── artgallery_artgallery_gg_20261101_020000.dump
    ├── artgallery_artgallery_gg_20261101_020000.sha256
    ├── artgallery_artgallery_flynn_20261101_020000.dump
    ├── artgallery_artgallery_flynn_20261101_020000.sha256
    └── backup.log

docker-compose/backups/              # local (Windows PowerShell scripts)
└── *.dump, *.sha256
```

`deploy-multi.sh` restores from `$BACKUP_DIR` (default `/opt/artgallery/backups`,
overridable in `.env.multi`) — **not** from `docker-compose/backups/`. A dump produced by
`create-local-backup*.ps1` will not be picked up by a server deploy unless you copy it
into `$BACKUP_DIR` first.

## Filename glob matters

`deploy-multi.sh` and `restore-multi.sh` look for `artgallery_<database>_<timestamp>.dump`,
for example `artgallery_artgallery_gg_20261105_020000.dump`.

The legacy `backup.sh` and `create-local-backup.ps1` write `artgallery_db_<timestamp>.dump`,
which does **not** match that glob. A backup carried over from the old gg-only stack is
silently skipped during a multi-site cutover. Either rename it to
`artgallery_artgallery_gg_<timestamp>.dump` before deploying, or restore it explicitly:

```bash
./scripts/restore-multi.sh gg artgallery_db_20260518_020000.dump
```

`restore-multi.sh` accepts an explicit filename; only the *automatic* lookup is glob-bound.

---

## Multi-Site Backup (current stack)

### Install on the production server

```bash
cd docker-compose
sudo ./scripts/install-backup-cron-multi.sh
```

This will:

1. Create `/opt/artgallery/scripts/` and `/opt/artgallery/backups/`
2. Copy `backup-multi.sh` and `restore-multi.sh` there (plus the legacy single-site
   scripts, for compatibility)
3. Generate `/opt/artgallery/scripts/backup.config` with `chmod 600`
4. Install a cron job for weekly backup (Sunday 2:00 AM)
5. Run a test backup for both sites

> **The generated `backup.config` is a placeholder.** The installer looks for
> `.env.multi` *inside* `scripts/`, which is not where that file lives, so it falls into
> the template branch and writes `POSTGRES_PASSWORD=YourStrongPassword!`. Edit it by hand
> to match `.env.multi`, or just leave it — `pg_dump` runs over the container's local
> socket, which PostgreSQL trusts, so the value is only a presence check. `backup-multi.sh`
> prints a warning and continues when it is unset.

> **Do not run `backup.sh` on a multi-site host.** The installer copies it into the same
> directory and the shared `backup.config` sets `CONTAINER_NAME=artgallery-postgres`.
> `backup.sh` checks for `artgallery-postgres-prod` before sourcing the config, then runs
> `pg_dump` *after* the source has retargeted the container — so it would try to dump a
> database named `artgallery` from the multi-site container, where no such database
> exists.

### Manual backup

```bash
# Both sites
/opt/artgallery/scripts/backup-multi.sh

# One site only
/opt/artgallery/scripts/backup-multi.sh gg       # artgallery_gg
/opt/artgallery/scripts/backup-multi.sh flynn    # artgallery_flynn
```

From a checkout, the same script runs directly:

```bash
cd docker-compose
./scripts/backup-multi.sh
```

### Multi-site restore

```bash
# Interactive — lists matching dumps in $BACKUP_DIR, newest first
/opt/artgallery/scripts/restore-multi.sh gg

# Explicit file
/opt/artgallery/scripts/restore-multi.sh flynn artgallery_artgallery_flynn_20261105_020000.dump
```

The restore script will:

1. Verify the `.sha256` checksum (prompts to continue if it is missing or mismatched)
2. Prompt for confirmation — you must type `RESTORE`
3. Stop that site's API container (`artgallery-api-gg` or `artgallery-api-flynn`)
4. Drop and recreate the site database
5. `pg_restore` the dump into it
6. Restart the API container

**Downtime per site:** ~10–30 seconds. The other site is unaffected.

### Verify a restore

Neither `docker-compose.multi.yml` nor the retired `docker-compose.prod.yml` publishes an
API host port — the `api-gg` / `api-flynn` services have no `ports:` key at all, so
`http://localhost:8080` does not exist on the host. Query from inside the container, or
go through NGINX:

```bash
# From inside the API container
docker exec artgallery-api-gg    curl -fsS http://localhost:8080/api/health/health
docker exec artgallery-api-flynn curl -fsS http://localhost:8080/api/health/health

# NGINX's own health endpoint (host 9090 -> container 8080)
curl http://localhost:9090/health

# End-to-end through NGINX (self-signed origin cert, so -k; Host selects the server block)
curl -k https://localhost/api/health/health -H "Host: ggpaintings.com"
curl -k https://localhost/api/health/health -H "Host: earthelementsart.com"
```

Port 80 serves nothing but a `301` to HTTPS. Without an explicit `Host:` header, SNI and
`Host` are both `localhost`, which matches no `server_name`, so NGINX falls through to the
first 443 block (gg) and you silently test the wrong site.

To confirm the data landed, not just the health check:

```bash
docker exec artgallery-postgres psql -U postgres -d artgallery_gg -c 'SELECT COUNT(*) FROM "Paintings";'
```

### Multi-site database architecture

| Site | Domain | API container | Database | Dump pattern |
|------|--------|---------------|----------|--------------|
| gg | ggpaintings.com | `artgallery-api-gg` | `artgallery_gg` | `artgallery_artgallery_gg_*.dump` |
| flynn | earthelementsart.com | `artgallery-api-flynn` | `artgallery_flynn` | `artgallery_artgallery_flynn_*.dump` |
| shared | — | `artgallery-postgres` | both databases | — |

---

## Local backup (Windows)

For a stack running locally out of `docker-compose/`. Dumps land in
`docker-compose/backups/`, which is gitignored.

```powershell
# Multi-site stack (artgallery-postgres)
.\docker-compose\scripts\create-local-backup-multi.ps1              # both sites
.\docker-compose\scripts\create-local-backup-multi.ps1 -Site gg
.\docker-compose\scripts\restore-local-backup-multi.ps1 -Site gg
.\docker-compose\scripts\restore-local-backup-multi.ps1 -Site flynn -BackupFile "artgallery_artgallery_flynn_20260101_120000.dump"
```

These talk to `artgallery-postgres` and the per-site API containers. The single-site
variants (`create-local-backup.ps1`, `restore-local-backup.ps1`) default to
`artgallery-postgres-prod` / `artgallery-api-prod` and the `artgallery` database.

---

## Cron schedule

Default for both installers: **every Sunday at 2:00 AM**.

```
0 2 * * 0 /opt/artgallery/scripts/backup-multi.sh
```

`install-backup-cron-multi.sh` rewrites the crontab and drops any existing line matching
`artgallery.*backup` before adding the new one, so an old single-site entry is replaced
rather than duplicated. Verify anyway:

```bash
crontab -l
```

### Change the schedule

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

## Retention

Both `backup.sh` and `backup-multi.sh` set `RETENTION_DAYS=120` near the top of the file,
then `source` `backup.config` before the cleanup step. **The config value wins** — a
`RETENTION_DAYS` line in `backup.config` overrides the hard-coded 120. The
`${RETENTION_DAYS:-30}` fallback only fires when the config omits the variable.

`install-backup-cron-multi.sh` writes `RETENTION_DAYS=30` into the generated config, so a
fresh install prunes at **30 days**, not 120. Raise it by editing
`/opt/artgallery/scripts/backup.config`:

```bash
RETENTION_DAYS=120
```

Pruning deletes `*.dump` and `*.sha256` files older than the window, on the whole
`backups/` directory.

## Monitoring

```bash
# Full log
cat /opt/artgallery/backups/backup.log

# Follow in real time
tail -f /opt/artgallery/backups/backup.log

# Last few runs
tail -5 /opt/artgallery/backups/backup.log

# Available dumps
ls -lh /opt/artgallery/backups/*.dump
```

### Verify cron is running

```bash
crontab -l
systemctl status cron
grep CRON /var/log/syslog | grep artgallery
```

---

## Troubleshooting

### Backup fails: "Container not running"

```bash
# Check container status
docker ps | grep artgallery

# Start the multi-site stack if it is down
cd docker-compose
docker compose -f docker-compose.multi.yml up -d
```

`backup-multi.sh` needs `artgallery-postgres` up. It does not care whether the API or
frontend containers are running.

### Backup fails: "Configuration file not found"

`backup.sh` (legacy) hard-exits when `/opt/artgallery/scripts/backup.config` is missing,
and also when `POSTGRES_PASSWORD` is unset in it. `backup-multi.sh` only warns and
continues. To create the config by hand:

```bash
cp scripts/backup.config.example /opt/artgallery/scripts/backup.config
chmod 600 /opt/artgallery/scripts/backup.config
# then set POSTGRES_PASSWORD to the same value as POSTGRES_PASSWORD in .env.multi
```

The password is not actually used by `pg_dump` — the dump runs over the container's local
socket, which PostgreSQL trusts — but `backup.sh` requires the variable to be present.

### Backup fails: "Permission denied"

```bash
sudo chmod +x /opt/artgallery/scripts/*.sh
sudo chmod 755 /opt/artgallery/backups
```

### Cron not running

```bash
sudo systemctl start cron
sudo systemctl enable cron
sudo grep CRON /var/log/syslog
```

### Restore skipped during deploy

Three things cause `deploy-multi.sh` to seed instead of restore:

1. `$BACKUP_DIR` does not exist (default `/opt/artgallery/backups`)
2. No file matches `artgallery_<database>_*.dump` — see
   [Filename glob matters](#filename-glob-matters)
3. The dump landed in `docker-compose/backups/` instead of `$BACKUP_DIR`

The script prints `No backup files found for site '<x>'` when it skips. A non-zero
painting count after deploy is the confirmation that the restore actually ran.

---

## Security notes

- Dumps contain sensitive data. A `pg_dump -Fc` archive of `artgallery_gg` or
  `artgallery_flynn` contains every `AdminUser` row — emails, Google `sub`, Yahoo `guid`.
- `docker-compose/backups/` is gitignored for exactly this reason (`.gitignore` ignores the
  whole directory, not just particular extensions). Do not add dumps back with `git add -f`.
- Keep the server directory locked down: `chmod 700 /opt/artgallery/backups` and
  `chmod 600 /opt/artgallery/scripts/backup.config`.
- SHA256 checksums are written next to each dump and verified on restore.
- Consider encrypting dumps before any offsite copy.

## Offsite backup (optional)

```bash
sudo apt install rclone
rclone config

# Add to backup-multi.sh before the cleanup step:
rclone copy /opt/artgallery/backups/ remote:artgallery-backups/ --checksum
```

## Disaster recovery checklist

1. [ ] SSH into the server
2. [ ] `docker ps --format '{{.Names}}'` — confirm which stack is live
3. [ ] `ls -lh /opt/artgallery/backups/*.dump` — list available dumps
4. [ ] Pick the newest dump whose name matches the site's glob
5. [ ] `/opt/artgallery/scripts/restore-multi.sh gg <file>` (and/or `flynn`)
6. [ ] Type `RESTORE` when prompted
7. [ ] Verify health: `curl http://localhost:9090/health`
8. [ ] Verify data: `docker exec artgallery-postgres psql -U postgres -d artgallery_gg -c 'SELECT COUNT(*) FROM "Paintings";'`
9. [ ] Test both sites through Cloudflare
10. [ ] Check logs: `tail -f /opt/artgallery/backups/restore.log`

---

## Legacy single-site scripts (retired)

> **Do not use these on a host running `docker-compose.multi.yml`.** They target the
> retired single-site stack: container `artgallery-postgres-prod`, a single `artgallery`
> database, and the `api` / `frontend` service names that do not exist in the multi-site
> stack. The two stacks cannot run side by side — both bind host ports 80/443/9090 and
> claim the `artgallery-nginx` container name.

```bash
# Installer for the single-site stack
sudo ./install-backup-cron.sh

# Manual backup -> /opt/artgallery/backups/artgallery_db_<timestamp>.dump
/opt/artgallery/scripts/backup.sh

# Restore
/opt/artgallery/scripts/restore.sh --list artgallery_db_20260518_020000.dump
/opt/artgallery/scripts/restore.sh artgallery_db_20260518_020000.dump
/opt/artgallery/scripts/restore.sh          # interactive
```

`restore.sh` stops the `artgallery-api-prod` container briefly, restores the `artgallery`
database, then restarts it. Expect ~10–30 seconds of downtime.

To bring such a host onto the multi-site stack, follow **Upgrading a Single-Site Server to
Multi-Site** in [`../DEPLOYMENT_GUIDE.md`](../DEPLOYMENT_GUIDE.md). Note that the dump
filename has to be renamed for the multi-site tooling to find it.
