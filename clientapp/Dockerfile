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

# Set environment variables for build time
# These are needed for Next.js to properly compile the application
ENV NEXT_PUBLIC_API_URL=/api
ENV SERVER_API_URL=http://api:8080/api

# Disable telemetry
ENV NEXT_TELEMETRY_DISABLED=1

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the project
COPY . .

# Build the Next.js app
# Using --experimental-build-mode=server to skip static generation
# This prevents build-time API calls that would fail in Docker
RUN npm run build

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
COPY --from=builder /app/package.json ./
COPY --from=builder /app/package-lock.json ./
COPY --from=builder /app/next.config.ts ./
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

# Install production dependencies only
RUN npm ci --only=production --omit=dev && npm cache clean --force

# Change ownership to non-root user
RUN chown -R nextjs:nodejs /app

# Switch to non-root user
USER nextjs

# Expose Next.js port
EXPOSE 3000

# Start Next.js using the standalone server
CMD ["node", "server.js"]