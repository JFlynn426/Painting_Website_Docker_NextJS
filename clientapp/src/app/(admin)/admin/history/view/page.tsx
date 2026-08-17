'use client';

import Link from 'next/link';

export default function ViewEditingHistoryPage() {
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();

    return (
        <div>
            <Link href="/admin/history" className="text-blue-400 hover:text-blue-300 mb-6 inline-block">
                ← Back to History
            </Link>

            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">View Editing History</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-[var(--foreground)]">
                    Browse all editing history records for {artworkLabelLowerPlural}, categories, and content changes. This feature is coming soon.
                </p>
            </div>
        </div>
    );
}
