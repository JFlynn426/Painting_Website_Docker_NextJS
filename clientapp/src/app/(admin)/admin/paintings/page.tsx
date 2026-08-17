'use client';

import Link from 'next/link';

export default function PaintingsAdminPage() {
    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Painting";
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">{artworkLabelPlural} Management</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/paintings/add" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Add {artworkLabel}</h2>
                        <p className="text-[var(--foreground)]">
                            Add new {artworkLabelLowerPlural} to existing categories on the website. Upload images, set titles, descriptions, and assign to a category, etc.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/paintings/delete" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Delete {artworkLabelPlural}</h2>
                        <p className="text-[var(--foreground)]">
                            Remove existing {artworkLabelLowerPlural} from the website. Select {artworkLabelLowerPlural} to permanently delete from the database and storage.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/paintings/edit" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Edit {artworkLabelPlural}</h2>
                        <p className="text-[var(--foreground)]">
                            Modify details of existing {artworkLabelLowerPlural} including titles, descriptions, images, dimensions, year, and category assignments, etc.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/paintings/select-new" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Select New {artworkLabelPlural}</h2>
                        <p className="text-[var(--foreground)]">
                            Mark {artworkLabelLowerPlural} as new to appear in the New {artworkLabelPlural} page. These {artworkLabelLowerPlural} will be displayed on the New {artworkLabelPlural} page and showcase your latest arrivals to the site.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
