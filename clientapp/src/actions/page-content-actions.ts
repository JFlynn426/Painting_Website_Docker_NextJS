'use server';

import { cookies } from 'next/headers';
import { revalidateTag } from 'next/cache';
import { pageContentMutationTags } from '@/lib/cache-tags';
import {
    addPageContent,
    updatePageContent,
    deletePageContent,
} from '@/lib/api';
import type {
    AddPageContentRequest,
    UpdatePageContentRequest,
    PageContentCreatedResult,
    CommandCompletionResponse,
} from '@/lib/api';

// ============================================================================
// Page Content CRUD Actions
// ============================================================================

/**
 * Server Action to add new page content.
 * Invalidates all page content cache tags after successful mutation.
 */
export async function addPageContentAction(
    data: AddPageContentRequest
): Promise<PageContentCreatedResult> {
    const cookieStore = await cookies();
    const authToken = cookieStore.get('admin_token')?.value;
    const result = await addPageContent(data, authToken);

    // Invalidate all page content caches
    for (const tag of pageContentMutationTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to update existing page content.
 * Invalidates all page content cache tags after successful mutation.
 */
export async function updatePageContentAction(
    id: string,
    data: UpdatePageContentRequest,
    idempotencyKey?: string
): Promise<CommandCompletionResponse> {
    const cookieStore = await cookies();
    const authToken = cookieStore.get('admin_token')?.value;
    const result = await updatePageContent(id, data, idempotencyKey, authToken);

    // Invalidate all page content caches
    for (const tag of pageContentMutationTags) {
        await revalidateTag(tag, {});
    }

    return result;
}

/**
 * Server Action to delete page content.
 * Invalidates all page content cache tags after successful mutation.
 */
export async function deletePageContentAction(address: string): Promise<void> {
    const cookieStore = await cookies();
    const authToken = cookieStore.get('admin_token')?.value;
    await deletePageContent(address, authToken);

    // Invalidate all page content caches
    for (const tag of pageContentMutationTags) {
        await revalidateTag(tag, {});
    }
}
