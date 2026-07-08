'use server';

import { cookies } from 'next/headers';
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
    const cookieStore = await cookies();
    const authToken = cookieStore.get('admin_token')?.value;
    const result = await addPaintingCategory(data, authToken);

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
    const cookieStore = await cookies();
    const authToken = cookieStore.get('admin_token')?.value;
    const result = await updatePaintingCategory(id, data, idempotencyKey, authToken);

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
    const cookieStore = await cookies();
    const authToken = cookieStore.get('admin_token')?.value;
    await deletePaintingCategory(id, authToken);

    // Invalidate all category-related caches
    for (const tag of categoryMutationTags) {
        await revalidateTag(tag, {});
    }
}
