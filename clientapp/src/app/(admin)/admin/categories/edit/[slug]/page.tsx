'use client';

import { use, useState, useEffect } from 'react';
import Link from 'next/link';
import { getCategoryData, UpdatePaintingCategoryRequest } from '@/lib/api';
import { updatePaintingCategoryAction } from '@/actions/category-actions';
import type { PaintingCategory } from '@/types/paintings';

interface EditCategoryPageProps {
    params: Promise<{ slug: string }>;
}

export default function EditCategoryPage({ params }: EditCategoryPageProps) {
    const { slug } = use(params);

    const [category, setCategory] = useState<PaintingCategory | null>(null);
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);

    // Form state - initialized with original values once category loads
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState<string | null>(null);
    const [submitSuccess, setSubmitSuccess] = useState(false);
    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        async function loadCategory() {
            try {
                setLoading(true);
                const data = await getCategoryData(slug, { noCache: true });
                if (data) {
                    setCategory(data);
                    setName(data.name);
                    setDescription(data.description || '');
                } else {
                    setLoadError('Category not found');
                }
            } catch (err) {
                setLoadError(err instanceof Error ? err.message : 'Failed to load category');
            } finally {
                setLoading(false);
            }
        }
        loadCategory();
    }, [slug]);

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};

        // Name is required, min 3 chars (trimmed), max 50 chars
        if (!name.trim()) {
            newErrors.name = 'Category name is required';
        } else if (name.trim().length < 3) {
            newErrors.name = 'Category name must be at least 3 characters';
        } else if (name.length > 50) {
            newErrors.name = 'Category name must be 50 characters or less';
        }

        // Description max 200 chars
        if (description && description.length > 200) {
            newErrors.description = 'Description must be 200 characters or less';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitError(null);
        setSubmitSuccess(false);

        if (!category || !validate()) {
            return;
        }

        setIsSubmitting(true);

        const request: UpdatePaintingCategoryRequest = {
            name: name.trim() !== category.name ? name.trim() : undefined,
            description: description.trim() !== (category.description || '') ? description.trim() || undefined : undefined,
        };

        try {
            await updatePaintingCategoryAction(category.id, request);
            setSubmitSuccess(true);
        } catch (err) {
            setSubmitError(err instanceof Error ? err.message : 'Failed to update category');
        } finally {
            setIsSubmitting(false);
        }
    };


    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                    Edit Category
                </h1>
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <p className="text-gray-400">Loading category data...</p>
                </div>
            </div>
        );
    }

    if (loadError || !category) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                    Edit Category
                </h1>
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-6">
                    <p className="text-red-200">{loadError || 'Category not found'}</p>
                </div>
                <div className="mt-6">
                    <Link href="/admin/categories/edit" className="block text-[var(--title-color)] hover:underline">
                        &larr; Back to Edit Categories
                    </Link>
                </div>
            </div>
        );
    }

    const originalName = category.name;
    const originalDescription = category.description || '';

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                Edit Category: {category.name}
            </h1>

            {submitSuccess && (
                <div className="bg-green-900 bg-opacity-50 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-green-200">Category updated successfully!</p>
                </div>
            )}

            {submitError && (
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">Error: {submitError}</p>
                </div>
            )}

            <form onSubmit={handleSubmit} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 space-y-4">
                {/* Name */}
                <div>
                    <label htmlFor="name" className="block text-sm font-medium text-gray-300 mb-1">
                        Category Name <span className="text-red-500">*</span>
                    </label>
                    <input
                        type="text"
                        id="name"
                        value={name}
                        onChange={e => setName(e.target.value)}
                        maxLength={50}
                        className="w-full px-3 py-2 bg-[var(--background)] text-white border border-gray-600 rounded focus:outline-none focus:border-blue-500"
                    />
                    <p className="text-gray-500 text-xs mt-1">{name.length}/50 characters</p>
                    {name !== originalName && (
                        <p className="text-white text-xs mt-1">Original: {originalName}</p>
                    )}
                    {errors.name && <p className="text-red-500 text-sm mt-1">{errors.name}</p>}
                </div>

                {/* Description */}
                <div>
                    <label htmlFor="description" className="block text-sm font-medium text-gray-300 mb-1">
                        Description
                    </label>
                    <textarea
                        id="description"
                        value={description}
                        onChange={e => setDescription(e.target.value)}
                        maxLength={200}
                        rows={4}
                        className="w-full px-3 py-2 bg-[var(--background)] text-white border border-gray-600 rounded focus:outline-none focus:border-blue-500"
                    />
                    <p className="text-gray-500 text-xs mt-1">{description.length}/200 characters</p>
                    {description !== originalDescription && (
                        <p className="text-white text-xs mt-1">Original: {originalDescription || '(empty)'}</p>
                    )}
                    {errors.description && <p className="text-red-500 text-sm mt-1">{errors.description}</p>}
                </div>

                {/* Submit */}
                <div className="flex gap-4 pt-4">
                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors disabled:opacity-50"
                    >
                        {isSubmitting ? 'Updating...' : 'Update Category'}
                    </button>
                    <Link
                        href="/admin/categories/edit"
                        className="px-6 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors text-center"
                    >
                        Cancel
                    </Link>
                </div>
            </form>

            <div className="mt-6 space-y-2">
                <Link href="/admin/categories/edit" className="block text-[var(--title-color)] hover:underline">
                    &larr; Back to Edit Categories
                </Link>
                <Link href="/admin/categories" className="block text-[var(--title-color)] hover:underline">
                    &larr; Back to Category Management
                </Link>
            </div>
        </div>
    );
}
