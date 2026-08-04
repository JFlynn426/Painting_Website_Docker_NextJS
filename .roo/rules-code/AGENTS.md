# Project Coding Rules (Non-Obvious Only)

- Client components must use 'use client' directive
- Images are stored in `/public/` directory and referenced with relative paths
- ArtCarousel component uses react-bootstrap Carousel with specific styling
- Uses Next.js Image component for optimized image loading with priority prop
- Component files use `.tsx` extension for TypeScript React components
- Uses CSS modules for styling (e.g., `page.module.css`)
- Uses React Compiler for performance optimization (enabled in next.config.ts)
- Uses CSS custom properties (variables) defined in `globals.css`:
  - `--title-color: #66b3ff` - Light blue color for titles (navbar, footer, category pages)
  - `--background: #3d3d3d` - Background color
  - `--foreground: #ffffff` - Foreground/text color
  - `--navbar-footer-bg: #2d2d2d` - Navbar and footer background color

## Next.js 16 Route Handler Params

- **CRITICAL**: In Next.js 16, `params` in route handlers is a `Promise` and must be unwrapped before accessing its properties
- For **client components** (with 'use client'): Import `use` from `react` and use `const { param } = use(params)` to unwrap
- For **server components** (async): Use `const { param } = await params` to unwrap
- The `params` type should be declared as `Promise<{ paramName: string }>` not `{ paramName: string }`
- Example for client component:
  ```typescript
  "use client";
  import { use } from "react";
  
  interface PageProps {
      params: Promise<{ category: string }>;
  }
  
  export default function Page({ params }: PageProps) {
      const { category } = use(params);
      // use category...
  }
  ```
- Example for server component:
  ```typescript
  interface PageProps {
      params: Promise<{ category: string }>;
  }
  
  export default async function Page({ params }: PageProps) {
      const { category } = await params;
      // use category...
  }

## Component Organization

- **Layout components** (NavBar, Footer) should be in `/src/components/` directory
- **Page-specific components** should be in their respective page directories
- **Shared UI components** (PaintingGrid, PaintingImage, ArtCarousel) should be in `/src/components/`
- **Data models and types** should be in `/src/types/` with central exports from `/src/types/index.ts`
- **API service functions** should be in `/src/lib/api.ts`
- **Server Actions** should be in `/src/actions/` directory
- **Sanitization utilities** in `/src/lib/sanitization.ts` (server) and `/src/lib/client-sanitization.ts` (client)

## Server-Side Rendering & Caching

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

## API Integration

- **API base URL** uses TWO environment variables:
  - `NEXT_PUBLIC_API_URL` - Client-side URL (browser-accessible, required)
  - `SERVER_API_URL` - Server-side URL (Docker internal URL, required for server components)
- **No default/fallback URLs** - Both variables throw errors if not set
- **All data fetching** should go through `/src/lib/api.ts` service layer
- **Error handling** should be implemented for all API calls
- **TypeScript interfaces** for API responses should be defined in `/src/types/`

## Data Models

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

## Refactoring Guidelines

- Follow the phased approach outlined in `clientapp/REFACTORING_ANALYSIS.md`
- **Phase 1**: Extract reusable components (low risk)
- **Phase 2**: Implement server-side rendering with caching (medium risk)
- **Phase 3**: Integrate external API (high risk)
- Always test changes thoroughly before merging

## C# Coding Style

- **Always use `using` statements** for type references instead of fully qualified class names
- **Correct**: `using ServerApp.Shared.Exceptions;` followed by `public class MyException : ServerAppException`
- **Incorrect**: `public class MyException : ServerApp.Shared.Exceptions.ServerAppException`
- **Also correct**: `using ServerApp.Shared.Domain;` followed by `public record MyValue : StringValueObject`
- **Incorrect**: `public record MyValue : ServerApp.Shared.Domain.StringValueObject`
- This improves code readability and maintainability
- **Namespace structure**: `ServerApp.Shared.{Domain,Exceptions}` (NOT `ServerApp.Shared.Abstractions.{Domain,Exceptions}`)

## Environment File Synchronization

- **CRITICAL**: When modifying CSS theme variables or any site-specific configuration in `.env` files, ALL `.env` files must be updated simultaneously:
  - `docker-compose/.env.multi`
  - `docker-compose/.env.multi.example`
  - `docker-compose/.env.multi.arm64.example`
- This ensures consistency across local development, production, and ARM64 deployments
- Variables to keep in sync include: `FLYNN_CSS_BACKGROUND`, `FLYNN_CSS_FOREGROUND`, `FLYNN_CSS_NAVBAR_FOOTER_BG`, `FLYNN_CSS_TITLE_COLOR`, `FLYNN_CSS_BUTTON_COLOR`, `FLYNN_CSS_FONT`, `FLYNN_CSS_LINK_HOVER`, and all corresponding `GG_` prefixed variables
