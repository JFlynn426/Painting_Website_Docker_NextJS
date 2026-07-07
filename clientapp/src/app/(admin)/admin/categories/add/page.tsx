'use client';

import { useState } from 'react';
import Link from 'next/link';
import { AddPaintingCategoryRequest } from '@/lib/api';
import { addPaintingCategoryAction } from '@/actions/category-actions';

export default function AddCategoryPage() {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState<string | null>(null);
    const [submitSuccess, setSubmitSuccess] = useState(false);
    const [errors, setErrors] = useState<Record<string, string>>({});

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};

        if (!name.trim()) {
            newErrors.name = 'Category name is required';
        } else if (name.trim().length > 50) {
            newErrors.name = 'Category name must be 50 characters or less';
        }

        if (description.trim().length > 200) {
            newErrors.description = 'Description must be 200 characters or less';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitError(null);
        setSubmitSuccess(false);

        if (!validate()) {
            return;
        }

        setIsSubmitting(true);

        const request: AddPaintingCategoryRequest = {
            name: name.trim(),
            description: description.trim() || undefined,
        };

        try {
            await addPaintingCategoryAction(request);
            setSubmitSuccess(true);
            setName('');
            setDescription('');
        } catch (err) {
            setSubmitError(err instanceof Error ? err.message : 'Failed to add category');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">
                Add Painting Category
            </h1>

            {submitSuccess && (
                <div className="bg-green-900 bg-opacity-50 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-green-200">Category added successfully!</p>
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
                        placeholder="Enter category name"
                    />
                    <p className="text-gray-500 text-xs mt-1">{name.length}/50 characters</p>
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
                        placeholder="Enter category description (optional) - This description is displayed underneath the category title"
                    />
                    <p className="text-gray-500 text-xs mt-1">{description.length}/200 characters</p>
                    {errors.description && <p className="text-red-500 text-sm mt-1">{errors.description}</p>}
                </div>

                {/* Submit */}
                <div className="flex gap-4 pt-4">
                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors disabled:opacity-50"
                    >
                        {isSubmitting ? 'Adding...' : 'Add Category'}
                    </button>
                    <Link
                        href="/admin/categories"
                        className="px-6 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors text-center"
                    >
                        Cancel
                    </Link>
                </div>
            </form>

            <div className="mt-4">
                <Link href="/admin/categories" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Category Management
                </Link>
            </div>
        </div>
    );
}
