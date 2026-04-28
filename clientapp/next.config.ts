import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  reactCompiler: true,
  images: {
    // Only 2 quality levels: 50 for thumbnails/carousel, 95 for detail views
    qualities: [50, 95],
    // Device sizes matching actual viewport breakpoints used in the grid
    // Mobile (<768px): full-width images, Tablet (768-1024px), Desktop (1024px+)
    deviceSizes: [640, 768, 1024],
    // Image sizes for srcset generation relative to the width prop
    // Thumbnails (width=400) generate: 400px variants
    // Carousel (width=1200) generates: 400, 800, 1200px variants
    // Detail views (width=1200) generate: 400, 800, 1200px variants
    // Modal (width=natural ~2400) generates: 400, 800, 1200, 2560px variants
    imageSizes: [400, 800, 1200, 2560],
    // Note: Not setting 'formats' option to avoid runtime WebP conversion overhead.
    // Next.js will serve original JPEG files directly without reprocessing.
  },
  // Standalone output for Docker deployments
  // Creates a minimal output directory with only necessary files
  output: 'standalone',
};

export default nextConfig;
