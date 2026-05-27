'use client';

import Link from 'next/link';

export default function PaintingsAdminPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Painting Management</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/paintings/add" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Add Paintings</h2>
                        <p className="text-gray-400">
                            Add new paintings to existing categories on the website. Upload images, set titles, descriptions, and assign to a category.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/paintings/delete" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Delete Paintings</h2>
                        <p className="text-gray-400">
                            Remove existing paintings from the website. Select paintings to permanently delete from the database and storage.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/paintings/edit" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Edit Paintings</h2>
                        <p className="text-gray-400">
                            Modify details of existing paintings including titles, descriptions, images, dimensions, year, and category assignments.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/paintings/select-new" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Select New Paintings</h2>
                        <p className="text-gray-400">
                            Mark paintings as new to highlight recent additions on the website. Choose which paintings appear in the new arrivals section.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
