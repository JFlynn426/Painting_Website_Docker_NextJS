#!/usr/bin/env bash
# =============================================================================
# Single-site -> multi-site cutover preparation
# =============================================================================
# Run this ON THE SERVER, inside the docker-compose/ directory, before
# deploy-multi.sh.
#
#   ./scripts/prepare-cutover.sh --check     # read-only report (default)
#   ./scripts/prepare-cutover.sh --fix       # apply the config fixes
#   ./scripts/prepare-cutover.sh --backup    # dump gg DB + copy gg images
#
# --fix changes exactly these things, and prints every change it makes:
#   1. chmod +x on deploy-multi.sh and scripts/*.sh
#   2. Creates .env.multi from .env.multi.example if it is missing
#   3. Replaces placeholder Admin__JwtSecretKey values with random secrets
#   4. Repoints the four OAuth redirect URIs at the production domains
#   5. Adds the flynn admin email to FLYNN_ADMIN_AUTHORIZED_EMAILS
#   6. Sets BACKUP_DIR and writes secrets/postgres_password
#
# It never touches the database, never starts containers, and is safe to
# re-run. .env.multi is gitignored, so none of this can leak via git.
#
# IMPORTANT: --fix only updates this file. The same redirect URIs must also be
# registered in the Google Cloud Console and the Yahoo developer portal, or
# the provider rejects the authorization request with a redirect_uri_mismatch.
# =============================================================================

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
ENV_FILE="${COMPOSE_DIR}/.env.multi"
EXAMPLE_FILE="${COMPOSE_DIR}/.env.multi.example"
SECRET_FILE="${COMPOSE_DIR}/secrets/postgres_password"

GG_DOMAIN="ggpaintings.com"
FLYNN_DOMAIN="earthelementsart.com"
FLYNN_ADMIN_EMAIL="texasterrigray@yahoo.com"
BACKUP_DIR_DEFAULT="/opt/artgallery/backups"

cd "${COMPOSE_DIR}"

# --- Helpers ---------------------------------------------------------------

# Current value of a key in .env.multi, empty string if absent.
get_kv() {
    local key="$1"
    [ -f "$ENV_FILE" ] || { echo ""; return; }
    local line
    line="$(grep -m1 "^${key}=" "$ENV_FILE" || true)"
    echo "${line#${key}=}" | tr -d '"'
}

# Write KEY=VALUE, replacing an existing line or appending a new one.
set_kv() {
    local key="$1" value="$2"
    local current
    current="$(get_kv "$key")"
    if [ "$current" = "$value" ]; then
        echo "    = ${key} unchanged"
        return
    fi
    if grep -q "^${key}=" "$ENV_FILE"; then
        sed -i "s|^${key}=.*|${key}=${value}|" "$ENV_FILE"
    else
        printf '%s=%s\n' "$key" "$value" >> "$ENV_FILE"
    fi
    echo "    ~ ${key}: '${current}' -> '${value}'"
}

# True when a value is empty or an obvious placeholder.
is_placeholder() {
    local v="$1"
    [ -z "$v" ] && return 0
    case "$v" in
        CHANGE_THIS*|your-*|*CHANGE_ME*|*xxxx*|*placeholder*|*TODO*) return 0 ;;
    esac
    return 1
}

random_secret() { openssl rand -hex 32; }

# The password the existing data volume was initialised with. POSTGRES_PASSWORD
# is only honoured on an EMPTY data directory, so on a reused volume the new
# stack must reuse this exact value or both APIs fail authentication.
detect_current_pg_password() {
    local container=""
    for candidate in artgallery-postgres-prod artgallery-postgres; do
        if docker ps -a --format '{{.Names}}' 2>/dev/null | grep -qx "$candidate"; then
            container="$candidate"
            break
        fi
    done
    [ -n "$container" ] || { echo ""; return; }
    # `|| true` is required: under `set -o pipefail` a grep with no match exits 1, which
    # would otherwise abort the script when no postgres container exists at all.
    docker inspect "$container" --format '{{range .Config.Env}}{{println .}}{{end}}' 2>/dev/null \
        | { grep '^POSTGRES_PASSWORD=' | head -1 | cut -d= -f2- || true; }
}

# --- Report ----------------------------------------------------------------

do_check() {
    echo "=========================================="
    echo "Cutover readiness report (read-only)"
    echo "=========================================="

    echo
    echo "--- Docker ---"
    docker --version || echo "  docker CLI not found"
    if docker --version 2>/dev/null | grep -qE 'Docker version (1[89]|20\.10\.[0-2])\.'; then
        echo "  WARNING: Docker older than 20.10.3. nginx runs as UID 101 and must bind 443."
        echo "           If it fails with 'Permission denied', set net.ipv4.ip_unprivileged_port_start=80."
    else
        echo "  Port binding for non-root nginx should be fine."
    fi

    echo
    echo "--- Containers now running ---"
    docker ps --format '  {{.Names}}\t{{.Status}}' 2>/dev/null || echo "  (cannot query docker; run with sudo if needed)"

    echo
    echo "--- Volumes ---"
    docker volume ls --format '  {{.Name}}' 2>/dev/null | grep -E 'postgres_data|image_data' || echo "  none matching postgres_data/image_data"

    echo
    echo "--- Executable bit on deploy scripts ---"
    local nonexec=0
    for f in ./deploy-multi.sh ./scripts/*.sh; do
        [ -f "$f" ] || continue
        if [ -x "$f" ]; then
            echo "  ok   $f"
        else
            echo "  NOX  $f   <- ./deploy-multi.sh would fail with Permission denied"
            nonexec=$((nonexec + 1))
        fi
    done
    [ "$nonexec" -eq 0 ] || echo "  -> run: ./scripts/prepare-cutover.sh --fix"

    echo
    echo "--- .env.multi ---"
    if [ ! -f "$ENV_FILE" ]; then
        echo "  MISSING. deploy-multi.sh will refuse to run. Create it with --fix."
        return
    fi
    echo "  present"

    local v
    for key in GG_ADMIN_JWT_SECRET_KEY FLYNN_ADMIN_JWT_SECRET_KEY; do
        v="$(get_kv "$key")"
        if is_placeholder "$v"; then
            echo "  BAD  $key is a placeholder -> admin tokens forgeable by anyone"
        else
            echo "  ok   $key looks set"
        fi
    done

    for key in GG_GOOGLE_AUTH_REDIRECT_URI GG_YAHOO_AUTH_REDIRECT_URI \
               FLYNN_GOOGLE_AUTH_REDIRECT_URI FLYNN_YAHOO_AUTH_REDIRECT_URI; do
        v="$(get_kv "$key")"
        case "$v" in
            *localhost*|"") echo "  BAD  $key = '${v}' -> OAuth will not work in production" ;;
            *)              echo "  ok   $key = ${v}" ;;
        esac
    done

    v="$(get_kv FLYNN_ADMIN_AUTHORIZED_EMAILS)"
    case "$v" in
        *"$FLYNN_ADMIN_EMAIL"*) echo "  ok   FLYNN_ADMIN_AUTHORIZED_EMAILS includes ${FLYNN_ADMIN_EMAIL}" ;;
        *)                      echo "  BAD  FLYNN_ADMIN_AUTHORIZED_EMAILS = '${v}' (missing ${FLYNN_ADMIN_EMAIL})" ;;
    esac

    for key in FLYNN_GOOGLE_AUTH_CLIENT_ID FLYNN_YAHOO_AUTH_CLIENT_ID; do
        v="$(get_kv "$key")"
        if [ -z "$v" ]; then
            echo "  BAD  $key is empty -> no flynn admin login at all"
        else
            echo "  ok   $key present (length ${#v}) - confirm its redirect URI is registered for https://${FLYNN_DOMAIN}/admin/login"
        fi
    done

    echo
    echo "--- Postgres password continuity ---"
    local cur new
    cur="$(detect_current_pg_password)"
    new="$(get_kv POSTGRES_PASSWORD)"
    if [ -z "$cur" ]; then
        echo "  No existing postgres container found -> fresh volume, POSTGRES_PASSWORD will initialise it."
    elif [ -z "$new" ]; then
        echo "  BAD  POSTGRES_PASSWORD not set in .env.multi, existing volume uses a different one."
    elif [ "$cur" = "$new" ]; then
        echo "  ok   .env.multi matches the password the volume was created with"
    else
        echo "  BAD  mismatch: volume has '${cur:0:4}****', .env.multi has '${new:0:4}****'"
        echo "       Both API containers will fail with 'password authentication failed'."
    fi

    echo
    echo "--- Flynn image assets ---"
    if [ -d ./image-data/flynn/high-res ]; then
        echo "  image-data/flynn/high-res: $(find ./image-data/flynn/high-res -type f | wc -l) files (expect 15)"
    else
        echo "  MISSING image-data/flynn/high-res -> every flynn painting image will 404"
    fi

    echo
    echo "=========================================="
    echo "Fix everything above with:  ./scripts/prepare-cutover.sh --fix"
    echo "=========================================="
}

# --- Fix -------------------------------------------------------------------

do_fix() {
    echo "=========================================="
    echo "Applying cutover fixes"
    echo "=========================================="

    echo
    echo "[1/6] Executable bit on scripts"
    chmod +x ./deploy-multi.sh ./scripts/*.sh 2>/dev/null || true
    echo "    done"

    echo
    echo "[2/6] .env.multi"
    if [ ! -f "$ENV_FILE" ]; then
        if [ -f "$EXAMPLE_FILE" ]; then
            cp "$EXAMPLE_FILE" "$ENV_FILE"
            echo "    + created .env.multi from .env.multi.example"
        else
            echo "    ERROR: neither .env.multi nor .env.multi.example exists."
            exit 1
        fi
    else
        cp "$ENV_FILE" "${ENV_FILE}.bak.$(date +%Y%m%d_%H%M%S)"
        echo "    backed up existing .env.multi"
    fi

    echo
    echo "[3/6] JWT secrets"
    for key in GG_ADMIN_JWT_SECRET_KEY FLYNN_ADMIN_JWT_SECRET_KEY; do
        if is_placeholder "$(get_kv "$key")"; then
            set_kv "$key" "$(random_secret)"
        fi
    done

    echo
    echo "[4/6] OAuth redirect URIs -> production domains"
    set_kv GG_GOOGLE_AUTH_REDIRECT_URI     "https://${GG_DOMAIN}/admin/login"
    set_kv GG_YAHOO_AUTH_REDIRECT_URI      "https://${GG_DOMAIN}/admin/login"
    set_kv FLYNN_GOOGLE_AUTH_REDIRECT_URI  "https://${FLYNN_DOMAIN}/admin/login"
    set_kv FLYNN_YAHOO_AUTH_REDIRECT_URI   "https://${FLYNN_DOMAIN}/admin/login"

    echo
    echo "[5/6] Admin allowlist and CORS"
    local emails
    emails="$(get_kv FLYNN_ADMIN_AUTHORIZED_EMAILS)"
    case "$emails" in
        *"$FLYNN_ADMIN_EMAIL"*) echo "    = flynn allowlist already includes ${FLYNN_ADMIN_EMAIL}" ;;
        "") set_kv FLYNN_ADMIN_AUTHORIZED_EMAILS "$FLYNN_ADMIN_EMAIL" ;;
        *)  set_kv FLYNN_ADMIN_AUTHORIZED_EMAILS "${emails},${FLYNN_ADMIN_EMAIL}" ;;
    esac
    set_kv GG_CORS_ALLOWED_ORIGINS    "https://${GG_DOMAIN},https://www.${GG_DOMAIN}"
    set_kv FLYNN_CORS_ALLOWED_ORIGINS "https://${FLYNN_DOMAIN},https://www.${FLYNN_DOMAIN}"
    set_kv BACKUP_DIR "$BACKUP_DIR_DEFAULT"

    echo
    echo "[6/6] Postgres password continuity"
    local cur
    cur="$(detect_current_pg_password)"
    if [ -n "$cur" ]; then
        set_kv POSTGRES_PASSWORD "$cur"
        mkdir -p "$(dirname "$SECRET_FILE")"
        printf '%s' "$cur" > "$SECRET_FILE"
        chmod 600 "$SECRET_FILE"
        echo "    wrote secrets/postgres_password (mode 600)"
    else
        echo "    No existing postgres container detected."
        echo "    If this is a FRESH volume, set POSTGRES_PASSWORD to a new strong value here;"
        echo "    scripts/init-databases.sql will then create both site databases for you."
    fi

    echo
    echo "Done. Re-check with:  ./scripts/prepare-cutover.sh --check"
    echo
    echo "STILL ON YOU (cannot be scripted):"
    echo "  - Register https://${GG_DOMAIN}/admin/login and https://${FLYNN_DOMAIN}/admin/login"
    echo "    as redirect URIs in Google Cloud Console AND the Yahoo developer portal."
    echo "    The existing keys were issued for localhost and will be rejected until you do."
    echo "  - Cloudflare: SSL mode Full (not Full-strict), flynn DNS records proxied."
    echo "  - Generate origin certs: ./scripts/generate-prod-ssl.sh"
}

# --- Backup ----------------------------------------------------------------

do_backup() {
    local stamp backup_dir container
    stamp="$(date +%Y%m%d_%H%M%S)"
    backup_dir="$(get_kv BACKUP_DIR)"
    [ -n "$backup_dir" ] || backup_dir="$BACKUP_DIR_DEFAULT"

    echo "=========================================="
    echo "Backing up gg data (no downtime)"
    echo "=========================================="

    if ! docker ps --format '{{.Names}}' | grep -qx 'artgallery-postgres-prod'; then
        echo "ERROR: artgallery-postgres-prod is not running."
        echo "If the old stack is already down, restore from your last known-good dump instead."
        exit 1
    fi

    sudo mkdir -p "$backup_dir"

    echo
    echo "[1/2] Database dump"
    # Name matters: deploy-multi.sh only auto-restores files matching
    # artgallery_<database>_<timestamp>.dump.
    docker exec artgallery-postgres-prod pg_dump -U postgres -Fc -f /tmp/gg.dump artgallery
    docker exec artgallery-postgres-prod pg_restore --list /tmp/gg.dump | head -5 \
        || { echo "ERROR: dump has no readable TOC, aborting."; exit 1; }

    local target="${backup_dir}/artgallery_artgallery_gg_${stamp}.dump"
    sudo docker cp artgallery-postgres-prod:/tmp/gg.dump "$target"
    ( cd "$backup_dir" && sudo sha256sum "$(basename "$target")" > "$(basename "$target").sha256" )
    docker exec artgallery-postgres-prod rm -f /tmp/gg.dump
    echo "    wrote $target"
    echo "    $(sudo du -h "$target" | cut -f1)"

    echo
    echo "[2/2] Painting images from the old named volume"
    local vol
    vol="$(docker volume ls --format '{{.Name}}' | grep -E 'image_data$' | head -1 || true)"
    if [ -z "$vol" ]; then
        echo "    No image_data volume found - skipping (repo already ships image-data/gg)."
    else
        echo "    copying from volume $vol into ./image-data/gg"
        docker run --rm -v "${vol}:/from:ro" -v "${COMPOSE_DIR}/image-data:/to" alpine \
            sh -c 'mkdir -p /to/gg && cp -au /from/. /to/gg/ && echo "    files now in image-data/gg: $(find /to/gg -type f | wc -l)"'
    fi

    echo
    echo "Backup complete. Next: ./scripts/prepare-cutover.sh --check, then ./deploy-multi.sh"
}

# --- Entry -----------------------------------------------------------------

case "${1:---check}" in
    --check)  do_check ;;
    --fix)    do_fix ;;
    --backup) do_backup ;;
    *)
        echo "Usage: $0 [--check|--fix|--backup]"
        exit 1
        ;;
esac