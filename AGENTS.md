# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Build/Lint/Test Commands
- Build: `npm run build` (in clientapp directory)
- Run dev server: `npm run dev` (in clientapp directory)
- Lint: `npm run lint` (in clientapp directory)
- Test commands: Not specified in package.json, but project uses Next.js with React

## Code Style Guidelines
- Uses TypeScript with React and Next.js
- Follows Next.js App Router conventions (e.g., `/src/app` directory structure)
- Uses Bootstrap for styling with React-Bootstrap components
- Uses React Compiler for performance optimization (enabled in next.config.ts)
- Component files use `.tsx` extension for TypeScript React components
- Uses CSS modules for styling (e.g., `page.module.css`)
- Image optimization through Next.js Image component

## Project-Specific Patterns
- Client components must use 'use client' directive
- Images are stored in `/public/` directory and referenced with relative paths
- ArtCarousel component uses react-bootstrap Carousel with specific styling
- Painting categories are defined in `/src/app/models/paintingCategories.ts`
- Uses Next.js Image component for optimized image loading with priority prop

## Architecture
- Full-stack application with Next.js frontend and .NET backend
- Frontend in `clientapp/` directory with Next.js App Router structure
- Backend in `ServerApp/` directory using .NET 8
- Docker configuration in `docker-compose/` directory
- Uses React with TypeScript and Bootstrap for UI components

## Key Directories
- `/clientapp/` - Next.js frontend application
- `/ServerApp/` - .NET backend API
- `/docker-compose/` - Docker configuration files
- `/public/` - Static assets and images
- `/src/app/` - Next.js App Router pages and components
- `/src/components/` - Shared React components