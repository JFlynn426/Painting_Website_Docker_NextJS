'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import { getAllPaintings, updatePainting } from '@/lib/api';
import type { Painting } from '@/types/paintings';

export default function SelectNewPaintingsPage() {
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
                // Initialize selected IDs from paintings that currently have isNew=true
                const currentNewIds = new Set<string>();
                paintings.forEach(p => {
                    if (p.isNew) {
                        currentNewIds.add(p.id);
                    }
                });
                setSelectedIds(currentNewIds);
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

    const currentNewPaintings = allPaintings.filter(p => p.isNew);
    const updatedNewPaintings = allPaintings.filter(p => selectedIds.has(p.id));

    const handleSave = async () => {
        setIsSaving(true);
        setSaveError(null);
        setSaveSuccess(false);

        try {
            const promises = allPaintings.map(async (painting) => {
                const shouldBeNew = selectedIds.has(painting.id);
                if (shouldBeNew !== painting.isNew) {
                    await updatePainting(painting.id, { isNew: shouldBeNew });
                }
            });

            await Promise.all(promises);
            setSaveSuccess(true);

            // Update local state to reflect saved changes
            setAllPaintings(prev =>
                prev.map(p => ({ ...p, isNew: selectedIds.has(p.id) }))
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
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Select New Paintings</h1>
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <p className="text-gray-400">Loading paintings...</p>
                </div>
            </div>
        );
    }

    if (loadError) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Select New Paintings</h1>
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-6">
                    <p className="text-red-200">Error: {loadError}</p>
                </div>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Select New Paintings</h1>
            <p className="text-gray-400 mb-6">
                This page allows you to select the paintings which are displayed in the New Paintings page.
                It should contain 10-20 of the newest and/or best paintings that will be featured in this section.
            </p>

            {saveSuccess && (
                <div className="bg-green-900 bg-opacity-50 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-green-200">Changes saved successfully! ({updatedNewPaintings.length} paintings selected)</p>
                </div>
            )}

            {saveError && (
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">Error: {saveError}</p>
                </div>
            )}

            {/* Current New Paintings */}
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    Current New Paintings ({currentNewPaintings.length})
                </h2>
                {currentNewPaintings.length === 0 ? (
                    <p className="text-gray-400">No paintings are currently marked as new.</p>
                ) : (
                    <div className="grid grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
                        {currentNewPaintings.map((painting) => (
                            <div
                                key={painting.id}
                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2"
                            >
                                <div className="aspect-square mb-1 overflow-hidden rounded">
                                    <img
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        className="w-full h-full object-cover"
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

            {/* Updated New Paintings Preview */}
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    Updated New Paintings ({updatedNewPaintings.length})
                </h2>
                {updatedNewPaintings.length === 0 ? (
                    <p className="text-gray-400">No paintings selected yet. Click on paintings below to add them.</p>
                ) : (
                    <div className="grid grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
                        {updatedNewPaintings.map((painting) => (
                            <div
                                key={painting.id}
                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2"
                            >
                                <div className="aspect-square mb-1 overflow-hidden rounded">
                                    <img
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        className="w-full h-full object-cover"
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
                    disabled={isSaving}
                    className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors disabled:opacity-50"
                >
                    {isSaving ? 'Saving...' : 'Save Selection'}
                </button>
                {updatedNewPaintings.length > 0 && updatedNewPaintings.length < 10 && (
                    <p className="text-yellow-400 text-sm mt-2">
                        Warning: You have selected only {updatedNewPaintings.length} painting{updatedNewPaintings.length !== 1 ? 's' : ''}. Consider selecting 10-20 paintings.
                    </p>
                )}
                {updatedNewPaintings.length > 20 && (
                    <p className="text-yellow-400 text-sm mt-2">
                        Warning: You have selected {updatedNewPaintings.length} paintings. Consider limiting to 10-20.
                    </p>
                )}
            </div>

            {/* All Paintings */}
            <div>
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    All Paintings ({allPaintings.length})
                </h2>
                <p className="text-gray-400 text-sm mb-4">
                    Click on a painting to toggle it as a {'"'}New Painting{'"'}. Selected paintings are highlighted with a blue border.
                </p>
                <div className="grid grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
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
                                <div className="aspect-square mb-1 overflow-hidden rounded">
                                    <img
                                        src={painting.thumbnailUrl || painting.imageUrl}
                                        alt={painting.title}
                                        className="w-full h-full object-cover group-hover:opacity-80 transition-opacity"
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
                <Link href="/admin/paintings" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Paintings Admin
                </Link>
            </div>
        </div>
    );
}
