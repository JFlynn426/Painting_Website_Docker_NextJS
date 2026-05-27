import Link from 'next/link';
import { getAllPaintingCategories } from '@/lib/api';
import { PaintingCategory } from '@/types';

// Force dynamic rendering to prevent static generation during Docker build
// when the API is unavailable
export const dynamic = 'force-dynamic';

export default async function DeletePaintingsPage() {
    let categories: PaintingCategory[] = [];
    let error: string | null = null;

    try {
        const allCategories = await getAllPaintingCategories();
        // Exclude "New Paintings" - it's a special category, not for deleting paintings
        categories = allCategories.filter(c => c.slug !== 'new-paintings');
    } catch (err) {
        error = err instanceof Error ? err.message : 'Failed to fetch categories';
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Delete Paintings From Categories</h1>
            <p className="text-gray-400 mb-6">
                Select a category below to view its paintings. You will be able to permanently delete individual paintings from the database.
            </p>

            {error && (
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">Error: {error}</p>
                </div>
            )}

            {categories.length === 0 && !error ? (
                <p className="text-gray-400">No categories found.</p>
            ) : (
                <div className="space-y-4">
                    {categories.map((category) => (
                        <div key={category.id} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 flex justify-between items-center">
                            <div>
                                <h2 className="text-xl font-bold text-[var(--title-color)]">{category.name}</h2>
                                {category.description && (
                                    <p className="text-gray-400">{category.description}</p>
                                )}
                            </div>
                            <Link
                                href={`/admin/paintings/delete/${category.slug}`}
                                className="flex-shrink-0 ml-4 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors text-sm font-bold"
                            >
                                Delete Paintings
                            </Link>
                        </div>
                    ))}
                </div>
            )}

            <div className="mt-6">
                <Link href="/admin/paintings" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Paintings
                </Link>
            </div>
        </div>
    );
}
