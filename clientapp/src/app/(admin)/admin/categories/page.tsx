import Link from 'next/link';

export default function CategoriesAdminPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Category Management</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/categories/add" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer min-h-[120px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Add Category</h2>
                        <p className="text-gray-400">
                            Create a new painting category to organize your artwork. Set the category name and description for better navigation.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories/edit" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer min-h-[120px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Edit Category</h2>
                        <p className="text-gray-400">
                            Edit existing category name or description. Update category information to better reflect your painting collections.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories/delete" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer min-h-[120px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Delete Category</h2>
                        <p className="text-gray-400">
                            Remove a painting category. Note: You can only delete a category if all paintings have been removed from it first.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories/reassign" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer min-h-[120px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Reassign Paintings</h2>
                        <p className="text-gray-400">
                            Move paintings between categories. Transfer paintings from one category to another before deleting empty categories or after creating a new category.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
