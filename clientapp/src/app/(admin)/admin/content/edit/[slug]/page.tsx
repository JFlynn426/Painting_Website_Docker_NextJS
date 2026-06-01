'use client';

import { use, useState, useEffect } from 'react';
import Link from 'next/link';
import { getPageContent, updatePageContent, UpdatePageContentRequest } from '@/lib/api';
import type { PageContent } from '@/types/page-content';

interface EditPageContentProps {
    params: Promise<{ slug: string }>;
}

export default function EditPageContentPage({ params }: EditPageContentProps) {
    const { slug } = use(params);

    const [content, setContent] = useState<PageContent | null>(null);
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);

    // Form state
    const [title, setTitle] = useState('');
    const [body, setBody] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState<string | null>(null);
    const [submitSuccess, setSubmitSuccess] = useState(false);
    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        async function loadContent() {
            try {
                setLoading(true);
                const data = await getPageContent(slug);
                if (data) {
                    setContent(data);
                    setTitle(data.title || '');
                    setBody(data.content || '');
                } else {
                    setLoadError('Page content not found');
                }
            } catch (err) {
                setLoadError(err instanceof Error ? err.message : 'Failed to load page content');
            } finally {
                setLoading(false);
            }
        }
        loadContent();
    }, [slug]);

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};

        if (body && body.length > 10000) {
            newErrors.content = 'Content must be 10000 characters or less';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSubmitError(null);
        setSubmitSuccess(false);

        if (!content || !validate()) {
            return;
        }

        setIsSubmitting(true);

        const request: UpdatePageContentRequest = {
            title: title.trim() !== (content.title || '') ? title.trim() || undefined : undefined,
            content: body.trim() !== content.content ? body.trim() : undefined
        };

        try {
            await updatePageContent(content.id, request);
            setSubmitSuccess(true);
            setContent(prev => prev ? { ...prev, title: title.trim() || undefined, content: body.trim() } : null);
        } catch (err) {
            setSubmitError(err instanceof Error ? err.message : 'Failed to update page content');
        } finally {
            setIsSubmitting(false);
        }
    };

    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit Page Content</h1>
                <p className="text-gray-400">Loading...</p>
            </div>
        );
    }

    if (loadError || !content) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit Page Content</h1>
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">{loadError || 'Page content not found'}</p>
                </div>
                <Link href="/admin/content/edit" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Edit Content
                </Link>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit Page Content</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 mb-6">
                <p className="text-gray-400 mb-2"><strong>Slug:</strong> {content.slug}</p>
            </div>

            {submitError && (
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">Error: {submitError}</p>
                </div>
            )}

            {submitSuccess && (
                <div className="bg-green-900 bg-opacity-50 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-green-200">Page content updated successfully!</p>
                </div>
            )}

            <form onSubmit={handleSubmit} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <div className="mb-4">
                    <label htmlFor="title" className="block text-sm font-medium text-gray-300 mb-2">
                        Title (optional)
                    </label>
                    <input
                        type="text"
                        id="title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        className="w-full px-3 py-2 bg-[var(--background)] border border-gray-600 rounded text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                        placeholder="Enter page title..."
                    />
                    {errors.title && (
                        <p className="text-red-400 text-sm mt-1">{errors.title}</p>
                    )}
                </div>

                <div className="mb-4">
                    <label htmlFor="content" className="block text-sm font-medium text-gray-300 mb-2">
                        Content (HTML)
                    </label>
                    <textarea
                        id="content"
                        value={body}
                        onChange={(e) => setBody(e.target.value)}
                        rows={15}
                        className="w-full px-3 py-2 bg-[var(--background)] border border-gray-600 rounded text-white focus:outline-none focus:ring-2 focus:ring-blue-500 font-mono text-sm"
                        placeholder="Enter page content (HTML)..."
                    />
                    {errors.content && (
                        <p className="text-red-400 text-sm mt-1">{errors.content}</p>
                    )}
                </div>

                <div className="flex gap-4">
                    <button
                        type="submit"
                        disabled={isSubmitting}
                        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors font-bold disabled:opacity-50"
                    >
                        {isSubmitting ? 'Saving...' : 'Save Changes'}
                    </button>
                    <Link
                        href="/admin/content/edit"
                        className="px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors font-bold text-center"
                    >
                        Cancel
                    </Link>
                </div>
            </form>

            <div className="mt-6">
                <Link href="/admin/content/edit" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Edit Content
                </Link>
            </div>
        </div>
    );
}