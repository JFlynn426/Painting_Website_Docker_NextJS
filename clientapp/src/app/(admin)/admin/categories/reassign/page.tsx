'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import Image from 'next/image';
import { getAllPaintings, getAllPaintingCategories } from '@/lib/api';
import { reassignPaintingsAction } from '@/actions/painting-actions';
import type { Painting, PaintingCategory } from '@/types/paintings';
import type { ReassignPaintingsRequest } from '@/lib/api';

export default function ReassignPaintingsPage() {
    const [allPaintings, setAllPaintings] = useState<Painting[]>([]);
    const [categories, setCategories] = useState<PaintingCategory[]>([]);
    const [selectedCategories, setSelectedCategories] = useState<Record<string, string>>({});
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);
    const [isSaving, setIsSaving] = useState(false);
    const [saveError, setSaveError] = useState<string | null>(null);
    const [saveSuccess, setSaveSuccess] = useState(false);
    const [savedCount, setSavedCount] = useState(0);
    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Paintings";
    const artworkLabelLower = artworkLabel.toLowerCase();

    useEffect(() => {
        async function loadData() {
            try {
                setLoading(true);
                const [paintings, cats] = await Promise.all([
                    getAllPaintings({ noCache: true }),
                    getAllPaintingCategories({ noCache: true })
                ]);
                // Filter out "new-paintings" category since it's special
                const filteredCategories = cats.filter(c => c.slug !== 'new-paintings');
                setAllPaintings(paintings);
                setCategories(filteredCategories);
            } catch (err) {
                setLoadError(err instanceof Error ? err.message : 'Failed to load data');
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, []);

    const handleCategoryChange = useCallback((paintingId: string, categorySlug: string) => {
        setSelectedCategories(prev => ({
            ...prev,
            [paintingId]: categorySlug
        }));
    }, []);

    // Group paintings by their current category
    const paintingsByCategory = allPaintings.reduce<Record<string, Painting[]>>((acc, painting) => {
        const categorySlug = painting.categorySlug;
        if (!acc[categorySlug]) {
            acc[categorySlug] = [];
        }
        acc[categorySlug].push(painting);
        return acc;
    }, {});

    // Get list of paintings that have been changed
    const changedPaintings = allPaintings.filter(painting =>
        selectedCategories[painting.id] && selectedCategories[painting.id] !== painting.categorySlug
    );

    // Group paintings being reassigned TO each category
    const incomingPaintingsByCategory = changedPaintings.reduce<Record<string, Painting[]>>((acc, painting) => {
        const targetSlug = selectedCategories[painting.id];
        if (targetSlug && !acc[targetSlug]) {
            acc[targetSlug] = [];
        }
        if (targetSlug) {
            acc[targetSlug].push(painting);
        }
        return acc;
    }, {});

    // Calculate outgoing paintings per category (paintings leaving this category)
    const outgoingPaintingsByCategory = changedPaintings.reduce<Record<string, number>>((acc, painting) => {
        acc[painting.categorySlug] = (acc[painting.categorySlug] || 0) + 1;
        return acc;
    }, {});

    // Calculate projected count per category after reassignment
    const projectedCounts = allPaintings.reduce<Record<string, number>>((acc, p) => {
        acc[p.categorySlug] = (acc[p.categorySlug] || 0) + 1;
        return acc;
    }, {});
    for (const painting of changedPaintings) {
        projectedCounts[painting.categorySlug] = (projectedCounts[painting.categorySlug] || 0) - 1;
        const targetSlug = selectedCategories[painting.id];
        if (targetSlug) {
            projectedCounts[targetSlug] = (projectedCounts[targetSlug] || 0) + 1;
        }
    }

    const handleSave = async () => {
        if (changedPaintings.length === 0) {
            return;
        }

        // Validate: check if any category would exceed 30 paintings after reassignment
        // Build a map of current counts per category
        const currentCounts = allPaintings.reduce<Record<string, number>>((acc, p) => {
            acc[p.categorySlug] = (acc[p.categorySlug] || 0) + 1;
            return acc;
        }, {});

        // Calculate final counts after reassignment
        const finalCounts = { ...currentCounts };
        for (const painting of changedPaintings) {
            // Decrement source category
            finalCounts[painting.categorySlug] = (finalCounts[painting.categorySlug] || 0) - 1;
            // Increment target category
            const targetSlug = selectedCategories[painting.id];
            if (targetSlug) {
                finalCounts[targetSlug] = (finalCounts[targetSlug] || 0) + 1;
            }
        }

        // Check for any category exceeding 30
        for (const [slug, count] of Object.entries(finalCounts)) {
            if (count > 30) {
                const category = categories.find(c => c.slug === slug);
                const categoryName = category?.name || slug;
                setSaveError(`Cannot save: "${categoryName}" would have ${count} paintings, which exceeds the maximum of 30 paintings per category.`);
                return;
            }
        }

        setIsSaving(true);
        setSaveError(null);
        setSaveSuccess(false);

        try {
            const paintingIdToCategoryId: Record<string, string> = {};
            for (const painting of changedPaintings) {
                const targetCategory = categories.find(c => c.slug === selectedCategories[painting.id]);
                if (targetCategory) {
                    paintingIdToCategoryId[painting.id] = targetCategory.id;
                }
            }

            const request: ReassignPaintingsRequest = {
                paintingIdToCategoryId
            };

            await reassignPaintingsAction(request);
            setSavedCount(changedPaintings.length);
            setSaveSuccess(true);
            window.scrollTo({ top: 0, behavior: 'smooth' });

            // Update local state to reflect saved changes
            setAllPaintings(prev =>
                prev.map(p => {
                    if (selectedCategories[p.id]) {
                        return { ...p, categorySlug: selectedCategories[p.id] };
                    }
                    return p;
                })
            );

            // Clear selected categories after save
            setSelectedCategories({});
        } catch (err) {
            setSaveError(err instanceof Error ? err.message : 'Failed to save changes');
        } finally {
            setIsSaving(false);
        }
    };

    if (loading) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Reassign {artworkLabel}</h1>
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <p className="text-[var(--foreground)]">Loading {artworkLabelLower} and categories...</p>
                </div>
            </div>
        );
    }

    if (loadError) {
        return (
            <div>
                <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Reassign {artworkLabel}</h1>
                <div className="bg-red-200 border border-red-500 rounded-lg p-6">
                    <p className="text-black">Error: {loadError}</p>
                </div>
            </div>
        );
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Reassign {artworkLabel}</h1>
            <p className="text-[var(--foreground)] mb-6">
                {artworkLabel} are displayed in their initial categories. Select the category to move the {artworkLabelLower} to by selecting a new category in the dropdown.
            </p>

            <div className="bg-yellow-100 border border-yellow-600 rounded-lg p-4 mb-6">
                <p className="text-black text-sm">
                    <strong>Note:</strong> Categories can have no more than 30 {artworkLabelLower} per page. It is suggested that each category should contain 8 or more {artworkLabelLower}.
                </p>
            </div>

            {saveSuccess && (
                <div className="bg-green-200 border border-green-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Changes saved successfully! ({savedCount} {artworkLabelLower} reassigned)</p>
                </div>
            )}

            {saveError && (
                <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Error: {saveError}</p>
                </div>
            )}

            {/* Paintings organized by category */}
            <div className="space-y-8">
                {categories.map((category) => {
                    const categorySlug = category.slug;
                    const paintings = paintingsByCategory[categorySlug] || [];

                    const projectedCount = projectedCounts[categorySlug] || 0;
                    const hasChanges = incomingPaintingsByCategory[categorySlug]?.length > 0 || outgoingPaintingsByCategory[categorySlug] > 0;

                    return (
                        <div key={categorySlug}>
                            <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                                {category.name} ({paintings.length})
                            </h2>
                            {hasChanges && (
                                <p className="text-sm mb-2">
                                    <span className="text-[var(--foreground)]">Projected after changes: </span>
                                    <span className={projectedCount > 30 ? 'text-red-700 font-bold' : projectedCount < 8 ? 'text-yellow-700 font-bold' : 'text-green-700 font-bold'}>
                                        {projectedCount}
                                    </span>
                                    {projectedCount > 30 && (
                                        <span className="text-red-700 ml-2">⚠ Error: Exceeds maximum of 30 {artworkLabelLower}</span>
                                    )}
                                    {projectedCount < 8 && projectedCount <= 30 && (
                                        <span className="text-yellow-700 ml-2">⚠ Warning: Fewer than 8 {artworkLabelLower} suggested</span>
                                    )}
                                </p>
                            )}
                            {!hasChanges && paintings.length < 8 && (
                                <div className="bg-yellow-100 border border-yellow-600 rounded-lg p-3 mb-3">
                                    <p className="text-black text-sm">
                                        <strong>Warning:</strong> This category has fewer than 8 {artworkLabelLower}. It is suggested that each category should contain 8 or more {artworkLabelLower}.
                                    </p>
                                </div>
                            )}
                            {paintings.length === 0 ? (
                                <div className="bg-yellow-100 border border-yellow-600 rounded-lg p-4">
                                    <p className="text-black text-sm">
                                        <strong>No paintings assigned to this category.</strong> Assign paintings from other categories above to add them here.
                                    </p>
                                </div>
                            ) : (
                                <div className="grid lg:grid-cols-6 gap-4">
                                    {paintings.map((painting) => {
                                        const hasChanged = selectedCategories[painting.id] && selectedCategories[painting.id] !== painting.categorySlug;
                                        return (
                                            <div
                                                key={painting.id}
                                                className={`bg-[var(--navbar-footer-bg)] rounded-lg p-2 ${hasChanged ? 'ring-2 ring-red-500' : ''
                                                    }`}
                                            >
                                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                                    <Image
                                                        src={painting.thumbnailUrl || painting.imageUrl}
                                                        alt={painting.title}
                                                        fill
                                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 33vw, 25vw"
                                                        className="object-cover"
                                                        unoptimized={(painting.thumbnailUrl || painting.imageUrl).startsWith('/images/')}
                                                    />
                                                </div>
                                                <p className="text-xs text-[var(--foreground)] truncate mb-2">
                                                    {painting.title}
                                                </p>
                                                <select
                                                    value={selectedCategories[painting.id] || painting.categorySlug}
                                                    onChange={(e) => handleCategoryChange(painting.id, e.target.value)}
                                                    className="w-full text-xs bg-[var(--admin-hover)] rounded px-2 py-1 border border-gray-600 focus:border-blue-500 focus:outline-none"
                                                >
                                                    {categories.map((cat) => (
                                                        <option key={cat.slug} value={cat.slug}>
                                                            {cat.name}
                                                        </option>
                                                    ))}
                                                </select>
                                            </div>
                                        );
                                    })}
                                </div>
                            )}

                            {/* Incoming paintings from other categories */}
                            {incomingPaintingsByCategory[categorySlug] && incomingPaintingsByCategory[categorySlug].length > 0 && (
                                <div className="mt-4">
                                    <h3 className="text-sm font-semibold mb-2 text-blue-400">
                                        Incoming from other categories ({incomingPaintingsByCategory[categorySlug].length})
                                    </h3>
                                    <div className="grid lg:grid-cols-6 gap-4">
                                        {incomingPaintingsByCategory[categorySlug].map((painting) => (
                                            <div
                                                key={`incoming-${painting.id}`}
                                                className="bg-[var(--navbar-footer-bg)] rounded-lg p-2 ring-2 ring-blue-500"
                                            >
                                                <div className="aspect-square mb-1 overflow-hidden rounded relative">
                                                    <Image
                                                        src={painting.thumbnailUrl || painting.imageUrl}
                                                        alt={painting.title}
                                                        fill
                                                        sizes="(max-width: 768px) 50vw, (max-width: 1024px) 33vw, 25vw"
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
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>

            {/* Changes List and Save Button */}
            {changedPaintings.length > 0 && (
                <div className="mt-8 bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <h2 className="text-xl font-semibold mb-3 text-[var(--title-color)]">
                        Pending Changes ({changedPaintings.length})
                    </h2>
                    <div className="max-h-60 overflow-y-auto mb-4">
                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b border-gray-600">
                                    <th className="text-left py-2 px-3 text-[var(--title-color)]">Painting</th>
                                    <th className="text-left py-2 px-3 text-[var(--title-color)]">Current Category</th>
                                    <th className="text-left py-2 px-3 text-[var(--title-color)]">New Category</th>
                                </tr>
                            </thead>
                            <tbody>
                                {changedPaintings.map((painting) => {
                                    const currentCategory = categories.find(c => c.slug === painting.categorySlug);
                                    const newCategory = categories.find(c => c.slug === selectedCategories[painting.id]);
                                    return (
                                        <tr key={painting.id} className="border-b border-gray-700">
                                            <td className="py-2 px-3 text-[var(--foreground)]">{painting.title}</td>
                                            <td className="py-2 px-3 text-[var(--foreground)]">{currentCategory?.name || painting.categorySlug}</td>
                                            <td className="py-2 px-3 text-yellow-400">{newCategory?.name || selectedCategories[painting.id]}</td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>
                    <button
                        onClick={handleSave}
                        disabled={isSaving}
                        className="px-6 py-2 bg-[var(--button-color)] text-white rounded hover:opacity-90 transition-opacity disabled:opacity-50"
                    >
                        {isSaving ? 'Saving...' : `Save Changes (${changedPaintings.length})`}
                    </button>
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
