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
- **Data models and types** should be in `/src/types/` with central exports from `/src/types/index.ts`
- **API service functions** should be in `/src/lib/api.ts`
- **Server Actions** should be in `/src/actions/` directory
- **Sanitization utilities** in `/src/lib/sanitization.ts` (server) and `/src/lib/client-sanitization.ts` (client)

### Server-Side Rendering & Caching
- **Prefer server components** over client components when possible for better performance
- **Use tag-based cache invalidation** for API calls with 24-hour fallback revalidation:
  ```typescript
  const res = await fetch(url, {
      cache: 'force-cache',
      next: {
          revalidate: 86400,  // 24-hour fallback
          tags: [CacheTags.paintings, CacheTags.allContent]
      }
  });
  ```
- **All API fetches use 24-hour cache duration** with tag-based invalidation
- **Cache tags are defined** in `/clientapp/src/lib/cache-tags.ts`
- **Cache invalidation** is handled by Server Actions in `/clientapp/src/actions/`:
  - `painting-actions.ts` - Uses `paintingMutationTags` (paintings, newPaintings, carousel, allContent) and `categoryAssignmentTags`
  - `category-actions.ts` - Uses `categoryMutationTags` (paintingCategories, allContent)
  - `page-content-actions.ts` - Uses `pageContentMutationTags` (pageContents, allContent)
- **Admin panel mutations MUST use Server Actions** (not direct API calls) to ensure cache is invalidated
- **Convert client components to server components** when they don't require interactivity
- **Use `await params`** in server components instead of `use(params)`

### API Integration
- **API base URL** uses TWO environment variables:
  - `NEXT_PUBLIC_API_URL` - Client-side URL (browser-accessible, required)
  - `SERVER_API_URL` - Server-side URL (Docker internal URL, required for server components)
- **No default/fallback URLs** - Both variables throw errors if not set
- **All data fetching** should go through `/src/lib/api.ts` service layer
- **Error handling** should be implemented for all API calls
- **TypeScript interfaces** for API responses should be defined in `/src/types/`

### Data Models
- Types are exported centrally from `@/types`:
  ```typescript
  import { Painting, PaintingCategory, PageContent, CarouselImage } from '@/types';
  ```

- **Painting** interface (matches `ServerApp.Application.DTOs.PaintingDto`):
  ```typescript
  interface Painting {
      id: string;
      slug: string;
      title: string;
      description?: string;
      imageUrl: string;
      thumbnailUrl?: string;
      categorySlug: string;
      width?: number;
      height?: number;
      depth?: number;
      year?: number;
      price?: number;
      isAvailable: boolean;
      isNew: boolean;
      isLandscape: boolean;
  }
  ```

- **PaintingCategory** interface (matches `ServerApp.Application.DTOs.PaintingCategoryDto`):
  ```typescript
  interface PaintingCategory {
      id: string;
      slug: string;
      name: string;
      description?: string;
  }
  ```

- **PaintingCategoryWithPaintings** extends PaintingCategory:
  ```typescript
  interface PaintingCategoryWithPaintings extends PaintingCategory {
      paintings: Painting[];
  }
  ```

- **PageContent** interface (matches `ServerApp.Application.DTOs.PageContentDto`):
  ```typescript
  interface PageContent {
      id: string;
      slug: string;
      title?: string;
      content: string;
      photoUrls?: string[];
  }
  ```

- **CarouselImage** interface:
  ```typescript
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
