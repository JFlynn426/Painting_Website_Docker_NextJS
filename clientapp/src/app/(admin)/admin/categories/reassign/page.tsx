import Link from 'next/link';

export default function ReassignPaintingsPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Reassign Paintings Between Categories</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-gray-400">
                    Select a source category and a target category to move paintings between them. This is useful when reorganizing or merging categories.
                </p>
            </div>

            <div className="mt-6">
                <Link href="/admin/categories" className="block text-[var(--title-color)] hover:underline">
                    &larr; Back to Category Management
                </Link>
            </div>
        </div>
    );
}
