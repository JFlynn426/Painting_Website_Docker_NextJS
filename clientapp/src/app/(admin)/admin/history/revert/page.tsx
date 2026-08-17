'use client';

import Link from 'next/link';

export default function RevertEditingHistoryPage() {
    const artworkLabelPlural = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL_PLURAL || "Paintings";
    const artworkLabelLowerPlural = artworkLabelPlural.toLowerCase();

    return (
        <div>
            <Link href="/admin/history" className="text-blue-400 hover:text-blue-300 mb-6 inline-block">
                ← Back to History
            </Link>

            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Revert Editing History</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-[var(--foreground)]">
                    Revert previous changes to {artworkLabelLowerPlural}, categories, or content by selecting from editing history.
                </p>
                <p className="text-[var(--foreground)] mt-4 text-sm">
                    Note: This feature is only visible to John Flynn to keep database reversions minimal, as the feature is complex and prone to issues.
                </p>
            </div>
        </div>
    );
}
