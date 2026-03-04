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

## Refactoring & API Migration

### Component Organization
- **Layout components** (NavBar, Footer) should be in `/src/components/` directory
- **Page-specific components** should be in their respective page directories
- **Shared UI components** (PaintingGrid, PaintingImage, ArtCarousel) should be in `/src/components/`
- **Data models and types** should be in `/src/types/` or `/src/app/models/`
- **API service functions** should be in `/src/lib/api.ts`

### Server-Side Rendering & Caching
- **Prefer server components** over client components when possible for better performance
- **Use Next.js 16 data fetching with caching** for API calls:
  ```typescript
  const res = await fetch(url, { next: { revalidate: 3600 } }); // Cache for 1 hour
  ```
- **Caching durations**:
  - Painting Categories: 24 hours (`revalidate: 86400`)
  - Painting Images: 1 hour (`revalidate: 3600`)
  - Carousel Images: 2 hours (`revalidate: 7200`)
- **Convert client components to server components** when they don't require interactivity
- **Use `await params`** in server components instead of `use(params)`

### API Integration
- **API base URL** should be configured via environment variable: `NEXT_PUBLIC_API_URL`
- **All data fetching** should go through `/src/lib/api.ts` service layer
- **Error handling** should be implemented for all API calls
- **TypeScript interfaces** for API responses should be defined in `/src/types/paintings.ts`
- **Default API URL** for development: `http://localhost:5000/api`

### Data Models
- Use the following interfaces for painting-related data:
  ```typescript
  interface Painting {
      id: string;
      title: string;
      description?: string;
      imageUrl: string;
      thumbnailUrl?: string;
      category: string;
      dimensions?: string;
      year?: number;
      price?: number;
      isAvailable?: boolean;
  }
  
  interface PaintingCategory {
      id: string;
      name: string;
      slug: string;
      description?: string;
      paintingCount?: number;
  }
  
  interface CarouselImage {
      id: string;
      imageUrl: string;
      alt: string;
      title?: string;
  }
  ```

### Refactoring Guidelines
- Follow the phased approach outlined in `clientapp/REFACTORING_ANALYSIS.md`
- **Phase 1**: Extract reusable components (low risk)
- **Phase 2**: Implement server-side rendering with caching (medium risk)
- **Phase 3**: Integrate external API (high risk)
- Always test changes thoroughly before merging