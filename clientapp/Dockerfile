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

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the project
COPY . .

# Build the Next.js app
RUN npm run build

# --- Production Stage ---
FROM node:25-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production

# Update Alpine packages to fix CVEs
RUN apk update && apk upgrade --no-cache

# Create non-root user for security
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nextjs -u 1001

# Copy built output from builder stage
COPY --from=builder /app/package.json ./
COPY --from=builder /app/package-lock.json ./
COPY --from=builder /app/next.config.ts ./
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next ./.next

# Install production dependencies only
RUN npm ci --only=production && npm cache clean --force

# Change ownership to non-root user
RUN chown -R nextjs:nodejs /app

# Switch to non-root user
USER nextjs

# Expose Next.js port
EXPOSE 3000

# Start Next.js
CMD ["npm", "start"]