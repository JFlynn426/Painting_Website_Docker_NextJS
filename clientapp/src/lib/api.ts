// API Configuration
// ============================================================================
// In Next.js, we need to handle API URLs differently for server vs client:
// - Server components run on the server and can reach Docker internal URLs
// - Client components run in the browser and need public URLs
// ============================================================================

// Client-side API URL (used by browser) - must be NEXT_PUBLIC_ to be exposed to client
// Environment variable is required - no fallback to ensure proper configuration
const CLIENT_API_URL = process.env.NEXT_PUBLIC_API_URL!;
if (!CLIENT_API_URL) {
    throw new Error('NEXT_PUBLIC_API_URL environment variable is not set. Please check your .env file.');
}

// Server-side API URL (used by Next.js server components running inside Docker)
// This is NOT prefixed with NEXT_PUBLIC_ so it's only available on the server
// We do NOT validate this at module load time because this module may be imported by client components
// Validation happens only when server-side functions are called
const SERVER_API_URL = process.env.SERVER_API_URL;

/**
 * Get the server-side API base URL with validation
 * This function should only be called from server components or server actions
 * @throws Error if SERVER_API_URL is not set (should only happen on server)
 */
function getServerApiUrl(): string {
    if (!SERVER_API_URL) {
        throw new Error('SERVER_API_URL environment variable is not set. Please check your .env file.');
    }
    return SERVER_API_URL;
}

/**
 * Get the appropriate API base URL based on execution context
 * This function must be called at runtime, not at module load time
 */
function getApiBaseUrl(): string {
    // Check if we're running on the server (window is undefined)
    const isServer = typeof window === 'undefined';

    if (isServer) {
        // Server-side: use the server API URL (Docker internal URL when in Docker)
        return getServerApiUrl();
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
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getAllPaintingCategories(): Promise<PaintingCategory[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintingcategories`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 86400 } // Cache for 24 hours
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch painting categories: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Fetch a specific painting category with its paintings
 * Endpoint: GET api/paintingcategories/{slug}
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getCategoryData(categorySlug: string): Promise<PaintingCategoryWithPaintings | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintingcategories/${categorySlug}`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch category data: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Fetch all paintings
 * Endpoint: GET api/paintings
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getAllPaintings(): Promise<Painting[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch paintings: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Fetch a specific painting by slug
 * Endpoint: GET api/paintings/{slug}
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getPaintingBySlug(slug: string): Promise<Painting | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings/${slug}`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch painting: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Fetch paintings by category slug
 * Endpoint: GET api/paintings/category/{categorySlug}
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getPaintingsByCategory(categorySlug: string): Promise<PaintingCategoryWithPaintings | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings/category/${categorySlug}`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch paintings for category: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

// Alias for backward compatibility
export { getPaintingBySlug as getPainting };

/**
 * Fetch all new paintings (where IsNew=true)
 * Endpoint: GET api/paintings/new
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getNewPaintings(): Promise<Painting[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings/new`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 3600 } // Cache for 1 hour
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch new paintings: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Fetch carousel images
 * Endpoint: GET api/carousel
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getCarouselImages(): Promise<CarouselImage[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/carousel`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 7200 } // Cache for 2 hours
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch carousel images: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Fetch page content by slug
 * Endpoint: GET api/pagecontent/{slug}
 * Uses runtime fetching with caching to avoid build-time API calls
 */
export async function getPageContent(slug: string): Promise<PageContent | null> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/pagecontent/${slug}`, {
            cache: 'force-cache', // Runtime fetching with caching
            next: { revalidate: 86400 } // Cache for 24 hours
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch page content: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}
