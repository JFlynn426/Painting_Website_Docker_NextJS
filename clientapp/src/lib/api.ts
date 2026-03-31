// API Configuration
// @ts-expect-error - process is a Node.js global provided by Next.js
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

// Log the API base URL for debugging
console.log('=== [API Config] ===');
console.log('API_BASE_URL:', API_BASE_URL);
// @ts-expect-error - process is a Node.js global provided by Next.js
console.log('NEXT_PUBLIC_API_URL env var:', process.env.NEXT_PUBLIC_API_URL);
console.log('Is running in browser:', typeof window !== 'undefined');
console.log('===================');

// ============================================================================
// Data Source Configuration
// Set to true to use dummy data from models, false to use API calls
// ============================================================================
export const USE_DUMMY_DATA = false;

// ============================================================================
// Data Interfaces - Match ServerApp DTOs
// ============================================================================

export interface Painting {
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
}

export interface PaintingCategory {
    id: string;
    name: string;
    slug: string;
    description?: string;
}

export interface PageContentDto {
    address: string;
    title: string;
    content: string;
}

export interface PaintingCategoryWithPaintings {
    id: string;
    name: string;
    slug: string;
    description?: string;
    paintings: Painting[];
}

// ============================================================================
// Dummy Data Imports (used when USE_DUMMY_DATA is true)
// ============================================================================

import { paintingCategories as dummyCategories } from '../app/models/paintingCategories';
import { paintingsData as dummyPaintings, getPaintingById as getDummyPaintingById, getPaintingsByCategory as getDummyPaintingsByCategory } from '../app/models/paintings';
import { pageContentData as dummyPageContent } from '../app/models/pageContent';

// ============================================================================
// API Service Functions - Match ServerApp Controllers
// ============================================================================

/**
 * Fetch all painting categories
 * Server endpoint: GET /api/PaintingCategories
 * Uses dummy data if USE_DUMMY_DATA is true, otherwise calls API
 * Cache duration: 24 hours (categories rarely change)
 */
export async function getPaintingCategories(): Promise<PaintingCategory[]> {
    if (USE_DUMMY_DATA) {
        console.log('[getPaintingCategories] Using dummy data');
        return dummyCategories;
    }

    const url = `${API_BASE_URL}/paintingcategories`;
    console.log('[getPaintingCategories] Fetching from URL:', url);

    const res = await fetch(url, {
        next: { revalidate: 86400 } // Cache for 24 hours
    } as RequestInit & { next: { revalidate: number } });

    console.log('[getPaintingCategories] Response status:', res.status, res.statusText);

    if (!res.ok) {
        const errorText = await res.text();
        console.error('[getPaintingCategories] Error response body:', errorText);
        throw new Error('Failed to fetch painting categories (Status: ' + res.status + ')');
    }

    const data = await res.json();
    console.log('[getPaintingCategories] Successfully fetched', data.length, 'categories');
    return data;
}

/**
 * Fetch paintings by category slug
 * Server endpoint: GET /api/Paintings/category/{categorySlug}
 * Returns PaintingCategoryWithPaintingsDto, extracts paintings array
 * Uses dummy data if USE_DUMMY_DATA is true, otherwise calls API
 * Cache duration: 1 hour (paintings may change daily)
 */
export async function getPaintingsByCategory(categorySlug: string): Promise<Painting[]> {
    if (USE_DUMMY_DATA) {
        return getDummyPaintingsByCategory(categorySlug);
    }

    const res = await fetch(`${API_BASE_URL}/paintings/category/${categorySlug}`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    } as RequestInit & { next: { revalidate: number } });

    if (!res.ok) {
        throw new Error(`Failed to fetch paintings for category: ${categorySlug}`);
    }

    const data: PaintingCategoryWithPaintings = await res.json();
    return data.paintings;
}

/**
 * Fetch category data including paintings
 * Server endpoint: GET /api/PaintingCategories/{slug}
 * Returns PaintingCategoryWithPaintingsDto
 * Uses dummy data if USE_DUMMY_DATA is true, otherwise calls API
 * Cache duration: 1 hour
 */
export async function getCategoryData(categorySlug: string): Promise<PaintingCategoryWithPaintings> {
    if (USE_DUMMY_DATA) {
        console.log('[getCategoryData] Using dummy data for category:', categorySlug);
        const category = dummyCategories.find(cat => cat.slug === categorySlug);
        const paintings = getDummyPaintingsByCategory(categorySlug);

        if (!category) {
            throw new Error(`Category not found: ${categorySlug}`);
        }

        return {
            ...category,
            paintings
        };
    }

    const url = `${API_BASE_URL}/paintingcategories/${categorySlug}`;
    console.log('=== [getCategoryData] DEBUG ===');
    console.log('API_BASE_URL:', API_BASE_URL);
    console.log('categorySlug:', categorySlug);
    console.log('Full URL:', url);
    console.log('Is server component:', typeof window === 'undefined');
    console.log('===========================');

    try {
        const res = await fetch(url, {
            next: { revalidate: 3600 } // Cache for 1 hour
        } as RequestInit & { next: { revalidate: number } });

        console.log('[getCategoryData] Response status:', res.status, res.statusText);
        console.log('[getCategoryData] Response headers:', Object.fromEntries(res.headers.entries()));

        if (!res.ok) {
            const errorText = await res.text();
            console.error('[getCategoryData] Error response body:', errorText || '(empty)');
            throw new Error(`Failed to fetch category data for: ${categorySlug} (Status: ${res.status})`);
        }

        const data = await res.json();
        console.log('[getCategoryData] Successfully fetched data for category:', categorySlug, 'with', data.paintings?.length || 0, 'paintings');
        return data;
    } catch (error) {
        console.error('[getCategoryData] Fetch error:', error);
        throw error;
    }
}

/**
 * Fetch all paintings across all categories
 * Server endpoint: GET /api/Paintings
 * Uses dummy data if USE_DUMMY_DATA is true, otherwise calls API
 * Cache duration: 1 hour
 */
export async function getAllPaintings(): Promise<Painting[]> {
    if (USE_DUMMY_DATA) {
        return dummyPaintings;
    }

    const res = await fetch(`${API_BASE_URL}/paintings`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    } as RequestInit & { next: { revalidate: number } });

    if (!res.ok) {
        throw new Error('Failed to fetch all paintings');
    }

    return res.json();
}

/**
 * Fetch a single painting by slug
 * Server endpoint: GET /api/Paintings/{slug}
 * Uses dummy data if USE_DUMMY_DATA is true, otherwise calls API
 * Cache duration: 24 hours (static content)
 */
export async function getPaintingBySlug(slug: string): Promise<Painting> {
    if (USE_DUMMY_DATA) {
        const painting = dummyPaintings.find(p => p.slug === slug);
        if (!painting) {
            throw new Error(`Painting not found with slug: ${slug}`);
        }
        return painting;
    }

    const res = await fetch(`${API_BASE_URL}/paintings/${slug}`, {
        next: { revalidate: 86400 } // Cache for 24 hours
    } as RequestInit & { next: { revalidate: number } });

    if (!res.ok) {
        throw new Error(`Failed to fetch painting with slug: ${slug}`);
    }

    return res.json();
}

/**
 * Fetch page content by address
 * Server endpoint: GET /api/PageContent/{address}
 * Uses dummy data if USE_DUMMY_DATA is true, otherwise calls API
 */
export async function getPageContent(address: string): Promise<PageContentDto | undefined> {
    if (USE_DUMMY_DATA) {
        return dummyPageContent.find(p => p.address === address);
    }

    const res = await fetch(`${API_BASE_URL}/pagecontent/${address}`, {
        next: { revalidate: 86400 } // Cache for 24 hours
    } as RequestInit & { next: { revalidate: number } });

    if (!res.ok) {
        if (res.status === 404) {
            return undefined;
        }
        throw new Error(`Failed to fetch page content for address: ${address}`);
    }

    return res.json();
}
