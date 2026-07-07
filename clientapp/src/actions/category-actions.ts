'use server';

import { revalidateTag } from 'next/cache';
import { categoryMutationTags } from '@/lib/cache-tags';
import {
    addPaintingCategory,
    updatePaintingCategory,
    deletePaintingCategory,
} from '@/lib/api';
import type {
    AddPaintingCategoryRequest,
    UpdatePaintingCategoryRequest,
    PaintingCategoryCreatedResult,
    CommandCompletionResponse,
} from '@/lib/api';

// ============================================================================
// Painting Category CRUD Actions
// ============================================================================

/**
 * Server Action to add a new painting category.
 * Invalidates all category-related cache tags after successful mutation.
 */
export async function addPaintingCategoryAction(
    data: AddPaintingCategoryRequest
): Promise<PaintingCategoryCreatedResult> {
    const result = await addPaintingCategory(data);

    // Invalidate all category-related caches
    for (const tag of categoryMutationTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to update an existing painting category.
 * Invalidates all category-related cache tags after successful mutation.
 */
export async function updatePaintingCategoryAction(
    id: string,
    data: UpdatePaintingCategoryRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    const result = await updatePaintingCategory(id, data, idempotencyKey);

    // Invalidate all category-related caches
    for (const tag of categoryMutationTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to delete a painting category.
 * Invalidates all category-related cache tags after successful mutation.
 */
export async function deletePaintingCategoryAction(id: string): Promise<void> {
    await deletePaintingCategory(id);

    // Invalidate all category-related caches
    for (const tag of categoryMutationTags) {
        await revalidateTag(tag, {});
    }
}
