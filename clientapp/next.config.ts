import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  reactCompiler: true,
  images: {
    qualities: [40, 60, 75, 95],
    formats: ['image/avif', 'image/webp'],
  },
  // Standalone output for Docker deployments
  // Creates a minimal output directory with only necessary files
  output: 'standalone',
};

export default nextConfig;
