# SSL Certificate Directory

This directory contains SSL certificates for NGINX when using Cloudflare Full mode.

## Generating Self-Signed Certificate

Run this command on your Linux server to generate a self-signed certificate:

```bash
cd docker-compose/nginx/ssl

openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout server.key \
  -out server.crt \
  -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=yourdomain.com"
```

Replace `yourdomain.com` with your actual domain name.

## Setting Permissions

After generating the certificates, set proper permissions:

```bash
chmod 600 server.key
chmod 644 server.crt
```

## Important Notes

1. **Self-signed certificates are only for Cloudflare Full mode** - Cloudflare will accept self-signed certificates, but browsers will not. Since Cloudflare terminates SSL at the edge, users will never see the self-signed certificate warning.

2. **Do NOT commit server.key to git** - The private key is already excluded by `.gitignore`. Never share your private key.

3. **Certificate renewal** - Self-signed certificates don't expire in a way that breaks functionality, but you should regenerate them annually:
   ```bash
   openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
     -keyout server.key \
     -out server.crt \
     -subj "/C=US/ST=YourState/L=YourCity/O=YourOrganization/CN=yourdomain.com"
   docker-compose -f docker-compose.cloudflare.yml restart nginx
   ```

## Cloudflare SSL Mode Explanation

| Mode | Client → Cloudflare | Cloudflare → Your Server |
|------|---------------------|--------------------------|
| Flexible | HTTPS | HTTP (no cert needed) |
| **Full** | HTTPS | HTTPS (self-signed OK) ← **We use this** |
| Full (Strict) | HTTPS | HTTPS (valid cert required) |

In **Full mode**, Cloudflare connects to your server via HTTPS and accepts self-signed certificates. This provides an extra layer of encryption between Cloudflare and your server.

## Troubleshooting

If NGINX fails to start due to SSL issues:

```bash
# Check certificate files exist
ls -la docker-compose/nginx/ssl/

# Check certificate validity
openssl x509 -in server.crt -text -noout

# Check NGINX configuration
docker-compose -f docker-compose.cloudflare.yml logs nginx