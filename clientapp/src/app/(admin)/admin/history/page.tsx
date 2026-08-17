'use client';

import Link from 'next/link';

export default function HistoryAdminPage() {
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();

    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">History</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/history/view" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">View Editing History</h2>
                        <p className="text-[var(--foreground)]">
                            Browse all editing history records for {artworkLabelLowerPlural}, categories, and content changes. Track when changes were made and by whom.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/history/revert" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Revert Editing History</h2>
                        <p className="text-[var(--foreground)]">
                            Revert previous changes to {artworkLabelLowerPlural}, categories, or content by selecting from editing history. Restore previous versions easily.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
