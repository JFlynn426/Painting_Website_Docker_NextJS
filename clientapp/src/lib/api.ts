// API Configuration
// ============================================================================
// In Next.js, we need to handle API URLs differently for server vs client:
// - Server components run on the server and can reach Docker internal URLs
// - Client components run in the browser and need public URLs
// ============================================================================

// Client-side API URL (used by browser) - must be NEXT_PUBLIC_ to be exposed to client
const CLIENT_API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8080/api';

// Server-side API URL (used by Next.js server components running inside Docker)
// This is NOT prefixed with NEXT_PUBLIC_ so it's only available on the server
const SERVER_API_URL = process.env.SERVER_API_URL || 'http://api:8080/api';

/**
 * Get the appropriate API base URL based on execution context
 * This function must be called at runtime, not at module load time
 */
function getApiBaseUrl(): string {
    // Check if we're running on the server (window is undefined)
    const isServer = typeof window === 'undefined';

    if (isServer) {
        // Server-side: use the server API URL (Docker internal URL when in Docker)
        return SERVER_API_URL;
    } else {
        // Client-side: use the client API URL (public URL accessible from browser)
        return CLIENT_API_URL;
    }
}

// ============================================================================
// API Service Functions
// ============================================================================

import {
    Painting,
    PaintingCategory,
    PaintingCategoryWithPaintings,
    PageContent,
    CarouselImage
} from '@/types';

/**
 * Fetch all painting categories
 * Endpoint: GET api/paintingcategories
 */
export async function getAllPaintingCategories(): Promise<PaintingCategory[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintingcategories`, {
            next: { revalidate: 86400 } // Cache for 24 hours
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch painting categories: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Error fetching painting categories:', error);
        throw error;
    }
}

/**
 * Fetch a specific painting category with its paintings
 * Endpoint: GET api/paintingcategories/{slug}
 */
export async function getCategoryData(categorySlug: string): Promise<PaintingCategoryWithPaintings | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintingcategories/${categorySlug}`, {
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch category data: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`Error fetching category data for ${categorySlug}:`, error);
        throw error;
    }
}

/**
 * Fetch all paintings
 * Endpoint: GET api/paintings
 */
export async function getAllPaintings(): Promise<Painting[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings`, {
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch paintings: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Error fetching paintings:', error);
        throw error;
    }
}

/**
 * Fetch a specific painting by slug
 * Endpoint: GET api/paintings/{slug}
 */
export async function getPaintingBySlug(slug: string): Promise<Painting | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings/${slug}`, {
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch painting: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`Error fetching painting ${slug}:`, error);
        throw error;
    }
}

/**
 * Fetch paintings by category slug
 * Endpoint: GET api/paintings/category/{categorySlug}
 */
export async function getPaintingsByCategory(categorySlug: string): Promise<PaintingCategoryWithPaintings | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings/category/${categorySlug}`, {
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch paintings for category: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`Error fetching paintings for category ${categorySlug}:`, error);
        throw error;
    }
}

// Alias for backward compatibility
export { getPaintingBySlug as getPainting };

/**
 * Fetch carousel images
 * Endpoint: GET api/carousel
 */
export async function getCarouselImages(): Promise<CarouselImage[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/carousel`, {
            next: { revalidate: 7200 } // Cache for 2 hours
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch carousel images: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Error fetching carousel images:', error);
        throw error;
    }
}

/**
 * Fetch page content by slug
 * Endpoint: GET api/pagecontent/{slug}
 */
export async function getPageContent(slug: string): Promise<PageContent | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/pagecontent/${slug}`, {
            next: { revalidate: 86400 } // Cache for 24 hours
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch page content: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`Error fetching page content for ${slug}:`, error);
        throw error;
    }
}
