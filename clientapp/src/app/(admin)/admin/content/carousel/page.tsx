'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { getAllPaintings, getPageContent } from '@/lib/api';
import { updatePageContentAction } from '@/actions/page-content-actions';
import type { Painting } from '@/types/paintings';
import type { PageContent } from '@/types/page-content';

export default function CarouselContentPage() {
    const [allPaintings, setAllPaintings] = useState<Painting[]>([]);
    const [currentCarouselPaintings, setCurrentCarouselPaintings] = useState<Painting[]>([]);
    const [selectedPaintings, setSelectedPaintings] = useState<Painting[]>([]);
    const [homeContent, setHomeContent] = useState<PageContent | null>(null);
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);
    const [isSaving, setIsSaving] = useState(false);
    const [saveError, setSaveError] = useState<string | null>(null);
    const [saveSuccess, setSaveSuccess] = useState(false);

    useEffect(() => {
        async function loadData() {
            try {
                setLoading(true);
                const [paintings, homePage] = await Promise.all([
                    getAllPaintings({ noCache: true }),
                    getPageContent('home', { noCache: true })
                ]);
                setAllPaintings(paintings);
                setHomeContent(homePage);

                // Initialize current and selected paintings from existing photoUrls
                // photoUrls store thumbnailUrl paths, so match against thumbnailUrl
                // Preserve the order from photoUrls
                if (homePage?.photoUrls) {
                    const current = homePage.photoUrls
                        .map(url => paintings.find(p => p.thumbnailUrl === url))
                        .filter((p): p is Painting => p !== undefined);
                    setCurrentCarouselPaintings(current);
                    setSelectedPaintings(current);
                }
            } catch (err) {
                setLoadError(err instanceof Error ? err.message : 'Failed to load data');
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, []);

    const togglePainting = useCallback((painting: Painting) => {
        setSelectedPaintings(prev => {
            const exists = prev.find(p => p.id === painting.id);
            if (exists) {
                return prev.filter(p => p.id !== painting.id);
            }
            return [...prev, painting];
        });
    }, []);

    const moveUp = useCallback((index: number) => {
        if (index === 0) return;
        setSelectedPaintings(prev => {
            const newOrder = [...prev];
            [newOrder[index - 1], newOrder[index]] = [newOrder[index], newOrder[index - 1]];
            return newOrder;
        });
    }, []);

    const moveDown = useCallback((index: number) => {
        setSelectedPaintings(prev => {
            if (index >= prev.length - 1) return prev;
            const newOrder = [...prev];
            [newOrder[index], newOrder[index + 1]] = [newOrder[index + 1], newOrder[index]];
            return newOrder;
        });
    }, []);

    const isValidCount = selectedPaintings.length >= 3 && selectedPaintings.length <= 8;

    const handleSave = async () => {
        if (selectedPaintings.length < 3 || selectedPaintings.length > 8) {
            setSaveError('You must select between 3 and 8 carousel paintings.');
            return;
        }

        if (!homeContent?.id) {
            setSaveError('Home page content not found.');
            return;
        }

        setIsSaving(true);
        setSaveError(null);
        setSaveSuccess(false);

        try {
            // Use thumbnailUrl for photoUrls since that's what the carousel displays
            const photoUrls = selectedPaintings.map(p => p.thumbnailUrl || p.imageUrl);
            await updatePageContentAction(homeContent.id, { photoUrls });
            setSaveSuccess(true);
            // Update current carousel paintings to reflect saved changes
            setCurrentCarouselPaintings(selectedPaintings);
            // Scroll to top of page
            window.scrollTo(0, 0);
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
            <p className="text-gray-400 mb-6">
                This page allows you to select and order the paintings displayed in the homepage carousel.
            </p>

            <div className="bg-yellow-900 bg-opacity-30 border border-yellow-600 rounded-lg p-4 mb-6">
                <p className="text-yellow-200 text-sm">
                    <strong>Note:</strong> The carousel requires between 3 and 8 paintings to be selected. Order determines display sequence.
                </p>
                <p className="text-yellow-200 text-sm mt-1">
                    <strong>Requirement:</strong> Only paintings that are wider than they are tall (landscape orientation) can be added to the carousel.
                </p>
            </div>

            {saveSuccess && (
                <div className="bg-green-900 bg-opacity-50 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-green-200">Changes saved successfully! ({selectedPaintings.length} paintings selected)</p>
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
                        {currentCarouselPaintings.map((painting, index) => (
                            <div
                                key={painting.id}
                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2 relative"
                            >
                                <span className="absolute top-2 left-2 bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs font-bold z-10">
                                    {index + 1}
                                </span>
                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                    <Image
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        fill
                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 25vw, 16vw"
                                        className="object-cover"
                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
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
                    Updated Carousel Paintings ({selectedPaintings.length})
                </h2>
                {selectedPaintings.length === 0 ? (
                    <p className="text-gray-400">No paintings selected yet. Click on paintings below to add them.</p>
                ) : (
                    <div className="grid grid-cols-2 lg:grid-cols-6 gap-4">
                        {selectedPaintings.map((painting, index) => (
                            <div
                                key={painting.id}
                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2 relative"
                            >
                                <span className="absolute top-2 left-2 bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs font-bold z-10">
                                    {index + 1}
                                </span>
                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                    <Image
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        fill
                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 25vw, 16vw"
                                        className="object-cover"
                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
                                    />
                                </div>
                                <p className="text-xs text-white truncate mb-1">
                                    {painting.title}
                                </p>
                                <div className="flex gap-1">
                                    <button
                                        onClick={() => moveUp(index)}
                                        disabled={index === 0}
                                        className="flex-1 px-1 py-0.5 text-xs bg-gray-600 text-white rounded hover:bg-gray-500 disabled:opacity-30"
                                        title="Move up"
                                    >
                                        ↑
                                    </button>
                                    <button
                                        onClick={() => moveDown(index)}
                                        disabled={index >= selectedPaintings.length - 1}
                                        className="flex-1 px-1 py-0.5 text-xs bg-gray-600 text-white rounded hover:bg-gray-500 disabled:opacity-30"
                                        title="Move down"
                                    >
                                        ↓
                                    </button>
                                    <button
                                        onClick={() => togglePainting(painting)}
                                        className="px-1 py-0.5 text-xs bg-red-600 text-white rounded hover:bg-red-500"
                                        title="Remove"
                                    >
                                        ×
                                    </button>
                                </div>
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
                {selectedPaintings.length > 0 && selectedPaintings.length < 3 && (
                    <p className="text-yellow-400 text-sm mt-2">
                        Warning: You must select between 3 and 8 carousel paintings. You have selected {selectedPaintings.length}.
                    </p>
                )}
                {selectedPaintings.length > 8 && (
                    <p className="text-yellow-400 text-sm mt-2">
                        Warning: You must select between 3 and 8 carousel paintings. You have selected {selectedPaintings.length}.
                    </p>
                )}
            </div>

            {/* All Paintings */}
            <div>
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    Landscape Paintings ({allPaintings.filter(p => p.isLandscape).length})
                </h2>
                <p className="text-gray-400 text-sm mb-4">
                    Click on a painting to toggle it in the carousel. Selected paintings are highlighted with a blue border.
                </p>
                <div className="grid grid-cols-2 lg:grid-cols-6 gap-4">
                    {allPaintings.filter(p => p.isLandscape).map((painting) => {
                        const isSelected = selectedPaintings.find(p => p.id === painting.id);
                        return (
                            <button
                                key={painting.id}
                                onClick={() => togglePainting(painting)}
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
                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
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
