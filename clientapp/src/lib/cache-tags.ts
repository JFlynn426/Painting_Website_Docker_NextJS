// ============================================================================
// Cache Tag Constants
// ============================================================================
// Centralized cache tag definitions for tag-based cache invalidation.
// Used with Next.js revalidateTag() to immediately clear stale cache entries
// when admin panel mutations occur.
// ============================================================================

export const CacheTags = {
    // Painting-related tags
    paintings: 'paintings',
    paintingCategories: 'paintingCategories',
    newPaintings: 'newPaintings',
    carousel: 'carousel',

    // Page content tags
    pageContents: 'pageContents',

    // Dynamic tag generators for specific resources
    painting: (slug: string) => `painting-${slug}`,
    paintingCategory: (slug: string) => `paintingCategory-${slug}`,
    pageContent: (slug: string) => `pageContent-${slug}`,

    // Wildcard tag - applied to all fetches, invalidated on any mutation
    // Ensures cache is always cleared even if specific tags are missed
    allContent: 'allContent',
} as const;

/**
 * Get all cache tags that should be invalidated when any painting-related
 * data changes (add, update, delete painting).
 */
export const paintingMutationTags = [
    CacheTags.paintings,
    CacheTags.newPaintings,
    CacheTags.carousel,
    CacheTags.allContent,
] as const;

/**
 * Get all cache tags that should be invalidated when painting categories
 * change (add, update, delete category).
 */
export const categoryMutationTags = [
    CacheTags.paintingCategories,
    CacheTags.allContent,
] as const;

/**
 * Get all cache tags that should be invalidated when paintings are reassigned
 * to different categories.
 */
export const categoryAssignmentTags = [
    CacheTags.paintings,
    CacheTags.paintingCategories,
    CacheTags.allContent,
] as const;

/**
 * Get all cache tags that should be invalidated when page content changes.
 */
export const pageContentMutationTags = [
    CacheTags.pageContents,
    CacheTags.allContent,
] as const;
