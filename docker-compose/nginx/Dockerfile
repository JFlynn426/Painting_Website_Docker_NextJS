# ============================================================================
# Custom NGINX Dockerfile for Non-Root User with Read-Only Support
# ============================================================================
# This Dockerfile creates an NGINX container that runs as non-root user (UID 101)
# with proper permissions on all required directories for read-only mode.
# ============================================================================

FROM nginx:alpine

# Install su-exec for switching to non-root user
RUN apk add --no-cache su-exec

# Set proper permissions for nginx user (UID 101) on required directories
# These directories need to be writable by the nginx user
RUN chown -R 101:101 /var/cache/nginx && \
    chown -R 101:101 /var/log/nginx && \
    chown -R 101:101 /run && \
    chown -R 101:101 /etc/nginx/conf.d && \
    chown -R 101:101 /usr/share/nginx/html && \
    mkdir -p /run/nginx && \
    chown -R 101:101 /run/nginx

# Create the nginx user if it doesn't exist (it should in alpine image)
# The nginx user in alpine has UID 101
RUN adduser -D -u 101 nginx || true

# Create entrypoint script that sets up tmpfs permissions before starting nginx
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Keep as root for entrypoint to fix tmpfs permissions, then switch to non-root
# USER 101 removed - entrypoint will switch to non-root after fixing permissions

# Expose ports
EXPOSE 80
EXPOSE 443
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget -q --spider http://localhost:8080/health || exit 1

# Use custom entrypoint
ENTRYPOINT ["/entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]