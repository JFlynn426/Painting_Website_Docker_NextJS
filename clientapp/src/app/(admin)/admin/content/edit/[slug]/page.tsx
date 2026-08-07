'use client';

import { Fragment, use, useState, useEffect, useRef } from 'react';
import Image from 'next/image';
import Link from 'next/link';
import { getPageContent, UpdatePageContentRequest, uploadImage } from '@/lib/api';
import { updatePageContentAction } from '@/actions/page-content-actions';
import { renderParagraphsClient, sanitizeHtmlForStorage } from '@/lib/client-sanitization';
import type { PageContent } from '@/types/page-content';
import UploadSpinnerModal from '@/components/UploadSpinnerModal';

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
    const [photoUrls, setPhotoUrls] = useState<string[]>([]);
    const [isUploading, setIsUploading] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState<string | null>(null);
    const [submitSuccess, setSubmitSuccess] = useState(false);
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [initialContentHtml, setInitialContentHtml] = useState('');
    const contentEditorRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        async function loadContent() {
            try {
                setLoading(true);
                const data = await getPageContent(slug, { noCache: true });
                if (data) {
                    setContent(data);
                    setTitle(data.title || '');
                    setPhotoUrls(data.photoUrls || []);
                    // Store initial rendered HTML for display
                    const initialHtml = renderParagraphsClient(data.content || '');
                    setInitialContentHtml(initialHtml);
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

    // Populate content editor when content loads - render plain text as HTML
    useEffect(() => {
        if (content && contentEditorRef.current) {
            const html = renderParagraphsClient(content.content || '');
            contentEditorRef.current.innerHTML = html;
        }
    }, [content]);

    const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (file) {
            // Validate file type - only JPG/JPEG allowed
            const validTypes = ['image/jpeg', 'image/jpg'];
            if (!validTypes.includes(file.type)) {
                setErrors((prev: Record<string, string>) => ({ ...prev, file: 'Please select a JPG/JPEG image file' }));
                return;
            }

            // Validate file size (max 20MB)
            if (file.size > 20 * 1024 * 1024) {
                setErrors((prev: Record<string, string>) => ({ ...prev, file: 'File size must be less than 20MB' }));
                return;
            }

            setIsUploading(true);
            setErrors((prev: Record<string, string>) => {
                const next = { ...prev };
                delete next.file;
                return next;
            });

            try {
                // Upload to server via API endpoint (avoids Server Actions body size limit)
                const result = await uploadImage(file);
                setPhotoUrls(prev => {
                    const next = [...prev];
                    next[0] = result.originalUrl;
                    return next;
                });
            } catch (err) {
                const message = err instanceof Error ? err.message : 'Failed to upload image';
                setErrors((prev: Record<string, string>) => ({ ...prev, file: message }));
            } finally {
                setIsUploading(false);
            }
        }
    };

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};

        const editorContent = contentEditorRef.current?.innerHTML || '';
        if (editorContent && editorContent.length > 10000) {
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

        // Sanitize HTML from contentEditable for safe storage
        const rawHtml = contentEditorRef.current?.innerHTML || '';
        const sanitizedHtml = sanitizeHtmlForStorage(rawHtml).trim();

        // Compare photoUrls - only send if changed
        const currentPhotoUrls = content.photoUrls || [];
        const newPhotoUrls = photoUrls.filter(u => u.trim() !== '');
        const photoUrlsChanged = newPhotoUrls.length !== currentPhotoUrls.length ||
            newPhotoUrls.some((url, i) => url !== currentPhotoUrls[i]);

        const request: UpdatePageContentRequest = {
            title: title.trim() !== (content.title || '') ? title.trim() || undefined : undefined,
            content: sanitizedHtml !== content.content ? sanitizedHtml : undefined,
            photoUrls: photoUrlsChanged ? newPhotoUrls : undefined
        };

        try {
            await updatePageContentAction(content.id, request);
            setSubmitSuccess(true);
            setContent(prev => prev ? { ...prev, title: title.trim() || undefined, content: sanitizedHtml, photoUrls: newPhotoUrls.length > 0 ? newPhotoUrls : undefined } : null);
            // Scroll to top to show success message
            window.scrollTo(0, 0);
        } catch (err) {
            setSubmitError(err instanceof Error ? err.message : 'Failed to update page content');
        } finally {
            setIsSubmitting(false);
        }
    };

    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit Page Content: {content?.title || slug}</h1>
                <p className="text-[var(--foreground)]">Loading...</p>
            </div>
        );
    }

    if (loadError || !content) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit Page Content</h1>
                <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-black">{loadError || 'Page content not found'}</p>
                </div>
                <Link href="/admin/content/edit" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Edit Content
                </Link>
            </div>
        );
    }

    return (
        <Fragment>
            <UploadSpinnerModal isVisible={isUploading} />
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit: {content.title || content.slug}</h1>

                {submitError && (
                    <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                        <p className="text-black">Error: {submitError}</p>
                    </div>
                )}

                {submitSuccess && (
                    <div className="bg-green-200 border border-green-500 rounded-lg p-4 mb-6">
                        <p className="text-black">Page content updated successfully!</p>
                    </div>
                )}

                {/* Initial Content Version - Current state before edits */}
                {initialContentHtml && (
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 mb-6">
                        <h2 className="text-xl font-bold mb-4 text-[var(--title-color)]">Current Version (Before Edits)</h2>
                        <div
                            className="w-full px-3 py-2 bg-[var(--background)] border border-gray-600 rounded text-[var(--foreground)] text-sm font-normal min-h-[100px]"
                            style={{ fontFamily: 'var(--font-local)' }}
                            dangerouslySetInnerHTML={{ __html: initialContentHtml }}
                        />
                    </div>
                )}

                <form onSubmit={handleSubmit} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <div className="mb-4">
                        <label htmlFor="title" className="block text-sm font-medium text-[var(--foreground)] mb-2">
                            Title (optional)
                        </label>
                        <input
                            type="text"
                            id="title"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            className="w-full px-3 py-2 bg-[var(--background)] border border-gray-600 rounded text-[var(--foreground)] focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="Enter page title..."
                        />
                        {errors.title && (
                            <p className="text-red-400 text-sm mt-1">{errors.title}</p>
                        )}
                    </div>

                    <div className="mb-4">
                        <label className="block text-sm font-medium text-[var(--foreground)] mb-2">
                            Content (separate paragraphs with blank lines)
                        </label>
                        {/* Formatting toolbar */}
                        <div className="flex gap-2 mb-2">
                            <button
                                type="button"
                                onClick={() => document.execCommand('bold', false)}
                                className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-600 text-sm font-bold"
                                title="Bold"
                            >
                                Bold
                            </button>
                            <button
                                type="button"
                                onClick={() => document.execCommand('italic', false)}
                                className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-600 text-sm italic"
                                title="Italic"
                            >
                                Italic
                            </button>
                            <button
                                type="button"
                                onClick={() => document.execCommand('underline', false)}
                                className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-600 text-sm underline"
                                title="Underline"
                            >
                                Underline
                            </button>
                            <button
                                type="button"
                                onClick={() => document.execCommand('justifyLeft', false)}
                                className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-600 text-sm"
                                title="Align Left"
                            >
                                Left
                            </button>
                            <button
                                type="button"
                                onClick={() => document.execCommand('justifyCenter', false)}
                                className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-600 text-sm"
                                title="Align Center"
                            >
                                Center
                            </button>
                            <button
                                type="button"
                                onClick={() => document.execCommand('justifyRight', false)}
                                className="px-3 py-1 bg-gray-700 text-white rounded hover:bg-gray-600 text-sm"
                                title="Align Right"
                            >
                                Right
                            </button>
                        </div>
                        <div
                            ref={contentEditorRef}
                            contentEditable
                            suppressContentEditableWarning
                            className="w-full px-3 py-2 bg-[var(--background)] border border-gray-600 rounded text-[var(--foreground)] focus:outline-none focus:ring-2 focus:ring-blue-500 min-h-[300px] text-sm font-normal"
                            style={{ fontFamily: 'var(--font-local)' }}
                        />
                        {errors.content && (
                            <p className="text-red-400 text-sm mt-1">{errors.content}</p>
                        )}
                    </div>
                    {slug === 'about' && (
                        <div className="mb-4">
                            <h2 className="text-sm font-medium text-[var(--foreground)] mb-2">Photo (Only 1 photo allowed for About page)</h2>
                            <div className="flex justify-center relative w-full max-w-xs mx-auto h-48">
                                <Image src={photoUrls[0] || '/placeholder.jpg'} alt="Photo" fill className="rounded object-contain" unoptimized={(photoUrls[0] || '/placeholder.jpg').startsWith('/images/')} />
                            </div>
                            <input type="file" accept=".jpg,.jpeg" onChange={handleFileUpload} className="w-full text-[var(--foreground)] mt-2 file:mr-4 file:py-2 file:px-4 file:rounded file:border-0 file:text-sm file:font-semibold file:bg-[var(--button-color)] file:text-white hover:file:opacity-90" />
                            {errors.file && (
                                <p className="text-red-400 text-sm mt-1">{errors.file}</p>
                            )}
                        </div>
                    )}

                    <div className="flex gap-4">
                        <button
                            type="submit"
                            disabled={isSubmitting}
                            className="px-4 py-2 bg-[var(--button-color)] text-white rounded hover:opacity-90 transition-opacity font-bold disabled:opacity-50"
                        >
                            {isSubmitting ? 'Saving...' : 'Save Changes'}
                        </button>
                        <Link
                            href="/admin/content/edit"
                            onClick={() => window.scrollTo(0, 0)}
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
        </Fragment>
    );
}