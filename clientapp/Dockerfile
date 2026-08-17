# --- Development Stage ---
FROM node:25-alpine AS development
WORKDIR /app

# Update Alpine packages to fix CVEs
RUN apk update && apk upgrade --no-cache

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the project
COPY . .

# Expose Next.js port
EXPOSE 3000

# Set environment variables for development
ENV HOST=0.0.0.0
ENV PORT=3000

# Development mode with hot reload
CMD ["npm", "run", "dev"]

# --- Build Stage ---
FROM node:25-alpine AS builder
WORKDIR /app

# Update Alpine packages to fix CVEs
RUN apk update && apk upgrade --no-cache

# Build arguments for site configuration
# These allow multi-site deployments with different branding per site
# Defaults match current hardcoded values for backward compatibility
ARG NEXT_PUBLIC_SITE_NAME="Gloria Gronowicz Fine Art"
ARG NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL="Painting"
ARG NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL="Paintings"
ARG NEXT_PUBLIC_SITE_DESCRIPTION="Gloria Gronowicz is an oil painter who creates works inspired by nature"
ARG NEXT_PUBLIC_CONTACT_EMAIL=""
ARG NEXT_PUBLIC_CONTACT_PHONE=""
ARG NEXT_PUBLIC_CSS_BACKGROUND="#3d3d3d"
ARG NEXT_PUBLIC_CSS_FOREGROUND="#ffffff"
ARG NEXT_PUBLIC_CSS_NAVBAR_FOOTER_BG="#2d2d2d"
ARG NEXT_PUBLIC_CSS_TITLE_COLOR="#66b3ff"
ARG NEXT_PUBLIC_CSS_BUTTON_COLOR="#1e3a8a"
ARG NEXT_PUBLIC_CSS_LINK_HOVER="#60a5fa"
ARG NEXT_PUBLIC_CSS_MENU_HOVER="#1e3a8a"
ARG NEXT_PUBLIC_CSS_ADMIN_HOVER="#374151"
ARG NEXT_PUBLIC_CSS_FONT="Manjari-Thin.ttf"
ARG NEXT_PUBLIC_FAVICON="/favicon.ico"

# Set environment variables for build time
# These are needed for Next.js to properly compile the application
ENV NEXT_PUBLIC_API_URL=/api
ENV SERVER_API_URL=http://api:8080/api

# Site configuration environment variables (from build args)
ENV NEXT_PUBLIC_SITE_NAME=$NEXT_PUBLIC_SITE_NAME
ENV NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL=$NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL
ENV NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL=$NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL
ENV NEXT_PUBLIC_SITE_DESCRIPTION=$NEXT_PUBLIC_SITE_DESCRIPTION
ENV NEXT_PUBLIC_CONTACT_EMAIL=$NEXT_PUBLIC_CONTACT_EMAIL
ENV NEXT_PUBLIC_CONTACT_PHONE=$NEXT_PUBLIC_CONTACT_PHONE
ENV NEXT_PUBLIC_CSS_BACKGROUND=$NEXT_PUBLIC_CSS_BACKGROUND
ENV NEXT_PUBLIC_CSS_FOREGROUND=$NEXT_PUBLIC_CSS_FOREGROUND
ENV NEXT_PUBLIC_CSS_NAVBAR_FOOTER_BG=$NEXT_PUBLIC_CSS_NAVBAR_FOOTER_BG
ENV NEXT_PUBLIC_CSS_TITLE_COLOR=$NEXT_PUBLIC_CSS_TITLE_COLOR
ENV NEXT_PUBLIC_CSS_BUTTON_COLOR=$NEXT_PUBLIC_CSS_BUTTON_COLOR
ENV NEXT_PUBLIC_CSS_LINK_HOVER=$NEXT_PUBLIC_CSS_LINK_HOVER
ENV NEXT_PUBLIC_CSS_MENU_HOVER=$NEXT_PUBLIC_CSS_MENU_HOVER
ENV NEXT_PUBLIC_CSS_ADMIN_HOVER=$NEXT_PUBLIC_CSS_ADMIN_HOVER
ENV NEXT_PUBLIC_CSS_FONT=$NEXT_PUBLIC_CSS_FONT
ENV NEXT_PUBLIC_FAVICON=$NEXT_PUBLIC_FAVICON

# Disable telemetry
ENV NEXT_TELEMETRY_DISABLED=1

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the project
COPY . .

# Build the Next.js app
# Clean .next to avoid stale Turbopack artifacts, then build without Turbopack
RUN rm -rf .next && NEXT_TURBOPACK=false npm run build

# --- Production Stage ---
FROM node:25-alpine AS production
WORKDIR /app

ENV NODE_ENV=production

# Default environment variables for API URLs
# These can be overridden by docker-compose or runtime environment variables
ENV NEXT_PUBLIC_API_URL=/api
ENV SERVER_API_URL=http://api:8080/api

# Update Alpine packages to fix CVEs
RUN apk update && apk upgrade --no-cache

# Create non-root user for security
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nextjs -u 1001

# Copy standalone output from builder stage
# The 'output: standalone' in next.config.ts creates a minimal output
COPY --from=builder /app/next.config.ts ./
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

# Copy full node_modules from builder because Turbopack standalone output
# requires all dependencies (not just production) to resolve hashed module names
COPY --from=builder /app/node_modules ./node_modules

# Change ownership to non-root user
RUN chown -R nextjs:nodejs /app

# Switch to non-root user
USER nextjs

# Expose Next.js port
EXPOSE 3000

# Start Next.js using the standalone server
CMD ["node", "server.js"]