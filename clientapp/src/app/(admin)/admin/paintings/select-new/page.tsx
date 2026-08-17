'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { getAllPaintings } from '@/lib/api';
import { updatePaintingAction } from '@/actions/painting-actions';
import type { Painting } from '@/types/paintings';

export default function SelectNewPaintingsPage() {
    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Painting";
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLower = artworkLabel.toLowerCase();
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();
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
                const paintings = await getAllPaintings({ noCache: true });
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
                setLoadError(err instanceof Error ? err.message : `Failed to load ${artworkLabelLowerPlural}`);
            } finally {
                setLoading(false);
            }
        }
        loadPaintings();
    }, [artworkLabelLowerPlural]);

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

        // Validate: check minimum (8) and maximum (30) paintings
        if (selectedIds.size < 8) {
            setSaveError(`Cannot save: At least 8 ${artworkLabelLowerPlural} must be selected. Please select more ${artworkLabelLowerPlural}.`);
            setIsSaving(false);
            return;
        }
        if (selectedIds.size > 30) {
            setSaveError(`Cannot save: Maximum of 30 ${artworkLabelLowerPlural} allowed. Please deselect some ${artworkLabelLowerPlural}.`);
            setIsSaving(false);
            return;
        }

        try {
            const promises = allPaintings.map(async (painting) => {
                const shouldBeNew = selectedIds.has(painting.id);
                if (shouldBeNew !== painting.isNew) {
                    await updatePaintingAction(painting.id, { isNew: shouldBeNew });
                }
            });

            await Promise.all(promises);
            setSaveSuccess(true);
            window.scrollTo(0, 0);

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
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Select New {artworkLabelPlural}</h1>
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <p className="text-[var(--foreground)]">Loading {artworkLabelLowerPlural}...</p>
                </div>
            </div>
        );
    }

    if (loadError) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Select New {artworkLabelPlural}</h1>
                <div className="bg-red-200 border border-red-500 rounded-lg p-6">
                    <p className="text-black">Error: {loadError}</p>
                </div>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Select New {artworkLabelPlural}</h1>
            <p className="text-[var(--foreground)] mb-6">
                This page allows you to select the {artworkLabelLowerPlural} displayed in the New {artworkLabelPlural} page.
            </p>

            <div className="bg-yellow-100 border border-yellow-600 rounded-lg p-4 mb-6">
                <p className="text-black text-sm">
                    <strong>Note:</strong> The New {artworkLabelPlural} section can have no more than 30 {artworkLabelLowerPlural}. It is suggested that this section should contain 8 or more {artworkLabelLowerPlural}.
                </p>
            </div>

            {saveSuccess && (
                <div className="bg-green-200 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Changes saved successfully! ({updatedNewPaintings.length} {artworkLabelLowerPlural} selected)</p>
                </div>
            )}

            {saveError && (
                <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Error: {saveError}</p>
                </div>
            )}

            {/* Current New Paintings */}
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    Current New {artworkLabelPlural} ({currentNewPaintings.length})
                </h2>
                {currentNewPaintings.length === 0 ? (
                    <p className="text-[var(--foreground)]">No {artworkLabelLowerPlural} currently marked as new.</p>
                ) : (
                    <div className="grid lg:grid-cols-6 gap-4">
                        {currentNewPaintings.map((painting) => (
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
                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
                                    />
                                </div>
                                <p className="text-xs text-[var(--foreground)] truncate">
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
                    Updated New {artworkLabelPlural} ({updatedNewPaintings.length})
                </h2>
                {updatedNewPaintings.length === 0 ? (
                    <p className="text-[var(--foreground)]">No {artworkLabelLowerPlural} selected yet. Click on {artworkLabelLowerPlural} below to add them.</p>
                ) : (
                    <div className="grid grid-cols-2 lg:grid-cols-6 gap-4">
                        {updatedNewPaintings.map((painting) => (
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
                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
                                    />
                                </div>
                                <p className="text-xs text-[var(--foreground)] truncate">
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
                    className="px-6 py-2 bg-[var(--button-color)] text-white rounded hover:opacity-90 transition-opacity disabled:opacity-50"
                >
                    {isSaving ? 'Saving...' : 'Save Selection'}
                </button>
                {updatedNewPaintings.length > 0 && updatedNewPaintings.length < 8 && (
                    <p className="text-[var(--foreground)] text-sm mt-2">
                        ⚠ Warning: Fewer than 8 {artworkLabelLowerPlural} selected. It is suggested that this section should contain 8 or more {artworkLabelLowerPlural}.
                    </p>
                )}
                {updatedNewPaintings.length > 30 && (
                    <p className="text-[var(--foreground)] text-sm mt-2">
                        ⚠ Error: Exceeds maximum of 30 {artworkLabelLowerPlural}.
                    </p>
                )}
            </div>

            {/* All Paintings */}
            <div>
                <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                    All {artworkLabelPlural} ({allPaintings.length})
                </h2>
                <p className="text-[var(--foreground)] text-sm mb-4">
                    Click on {artworkLabelLower} to toggle it as new. Selected {artworkLabelLowerPlural} highlighted with a blue border.
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
                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
                                    />
                                </div>
                                <p className={`text-xs truncate group-hover:text-[var(--title-color)] transition-colors ${isSelected ? 'text-[var(--title-color)]' : 'text-[var(--foreground)]'
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
                    &larr; Back to {artworkLabelPlural} Admin
                </Link>
            </div>
        </div>
    );
}
