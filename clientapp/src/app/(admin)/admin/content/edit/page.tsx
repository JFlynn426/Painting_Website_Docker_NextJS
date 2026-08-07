import Link from 'next/link';
import { getAllPageContents } from '@/lib/api';
import { PageContent } from '@/types/page-content';

// Force dynamic rendering to prevent static generation during Docker build
// when the API is unavailable
export const dynamic = 'force-dynamic';

export default async function EditContentPage() {
    let pageContents: PageContent[] = [];
    let error: string | null = null;

    try {
        const allContents = await getAllPageContents({ noCache: true });
        pageContents = allContents;
    } catch (err) {
        error = err instanceof Error ? err.message : 'Failed to fetch page contents';
    }

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Page Content</h1>

            {error && (
                <div className="bg-red-200 border border-red-500 rounded-lg p-4 mb-6">
                    <p className="text-black">Error: {error}</p>
                </div>
            )}

            {pageContents.length === 0 && !error ? (
                <p className="text-[var(--foreground)]">No page content items found.</p>
            ) : (
                <div className="space-y-4">
                    {pageContents.map((content) => {
                        const friendlyName = content.title || content.slug.charAt(0).toUpperCase() + content.slug.slice(1);
                        return (
                            <div key={content.id} className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 flex justify-between items-center">
                                <div>
                                    <h2 className="text-xl font-bold text-[var(--title-color)]">{friendlyName}</h2>
                                </div>
                                <Link
                                    href={`/admin/content/edit/${content.slug}`}
                                    className="flex-shrink-0 ml-4 px-4 py-2 bg-yellow-600 text-white rounded hover:bg-yellow-700 transition-colors text-sm font-bold"
                                >
                                    Edit
                                </Link>
                            </div>
                        );
                    })}
                </div>
            )}

            <div className="mt-6">
                <Link href="/admin/content" className="text-[var(--title-color)] hover:underline">
                    &larr; Back to Content Management
                </Link>
            </div>
        </div>
    );
}
