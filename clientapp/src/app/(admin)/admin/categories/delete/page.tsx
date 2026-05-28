import Link from 'next/link';

export default function DeleteCategoryPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Delete Painting Category</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-gray-400">
                    Select a category to delete. You can only delete a category if all paintings have been removed from it first. Use the Reassign Paintings feature to move paintings to another category before deleting.
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
