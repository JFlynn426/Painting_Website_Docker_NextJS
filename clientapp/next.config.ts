import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  reactCompiler: true,
  images: {
    qualities: [60, 75, 95],
  },
  // Standalone output for Docker deployments
  // Creates a minimal output directory with only necessary files
  output: 'standalone',
};

export default nextConfig;
