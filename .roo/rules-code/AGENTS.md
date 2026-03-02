# Project Coding Rules (Non-Obvious Only)

- Client components must use 'use client' directive
- Images are stored in `/public/` directory and referenced with relative paths
- ArtCarousel component uses react-bootstrap Carousel with specific styling
- Painting categories are defined in `/src/app/models/paintingCategories.ts`
- Uses Next.js Image component for optimized image loading with priority prop
- Component files use `.tsx` extension for TypeScript React components
- Uses CSS modules for styling (e.g., `page.module.css`)
- Uses React Compiler for performance optimization (enabled in next.config.ts)