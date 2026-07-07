'use server';

import { revalidateTag } from 'next/cache';
import { CacheTags, paintingMutationTags, categoryAssignmentTags } from '@/lib/cache-tags';
import {
    addPainting,
    updatePainting,
    deletePainting,
    assignPaintingCategory,
    reassignPaintings,
    uploadImage,
    deleteImage,
} from '@/lib/api';
import type {
    AddPaintingRequest,
    UpdatePaintingRequest,
    ReassignPaintingsRequest,
    PaintingCreatedResult,
    CommandCompletionResponse,
    ImageUploadResult,
} from '@/lib/api';

// ============================================================================
// Painting CRUD Actions
// ============================================================================

/**
 * Server Action to add a new painting.
 * Invalidates all painting-related cache tags after successful mutation.
 */
export async function addPaintingAction(data: AddPaintingRequest): Promise<PaintingCreatedResult> {
    const result = await addPainting(data);

    // Invalidate all painting-related caches
    for (const tag of paintingMutationTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to update an existing painting.
 * Invalidates all painting-related cache tags after successful mutation.
 */
export async function updatePaintingAction(
    id: string,
    data: UpdatePaintingRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    const result = await updatePainting(id, data, idempotencyKey);

    // Invalidate all painting-related caches
    for (const tag of paintingMutationTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to delete a painting.
 * Invalidates all painting-related cache tags after successful mutation.
 */
export async function deletePaintingAction(id: string): Promise<void> {
    await deletePainting(id);

    // Invalidate all painting-related caches
    for (const tag of paintingMutationTags) {
        await revalidateTag(tag, {});
    }
}

/**
 * Server Action to assign a painting to a different category.
 * Invalidates both painting and category cache tags.
 */
export async function assignPaintingCategoryAction(
    paintingId: string,
    categoryId: string,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    const result = await assignPaintingCategory(paintingId, categoryId, idempotencyKey);

    // Invalidate painting and category caches
    for (const tag of categoryAssignmentTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to bulk reassign paintings to different categories.
 * Invalidates both painting and category cache tags.
 */
export async function reassignPaintingsAction(
    data: ReassignPaintingsRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    const result = await reassignPaintings(data, idempotencyKey);

    // Invalidate painting and category caches
    for (const tag of categoryAssignmentTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

// ============================================================================
// Image Actions
// ============================================================================

/**
 * Server Action to upload an image file.
 * Note: This runs on the server but receives the File from the client.
 */
export async function uploadImageAction(file: File): Promise<ImageUploadResult> {
    return await uploadImage(file);
}

/**
 * Server Action to delete an image file.
 * Invalidates painting caches since images are associated with paintings.
 */
export async function deleteImageAction(fileName: string): Promise<void> {
    await deleteImage(fileName);

    // Invalidate painting caches since images are associated with paintings
    for (const tag of paintingMutationTags) {
        await revalidateTag(tag, {});
    }
}
