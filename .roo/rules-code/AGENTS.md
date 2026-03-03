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