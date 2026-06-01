import Link from 'next/link';
import Image from 'next/image';
import { getPaintingsByCategory } from '@/lib/api';
import { PaintingCategoryWithPaintings } from '@/types';

// Force dynamic rendering to prevent static generation during Docker build
export const dynamic = 'force-dynamic';

interface EditCategoryPaintingsPageProps {
    params: Promise<{ categorySlug: string }>;
}

export default async function EditCategoryPaintingsPage({ params }: EditCategoryPaintingsPageProps) {
    const { categorySlug } = await params;

    let category: PaintingCategoryWithPaintings | null = null;
    let error: string | null = null;

    try {
        category = await getPaintingsByCategory(categorySlug);
    } catch (err) {
        error = err instanceof Error ? err.message : 'Failed to fetch category paintings';
    }

    const categoryName = categorySlug.split('-').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');

    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">
                Edit Paintings in {categoryName} Category
            </h1>
            <p className="text-gray-400 mb-6 text-sm">Please select a painting to edit</p>

            {error && (
                <div className="bg-red-900 bg-opacity-50 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-red-200">Error: {error}</p>
                </div>
            )}

            {category && category.paintings.length === 0 ? (
                <p className="text-gray-400">No paintings found in this category.</p>
            ) : category ? (
                <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                    {category.paintings.map((painting) => (
                        <Link
                            key={painting.id}
                            href={`/admin/paintings/edit/${categorySlug}/${painting.slug}`}
                            className="bg-[var(--navbar-footer-bg)] rounded-lg p-3 hover:bg-[var(--background)] transition-colors group"
                        >
                            <div className="aspect-square mb-2 overflow-hidden rounded relative">
                                <Image
                                    src={painting.thumbnailUrl || painting.imageUrl}
                                    alt={painting.title}
                                    fill
                                    sizes="(max-width: 768px) 50vw, (max-width: 1024px) 33vw, 25vw"
                                    className="object-cover group-hover:opacity-80 transition-opacity"
                                />
                            </div>
                            <p className="text-sm text-white truncate group-hover:text-[var(--title-color)] transition-colors">
                                {painting.title}
                            </p>
                        </Link>
                    ))}
                </div>
            ) : null}

            <div className="mt-6">
                <Link href="/admin/paintings/edit" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Edit Categories
                </Link>
            </div>
        </div>
    );
}
