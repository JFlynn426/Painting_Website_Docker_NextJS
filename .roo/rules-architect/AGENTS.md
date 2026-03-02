# Project Architecture Rules (Non-Obvious Only)

- Full-stack application with Next.js frontend and .NET backend
- Frontend in `clientapp/` directory with Next.js App Router structure
- Backend in `ServerApp/` directory using .NET 8
- Docker configuration in `docker-compose/` directory
- Uses React with TypeScript and Bootstrap for UI components
- Client components must use 'use client' directive
- Images are stored in `/public/` directory and referenced with relative paths
- ArtCarousel component uses react-bootstrap Carousel with specific styling
- Painting categories are defined in `/src/app/models/paintingCategories.ts`
- Uses Next.js Image component for optimized image loading with priority prop