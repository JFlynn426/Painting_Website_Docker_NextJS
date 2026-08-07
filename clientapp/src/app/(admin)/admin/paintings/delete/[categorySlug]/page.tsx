'use client';

import { use, useState, useCallback } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { getPaintingsByCategory } from '@/lib/api';
import { deletePaintingAction } from '@/actions/painting-actions';
import type { PaintingCategoryWithPaintings } from '@/types/paintings';

interface DeleteCategoryPaintingsPageProps {
    params: Promise<{ categorySlug: string }>;
}

export default function DeleteCategoryPaintingsPage({ params }: DeleteCategoryPaintingsPageProps) {
    const { categorySlug } = use(params);

    const [category, setCategory] = useState<PaintingCategoryWithPaintings | null>(null);
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);
    const [deletingId, setDeletingId] = useState<string | null>(null);
    const [deleteError, setDeleteError] = useState<string | null>(null);
    const [deletedCount, setDeletedCount] = useState(0);
    const [confirmDelete, setConfirmDelete] = useState<string | null>(null);

    const categoryName = categorySlug.split('-').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');

    const handleDelete = useCallback(async (paintingId: string, paintingTitle: string) => {
        setDeleteError(null);
        try {
            setDeletingId(paintingId);
            await deletePaintingAction(paintingId);
            setDeletedCount(prev => prev + 1);
            window.scrollTo(0, 0);
            // Remove from local list
            setCategory(prev => {
                if (!prev) return prev;
                return {
                    ...prev,
                    paintings: prev.paintings.filter(p => p.id !== paintingId)
                };
            });
        } catch (err) {
            setDeleteError(err instanceof Error ? err.message : `Failed to delete "${paintingTitle}"`);
        } finally {
            setDeletingId(null);
            setConfirmDelete(null);
        }
    }, []);

    // Load paintings on mount
    useState(() => {
        async function loadCategory() {
            try {
                setLoading(true);
                const data = await getPaintingsByCategory(categorySlug, { noCache: true });
                setCategory(data);
            } catch (err) {
                setLoadError(err instanceof Error ? err.message : 'Failed to fetch category paintings');
            } finally {
                setLoading(false);
            }
        }
        loadCategory();
    });

    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                    Delete Paintings from {categoryName}
                </h1>
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <p className="text-[var(--foreground)]">Loading paintings...</p>
                </div>
            </div>
        );
    }

    if (loadError) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                    Delete Paintings from {categoryName}
                </h1>
                <div className="bg-red-200 border border-red-500 rounded-lg p-6">
                    <p className="text-black">Error: {loadError}</p>
                </div>
                <div className="mt-6">
                    <Link href="/admin/paintings/delete" className="text-[var(--title-color)] hover:underline">
                        &larr; Back to Categories
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">
                Delete Paintings from {categoryName}
            </h1>
            <p className="text-[var(--foreground)] mb-6">
                Click the delete button to permanently remove a painting from the database. This action cannot be undone.
            </p>

            {deletedCount > 0 && (
                <div className="bg-green-200 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-black">{deletedCount} painting{deletedCount !== 1 ? 's' : ''} deleted successfully.</p>
                </div>
            )}

            {deleteError && (
                <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Error: {deleteError}</p>
                </div>
            )}

            {category && category.paintings.length === 0 ? (
                <p className="text-[var(--foreground)]">No paintings found in this category.</p>
            ) : category ? (
                <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                    {category.paintings.map((painting) => (
                        <div
                            key={painting.id}
                            className="bg-[var(--navbar-footer-bg)] rounded-lg p-3 hover:bg-[var(--background)] transition-colors group"
                        >
                            <div className="aspect-square mb-2 overflow-hidden rounded relative">
                                <Image
                                    src={painting.thumbnailUrl || painting.imageUrl}
                                    alt={painting.title}
                                    fill
                                    sizes="(max-width: 768px) 50vw, (max-width: 1024px) 33vw, 25vw"
                                    className="object-cover group-hover:opacity-80 transition-opacity"
                                    unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
                                />
                            </div>
                            <p className="text-sm text-[var(--foreground)] truncate group-hover:text-[var(--title-color)] transition-colors mb-2">
                                {painting.title}
                            </p>

                            {confirmDelete === painting.id ? (
                                <div className="space-y-1">
                                    <p className="text-red-700 text-xs">Are you sure?</p>
                                    <button
                                        onClick={() => handleDelete(painting.id, painting.title)}
                                        disabled={deletingId === painting.id}
                                        className="w-full px-2 py-1 bg-red-700 text-white rounded text-xs hover:bg-red-800 transition-colors disabled:opacity-50"
                                    >
                                        {deletingId === painting.id ? '...' : 'Yes, Delete'}
                                    </button>
                                    <button
                                        onClick={() => setConfirmDelete(null)}
                                        className="w-full px-2 py-1 bg-gray-600 text-white rounded text-xs hover:bg-gray-700 transition-colors"
                                    >
                                        Cancel
                                    </button>
                                </div>
                            ) : (
                                <button
                                    onClick={() => setConfirmDelete(painting.id)}
                                    disabled={deletingId !== null}
                                    className="w-full px-2 py-1 bg-red-600 text-white rounded text-xs hover:bg-red-700 transition-colors disabled:opacity-50"
                                >
                                    Delete
                                </button>
                            )}
                        </div>
                    ))}
                </div>
            ) : null}

            <div className="mt-6 space-y-2">
                <Link href="/admin/paintings/delete" className="block text-[var(--title-color)] hover:underline">
                    &larr; Back to Categories
                </Link>
                <Link href="/admin/paintings" className="block text-[var(--title-color)] hover:underline">
                    &larr; Back to Paintings Admin
                </Link>
            </div>
        </div>
    );
}
