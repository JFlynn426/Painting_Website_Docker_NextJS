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
            next: { revalidate: 86400 } // Cache for 24 hours
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
            next: { revalidate: 86400 } // Cache for 24 hours
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
export async function getAllPageContents(): Promise<PageContent[]> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/pagecontent`, {
            cache: 'force-cache',
            next: { revalidate: 86400 }
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch page contents: ${response.statusText}`);
        }

        const results = await response.json() as RawPageContentDto[];
        return results.map(r => ({
            id: r.id,
            slug: r.address,
            title: r.title,
            content: r.content,
            photoUrls: r.photoUrls
        }));
    } catch (error) {
        throw error;
    }
}

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

        const result = await response.json() as RawPageContentDto;
        return {
            id: result.id,
            slug: result.address,
            title: result.title,
            content: result.content,
            photoUrls: result.photoUrls
        };
    } catch (error) {
        throw error;
    }
}

interface RawPageContentDto {
    id: string;
    address: string;
    title?: string;
    content: string;
    photoUrls?: string[];
}

// ============================================================================
// TypeScript Interfaces for Request/Response Types
// ============================================================================

/** Response from mutation commands indicating completion status */
export interface CommandCompletionResponse {
    success: boolean;
    message: string;
    completedAt: string;
    affectedRecords?: number;
}

/** Result returned when a painting is created */
export interface PaintingCreatedResult {
    id: string;
    slug: string;
}

/** Result returned when a painting category is created */
export interface PaintingCategoryCreatedResult {
    id: string;
    slug: string;
}

/** Result returned when page content is created */
export interface PageContentCreatedResult {
    id: string;
    address: string;
}

/** Request body for Google OAuth callback */
export interface GoogleAuthCallbackRequest {
    code: string;
    state: string;
}

/** Response from Google OAuth callback */
export interface GoogleAuthCallbackResponse {
    token: string;
    adminUser: {
        id: string;
        email: string;
        displayName: string;
        pictureUrl?: string;
        lastLoginAt: string;
        createdAt: string;
        isActive: boolean;
    };
}

/** Request body for adding a painting */
export interface AddPaintingRequest {
    title: string;
    description?: string;
    imageUrl: string;
    thumbnailUrl?: string;
    categorySlug: string;
    price?: number;
    width?: number;
    height?: number;
    depth?: number;
    year?: number;
    isAvailable: boolean;
    isNew: boolean;
}

/** Request body for updating a painting */
export interface UpdatePaintingRequest {
    name?: string;
    description?: string;
    imageUrl?: string;
    thumbnailUrl?: string;
    slug?: string;
    categoryId?: string;
    width?: number;
    height?: number;
    depth?: number;
    year?: number;
    price?: number;
    isAvailable?: boolean;
    isNew?: boolean;
}

/** Request body for adding a painting category */
export interface AddPaintingCategoryRequest {
    name: string;
    description?: string;
}

/** Request body for updating a painting category */
export interface UpdatePaintingCategoryRequest {
    name?: string;
    description?: string;
}

/** Request body for adding page content */
export interface AddPageContentRequest {
    address: string;
    title?: string;
    content: string;
}

/** Request body for updating page content */
export interface UpdatePageContentRequest {
    title?: string;
    content?: string;
    photoUrls?: string[];
}

/** Request body for updating an admin user */
export interface UpdateAdminUserRequest {
    displayName?: string;
    pictureUrl?: string;
    isActive?: boolean;
}

/** Request body for bulk reassigning paintings to categories */
export interface ReassignPaintingsRequest {
    paintingIdToCategoryId: Record<string, string>;
}

// ============================================================================
// Auth Endpoints
// ============================================================================

/**
 * Get Google OAuth authorization URL
 * Endpoint: GET api/auth/google/url
 */
export async function getGoogleAuthUrl(): Promise<{ url: string }> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/auth/google/url`, {
            cache: 'no-store'
        });

        if (!response.ok) {
            throw new Error(`Failed to get Google auth URL: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Handle Google OAuth callback
 * Endpoint: POST api/auth/google/callback
 * Sets httpOnly cookie with JWT token on success
 */
export async function googleAuthCallback(request: GoogleAuthCallbackRequest): Promise<GoogleAuthCallbackResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/auth/google/callback`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `Google auth callback failed: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Update an admin user
 * Endpoint: PATCH api/auth/{id}
 * Requires admin_token cookie authentication
 */
export async function updateAdminUser(
    id: string,
    request: UpdateAdminUserRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const headers: Record<string, string> = { 'Content-Type': 'application/json' };
        if (idempotencyKey) {
            headers['X-Idempotency-Key'] = idempotencyKey;
        }

        const response = await fetch(`${API_BASE_URL}/auth/${id}`, {
            method: 'PATCH',
            headers,
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to update admin user: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

// ============================================================================
// Painting Mutation Endpoints
// ============================================================================

/**
 * Add a new painting
 * Endpoint: POST api/paintings
 * Requires admin_token cookie authentication
 */
export async function addPainting(request: AddPaintingRequest): Promise<PaintingCreatedResult> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to add painting: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Update a painting
 * Endpoint: PATCH api/paintings/{id}
 * Requires admin_token cookie authentication
 */
export async function updatePainting(
    id: string,
    request: UpdatePaintingRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const headers: Record<string, string> = { 'Content-Type': 'application/json' };
        if (idempotencyKey) {
            headers['X-Idempotency-Key'] = idempotencyKey;
        }

        const response = await fetch(`${API_BASE_URL}/paintings/${id}`, {
            method: 'PATCH',
            headers,
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to update painting: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Delete a painting
 * Endpoint: DELETE api/paintings/{id}
 * Requires admin_token cookie authentication
 */
export async function deletePainting(id: string): Promise<void> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintings/${id}`, {
            method: 'DELETE',
            credentials: 'include',
            cache: 'no-store'
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to delete painting: ${response.statusText}`);
        }
    } catch (error) {
        throw error;
    }
}

/**
 * Assign a painting to a different category
 * Endpoint: PATCH api/paintings/{paintingId}/category/{categoryId}
 * Requires admin_token cookie authentication
 */
export async function assignPaintingCategory(
    paintingId: string,
    categoryId: string,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const headers: Record<string, string> = {};
        if (idempotencyKey) {
            headers['X-Idempotency-Key'] = idempotencyKey;
        }

        const response = await fetch(`${API_BASE_URL}/paintings/${paintingId}/category/${categoryId}`, {
            method: 'PATCH',
            headers,
            credentials: 'include',
            cache: 'no-store'
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to assign painting category: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Bulk reassign paintings to different categories
 * Endpoint: POST api/paintings/reassign
 * Requires admin_token cookie authentication
 */
export async function reassignPaintings(
    request: ReassignPaintingsRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const headers: Record<string, string> = { 'Content-Type': 'application/json' };
        if (idempotencyKey) {
            headers['X-Idempotency-Key'] = idempotencyKey;
        }

        const response = await fetch(`${API_BASE_URL}/paintings/reassign`, {
            method: 'POST',
            headers,
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to reassign paintings: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

// ============================================================================
// Painting Category Mutation Endpoints
// ============================================================================

/**
 * Add a new painting category
 * Endpoint: POST api/paintingcategories
 * Requires admin_token cookie authentication
 */
export async function addPaintingCategory(request: AddPaintingCategoryRequest): Promise<PaintingCategoryCreatedResult> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintingcategories`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to add painting category: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Update a painting category
 * Endpoint: PATCH api/paintingcategories/{id}
 * Requires admin_token cookie authentication
 */
export async function updatePaintingCategory(
    id: string,
    request: UpdatePaintingCategoryRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const headers: Record<string, string> = { 'Content-Type': 'application/json' };
        if (idempotencyKey) {
            headers['X-Idempotency-Key'] = idempotencyKey;
        }

        const response = await fetch(`${API_BASE_URL}/paintingcategories/${id}`, {
            method: 'PATCH',
            headers,
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to update painting category: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Delete a painting category
 * Endpoint: DELETE api/paintingcategories/{id}
 * Requires admin_token cookie authentication
 */
export async function deletePaintingCategory(id: string): Promise<void> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/paintingcategories/${id}`, {
            method: 'DELETE',
            credentials: 'include',
            cache: 'no-store'
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to delete painting category: ${response.statusText}`);
        }
    } catch (error) {
        throw error;
    }
}

// ============================================================================
// Page Content Mutation Endpoints
// ============================================================================

/**
 * Add new page content
 * Endpoint: POST api/pagecontent
 * Requires admin_token cookie authentication
 */
export async function addPageContent(request: AddPageContentRequest): Promise<PageContentCreatedResult> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/pagecontent`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to add page content: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Update page content
 * Endpoint: PATCH api/pagecontent/{id}
 * Requires admin_token cookie authentication
 */
export async function updatePageContent(
    id: string,
    request: UpdatePageContentRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const headers: Record<string, string> = { 'Content-Type': 'application/json' };
        if (idempotencyKey) {
            headers['X-Idempotency-Key'] = idempotencyKey;
        }

        const response = await fetch(`${API_BASE_URL}/pagecontent/${id}`, {
            method: 'PATCH',
            headers,
            credentials: 'include',
            cache: 'no-store',
            body: JSON.stringify(request)
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to update page content: ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        throw error;
    }
}

/**
 * Delete page content
 * Endpoint: DELETE api/pagecontent/{address}
 * Requires admin_token cookie authentication
 */
export async function deletePageContent(address: string): Promise<void> {
    try {
        const API_BASE_URL = getApiBaseUrl();
        const response = await fetch(`${API_BASE_URL}/pagecontent/${address}`, {
            method: 'DELETE',
            credentials: 'include',
            cache: 'no-store'
        });

        if (response.status === 401) {
            if (typeof window !== 'undefined') {
                localStorage.removeItem('admin_user');
                window.location.href = '/admin/login';
            }
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            throw new Error(`Failed to delete page content: ${response.statusText}`);
        }
    } catch (error) {
        throw error;
    }
}

// ============================================================================
// Authenticated Fetch Wrapper
// ============================================================================
// Wraps fetch to automatically include credentials and handle 401 responses.
// Only use this for admin mutation endpoints that require authentication.
// ============================================================================

/**
 * Fetch wrapper that includes credentials (cookies) and handles 401 responses.
 * On 401, clears stale session data and redirects to login page.
 * This wrapper is only used for admin mutation endpoints, not public read endpoints.
 */
export async function fetchWithAuth(url: string, options?: RequestInit): Promise<Response> {
    const response = await fetch(url, {
        ...options,
        credentials: 'include'
    });

    if (response.status === 401) {
        // Clear any stale session data
        if (typeof window !== 'undefined') {
            localStorage.removeItem('admin_user');
            // Redirect to login
            window.location.href = '/admin/login';
        }
    }

    return response;
}
