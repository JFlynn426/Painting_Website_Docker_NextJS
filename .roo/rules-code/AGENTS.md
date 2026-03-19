# Project Coding Rules (Non-Obvious Only)

- Client components must use 'use client' directive
- Images are stored in `/public/` directory and referenced with relative paths
- ArtCarousel component uses react-bootstrap Carousel with specific styling
- Painting categories are defined in `/src/app/models/paintingCategories.ts`
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
- **Data models and types** should be in `/src/types/` or `/src/app/models/`
- **API service functions** should be in `/src/lib/api.ts`

## Server-Side Rendering & Caching

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

## API Integration

- **API base URL** should be configured via environment variable: `NEXT_PUBLIC_API_URL`
- **All data fetching** should go through `/src/lib/api.ts` service layer
- **Error handling** should be implemented for all API calls
- **TypeScript interfaces** for API responses should be defined in `/src/types/paintings.ts`
- **Default API URL** for development: `http://localhost:5000/api`

## Data Models

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

## Refactoring Guidelines

- Follow the phased approach outlined in `clientapp/REFACTORING_ANALYSIS.md`
- **Phase 1**: Extract reusable components (low risk)
- **Phase 2**: Implement server-side rendering with caching (medium risk)
- **Phase 3**: Integrate external API (high risk)
- Always test changes thoroughly before merging

## C# Coding Style

- **Always use `using` statements** for type references instead of fully qualified class names
- **Correct**: `using ServerApp.Shared.Abstractions.Exceptions;` followed by `public class MyException : ServerAppException`
- **Incorrect**: `public class MyException : ServerApp.Shared.Abstractions.Exceptions.ServerAppException`
- **Also correct**: `using ServerApp.Shared.Abstractions.Domain;` followed by `public record MyValue : StringValueObject`
- **Incorrect**: `public record MyValue : ServerApp.Shared.Abstractions.Domain.StringValueObject`
- This improves code readability and maintainability