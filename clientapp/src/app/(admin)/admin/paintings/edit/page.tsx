import Link from 'next/link';
import { getAllPaintingCategories } from '@/lib/api';
import { PaintingCategory } from '@/types';

// Force dynamic rendering to prevent static generation during Docker build
// when the API is unavailable
export const dynamic = 'force-dynamic';

export default async function EditPaintingsPage() {
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    let categories: PaintingCategory[] = [];
    let error: string | null = null;

    try {
        const allCategories = await getAllPaintingCategories({ noCache: true });
        // Exclude "New Paintings" - it's a special category, not for editing paintings
        categories = allCategories.filter(c => c.slug !== 'new-paintings');
    } catch (err) {
        error = err instanceof Error ? err.message : 'Failed to fetch categories';
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit {artworkLabelPlural} In Existing Categories</h1>

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
                            </div>
                            <Link
                                href={`/admin/paintings/edit/${category.slug}`}
                                className="flex-shrink-0 ml-4 px-4 py-2 bg-yellow-600 text-white rounded hover:bg-yellow-700 transition-colors text-sm font-bold"
                            >
                                Edit
                            </Link>
                        </div>
                    ))}
                </div>
            )}

            <div className="mt-6">
                <Link href="/admin/paintings" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to {artworkLabelPlural}
                </Link>
            </div>
        </div>
    );
}
