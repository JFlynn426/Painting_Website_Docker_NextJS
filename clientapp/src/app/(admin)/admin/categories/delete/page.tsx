'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { getAllPaintingCategories, getCategoryData } from '@/lib/api';
import { deletePaintingCategoryAction } from '@/actions/category-actions';
import { PaintingCategory } from '@/types';

interface CategoryWithPaintingCount extends PaintingCategory {
    paintingCount: number;
}

export default function DeleteCategoryPage() {
    const [categories, setCategories] = useState<CategoryWithPaintingCount[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [deletingId, setDeletingId] = useState<string | null>(null);
    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Painting";
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();

    useEffect(() => {
        loadCategories();
    }, []);

    const loadCategories = async () => {
        try {
            setLoading(true);
            setError(null);
            const allCategories = await getAllPaintingCategories({ noCache: true });
            const filteredCategories = allCategories.filter(c => c.slug !== 'new-paintings');

            const categoryData = await Promise.all(
                filteredCategories.map(async (category) => {
                    try {
                        const data = await getCategoryData(category.slug, { noCache: true });
                        return {
                            ...category,
                            paintingCount: data ? data.paintings.length : 0
                        };
                    } catch {
                        return {
                            ...category,
                            paintingCount: 0
                        };
                    }
                })
            );

            setCategories(categoryData);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to fetch categories');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: string, name: string) => {
        if (!confirm(`Are you sure you want to permanently delete the category "${name}"? This action cannot be undone.`)) {
            return;
        }

        try {
            setDeletingId(id);
            setSuccess(null);
            setError(null);
            await deletePaintingCategoryAction(id);
            setSuccess(`Category "${name}" has been deleted successfully.`);
            setCategories(prev => prev.filter(c => c.id !== id));
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to delete category');
        } finally {
            setDeletingId(null);
        }
    };

    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Delete {artworkLabel} Categories</h1>
                <p className="text-[var(--foreground)]">Loading categories...</p>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Delete {artworkLabel} Categories</h1>
            <p className="text-[var(--foreground)] mb-4">
                Select a category below to delete. A category must be empty of {artworkLabelLowerPlural} before it can be deleted.
            </p>

            <div className="bg-yellow-100 border border-yellow-600 rounded-lg p-4 mb-6">
                <p className="text-black text-sm">
                    <strong>Note:</strong> {artworkLabel} categories can only be deleted if the category has had all of the {artworkLabelLowerPlural} removed from the category.
                    Use the <Link href="/admin/categories/reassign" className="underline font-medium">Reassign {artworkLabelPlural}</Link> feature to move {artworkLabelLowerPlural} to another category before deleting.
                </p>
            </div>

            {success && (
                <div className="bg-green-200 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-black">{success}</p>
                </div>
            )}

            {error && (
                <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Error: {error}</p>
                </div>
            )}

            {categories.length === 0 && !error ? (
                <p className="text-[var(--foreground)]">No categories found.</p>
            ) : (
                <div className="space-y-4">
                    {categories.map((category) => (
                        <div key={category.id} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 flex justify-between items-center">
                            <div>
                                <h2 className="text-xl font-bold text-[var(--title-color)]">{category.name}</h2>
                                {category.description && (
                                    <p className="text-[var(--foreground)]">{category.description}</p>
                                )}
                                <p className="text-gray-500 text-sm mt-1">
                                    {category.paintingCount} {artworkLabelLowerPlural} in this category
                                </p>
                            </div>
                            {category.paintingCount > 0 ? (
                                <span
                                    className="flex-shrink-0 ml-4 px-4 py-2 bg-gray-600 text-[var(--foreground)] rounded text-sm font-bold cursor-not-allowed"
                                    title={`Remove all ${artworkLabelLowerPlural} from this category before deleting`}
                                >
                                    Remove {artworkLabelPlural}
                                </span>
                            ) : (
                                <button
                                    onClick={() => handleDelete(category.id, category.name)}
                                    disabled={deletingId === category.id}
                                    className="flex-shrink-0 ml-4 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors text-sm font-bold disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                    {deletingId === category.id ? 'Deleting...' : 'Delete'}
                                </button>
                            )}
                        </div>
                    ))}
                </div>
            )}

            <div className="mt-6">
                <Link href="/admin/categories" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Category Management
                </Link>
            </div>
        </div>
    );
}
