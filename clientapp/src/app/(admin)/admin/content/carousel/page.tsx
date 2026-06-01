'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { getAllPaintings, updatePainting } from '@/lib/api';
import type { Painting } from '@/types/paintings';

export default function CarouselContentPage() {
    const [allPaintings, setAllPaintings] = useState<Painting[]>([]);
    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);
    const [isSaving, setIsSaving] = useState(false);
    const [saveError, setSaveError] = useState<string | null>(null);
    const [saveSuccess, setSaveSuccess] = useState(false);

    useEffect(() => {
        async function loadPaintings() {
            try {
                setLoading(true);
                const paintings = await getAllPaintings();
                setAllPaintings(paintings);
                // Initialize selected IDs from paintings that currently have isCarouselPainting=true
                const currentCarouselIds = new Set<string>();
                paintings.forEach(p => {
                    if (p.isCarouselPainting) {
                        currentCarouselIds.add(p.id);
                    }
                });
                setSelectedIds(currentCarouselIds);
            } catch (err) {
                setLoadError(err instanceof Error ? err.message : 'Failed to load paintings');
            } finally {
                setLoading(false);
            }
        }
        loadPaintings();
    }, []);

    const togglePainting = useCallback((id: string) => {
        setSelectedIds(prev => {
            const next = new Set(prev);
            if (next.has(id)) {
                next.delete(id);
            } else {
                next.add(id);
            }
            return next;
        });
    }, []);

    const currentCarouselPaintings = allPaintings.filter(p => p.isCarouselPainting);
    const updatedCarouselPaintings = allPaintings.filter(p => selectedIds.has(p.id));
    const isValidCount = updatedCarouselPaintings.length >= 3 && updatedCarouselPaintings.length <= 8;

    const handleSave = async () => {
        if (updatedCarouselPaintings.length < 3 || updatedCarouselPaintings.length > 8) {
            setSaveError('You must select between 3 and 8 carousel paintings.');
            return;
        }

        setIsSaving(true);
        setSaveError(null);
        setSaveSuccess(false);

        try {
            const promises = allPaintings.map(async (painting) => {
                const shouldBeCarousel = selectedIds.has(painting.id);
                if (shouldBeCarousel !== painting.isCarouselPainting) {
                    await updatePainting(painting.id, { isCarouselPainting: shouldBeCarousel });
                }
            });

            await Promise.all(promises);
            setSaveSuccess(true);

            // Update local state to reflect saved changes
            setAllPaintings(prev =>
                prev.map(p => ({ ...p, isCarouselPainting: selectedIds.has(p.id) }))
            );
        } catch (err) {
            setSaveError(err instanceof Error ? err.message : 'Failed to save changes');
        } finally {
            setIsSaving(false);
        }
    };

    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Change Carousel Images</h1>
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <p className="text-gray-400">Loading paintings...</p>
                </div>
            </div>
        );
    }

    if (loadError) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Change Carousel Images</h1>
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-6">
                    <p className="text-red-200">Error: {loadError}</p>
                </div>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Change Carousel Images</h1>
            <p className="text-gray-400 mb-4">
                This page allows you to select the paintings which are displayed in the homepage carousel.
            </p>

            <div className="bg-yellow-900 bg-opacity-30 border border-yellow-600 rounded-lg p-4 mb-6">
                <p className="text-yellow-200 text-sm">
                    <strong>Note:</strong> The carousel requires between 3 and 8 paintings to be selected.
                </p>
            </div>

            {saveSuccess && (
                <div className="bg-green-900 bg-opacity-50 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-green-200">Changes saved successfully! ({updatedCarouselPaintings.length} paintings selected)</p>
                </div>
            )}

            {saveError && (
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">Error: {saveError}</p>
                </div>
            )}

            {/* Current Carousel Paintings */}
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    Current Carousel Paintings ({currentCarouselPaintings.length})
                </h2>
                {currentCarouselPaintings.length === 0 ? (
                    <p className="text-gray-400">No paintings are currently set for the carousel.</p>
                ) : (
                    <div className="grid lg:grid-cols-6 gap-4">
                        {currentCarouselPaintings.map((painting) => (
                            <div
                                key={painting.id}
                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2"
                            >
                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                    <Image
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        fill
                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 25vw, 16vw"
                                        className="object-cover"
                                    />
                                </div>
                                <p className="text-xs text-white truncate">
                                    {painting.title}
                                </p>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Updated Carousel Paintings Preview */}
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    Updated Carousel Paintings ({updatedCarouselPaintings.length})
                </h2>
                {updatedCarouselPaintings.length === 0 ? (
                    <p className="text-gray-400">No paintings selected yet. Click on paintings below to add them.</p>
                ) : (
                    <div className="grid grid-cols-2 lg:grid-cols-6 gap-4">
                        {updatedCarouselPaintings.map((painting) => (
                            <div
                                key={painting.id}
                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2"
                            >
                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                    <Image
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        fill
                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 25vw, 16vw"
                                        className="object-cover"
                                    />
                                </div>
                                <p className="text-xs text-white truncate">
                                    {painting.title}
                                </p>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Save Button */}
            <div className="mb-8">
                <button
                    onClick={handleSave}
                    disabled={isSaving || !isValidCount}
                    className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors disabled:opacity-50"
                >
                    {isSaving ? 'Saving...' : 'Save Selection'}
                </button>
                {updatedCarouselPaintings.length > 0 && updatedCarouselPaintings.length < 3 && (
                    <p className="text-yellow-400 text-sm mt-2">
                        Warning: You must select between 3 and 8 carousel paintings. You have selected {updatedCarouselPaintings.length}.
                    </p>
                )}
                {updatedCarouselPaintings.length > 8 && (
                    <p className="text-yellow-400 text-sm mt-2">
                        Warning: You must select between 3 and 8 carousel paintings. You have selected {updatedCarouselPaintings.length}.
                    </p>
                )}
            </div>

            {/* All Paintings */}
            <div>
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    All Paintings ({allPaintings.length})
                </h2>
                <p className="text-gray-400 text-sm mb-4">
                    Click on a painting to toggle it as a {'"'}Carousel Painting{'"'}. Selected paintings are highlighted with a blue border.
                </p>
                <div className="grid grid-cols-2 lg:grid-cols-6 gap-4">
                    {allPaintings.map((painting) => {
                        const isSelected = selectedIds.has(painting.id);
                        return (
                            <button
                                key={painting.id}
                                onClick={() => togglePainting(painting.id)}
                                className={`bg-[var(--navbar-footer-bg)] rounded-lg p-2 transition-colors group cursor-pointer ${isSelected
                                    ? 'ring-2 ring-blue-500'
                                    : 'hover:bg-[var(--background)]'
                                    }`}
                            >
                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                    <Image
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        fill
                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 25vw, 16vw"
                                        className="object-cover group-hover:opacity-80 transition-opacity"
                                    />
                                </div>
                                <p className={`text-xs truncate group-hover:text-[var(--title-color)] transition-colors ${isSelected ? 'text-[var(--title-color)]' : 'text-white'
                                    }`}>
                                    {painting.title}
                                </p>
                            </button>
                        );
                    })}
                </div>
            </div>

            <div className="mt-6">
                <Link href="/admin/content" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Content Management
                </Link>
            </div>
        </div>
    );
}
