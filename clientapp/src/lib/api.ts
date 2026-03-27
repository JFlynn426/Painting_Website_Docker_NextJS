// API Configuration
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

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

export interface PageContent {
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

export interface CarouselImage {
    id: string;
    imageUrl: string;
    alt: string;
    title?: string;
}

export interface PaintingImageItem {
    src: string;
    alt: string;
    filename: string;
}

export interface CategoryData {
    category: PaintingCategory;
    paintings: Painting[];
}

// ============================================================================
// API Service Functions
// ============================================================================

/**
 * Fetch all painting categories
 * Cache duration: 24 hours (categories rarely change)
 */
export async function getPaintingCategories(): Promise<PaintingCategory[]> {
    const res = await fetch(`${API_BASE_URL}/categories`, {
        next: { revalidate: 86400 } // Cache for 24 hours
    });

    if (!res.ok) {
        throw new Error('Failed to fetch painting categories');
    }

    return res.json();
}

/**
 * Fetch paintings by category slug
 * Cache duration: 1 hour (paintings may change daily)
 */
export async function getPaintingsByCategory(categorySlug: string): Promise<Painting[]> {
    const res = await fetch(`${API_BASE_URL}/paintings/category/${categorySlug}`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    });

    if (!res.ok) {
        throw new Error(`Failed to fetch paintings for category: ${categorySlug}`);
    }

    return res.json();
}

/**
 * Fetch category data including paintings
 * Cache duration: 1 hour
 */
export async function getCategoryData(categorySlug: string): Promise<PaintingCategoryWithPaintings> {
    const res = await fetch(`${API_BASE_URL}/categories/${categorySlug}`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    });

    if (!res.ok) {
        throw new Error(`Failed to fetch category data for: ${categorySlug}`);
    }

    return res.json();
}

/**
 * Fetch all paintings across all categories
 * Cache duration: 1 hour
 */
export async function getAllPaintings(): Promise<Painting[]> {
    const res = await fetch(`${API_BASE_URL}/paintings`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    });

    if (!res.ok) {
        throw new Error('Failed to fetch all paintings');
    }

    return res.json();
}

/**
 * Fetch a single painting by ID
 * Cache duration: 24 hours (static content)
 */
export async function getPaintingById(id: string): Promise<Painting> {
    const res = await fetch(`${API_BASE_URL}/paintings/${id}`, {
        next: { revalidate: 86400 } // Cache for 24 hours
    });

    if (!res.ok) {
        throw new Error(`Failed to fetch painting with ID: ${id}`);
    }

    return res.json();
}

/**
 * Fetch a painting by category and slug
 * Cache duration: 24 hours (static content)
 */
export async function getPaintingBySlug(categorySlug: string, slug: string): Promise<Painting> {
    const res = await fetch(`${API_BASE_URL}/paintings/${categorySlug}/${slug}`, {
        next: { revalidate: 86400 } // Cache for 24 hours
    });

    if (!res.ok) {
        throw new Error(`Failed to fetch painting: ${categorySlug}/${slug}`);
    }

    return res.json();
}

/**
 * Fetch carousel images
 * Cache duration: 2 hours (updated periodically)
 */
export async function getCarouselImages(): Promise<CarouselImage[]> {
    const res = await fetch(`${API_BASE_URL}/carousel`, {
        next: { revalidate: 7200 } // Cache for 2 hours
    });

    if (!res.ok) {
        throw new Error('Failed to fetch carousel images');
    }

    return res.json();
}

/**
 * Search paintings by query
 * Cache duration: 1 hour
 */
export async function searchPaintings(query: string): Promise<Painting[]> {
    const encodedQuery = encodeURIComponent(query);
    const res = await fetch(`${API_BASE_URL}/paintings/search?query=${encodedQuery}`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    });

    if (!res.ok) {
        throw new Error(`Failed to search paintings for query: ${query}`);
    }

    return res.json();
}

/**
 * Fetch paintings filtered by availability status
 * Cache duration: 1 hour
 */
export async function getPaintingsByAvailability(isAvailable: boolean): Promise<Painting[]> {
    const res = await fetch(`${API_BASE_URL}/paintings/availability?isAvailable=${isAvailable}`, {
        next: { revalidate: 3600 } // Cache for 1 hour
    });

    if (!res.ok) {
        throw new Error(`Failed to fetch paintings by availability: ${isAvailable}`);
    }

    return res.json();
}

/**
 * Fetch paintings filtered by price range
 * Cache duration: 1 hour
 */
export async function getPaintingsByPriceRange(minPrice: number, maxPrice: number): Promise<Painting[]> {
    const res = await fetch(
        `${API_BASE_URL}/paintings/price?min=${minPrice}&max=${maxPrice}`,
        {
            next: { revalidate: 3600 } // Cache for 1 hour
        }
    );

    if (!res.ok) {
        throw new Error(`Failed to fetch paintings in price range: $${minPrice} - $${maxPrice}`);
    }

    return res.json();
}

/**
 * Fetch paintings filtered by year range
 * Cache duration: 1 hour
 */
export async function getPaintingsByYearRange(minYear: number, maxYear: number): Promise<Painting[]> {
    const res = await fetch(
        `${API_BASE_URL}/paintings/year?min=${minYear}&max=${maxYear}`,
        {
            next: { revalidate: 3600 } // Cache for 1 hour
        }
    );

    if (!res.ok) {
        throw new Error(`Failed to fetch paintings in year range: ${minYear} - ${maxYear}`);
    }

    return res.json();
}