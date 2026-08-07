'use client';

import Link from 'next/link';

export default function CategoriesAdminPage() {
    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Paintings";
    const artworkLabelLower = artworkLabel.toLowerCase();

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Category Management</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/categories/add" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Add Category</h2>
                        <p className="text-[var(--foreground)]">
                            Create a new {artworkLabelLower} category to organize your artwork. Set the category name and description for better navigation.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories/edit" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Edit Category</h2>
                        <p className="text-[var(--foreground)]">
                            Edit existing category name or description. Update category information to better reflect your {artworkLabelLower} collections.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories/delete" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Delete Category</h2>
                        <p className="text-[var(--foreground)]">
                            Remove a {artworkLabelLower} category. Note: You can only delete a category if all {artworkLabelLower} have been removed from it first.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories/reassign" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Reassign {artworkLabel}</h2>
                        <p className="text-[var(--foreground)]">
                            Move {artworkLabelLower} between categories. Transfer {artworkLabelLower} from one category to another before deleting empty categories or after creating a new category.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
