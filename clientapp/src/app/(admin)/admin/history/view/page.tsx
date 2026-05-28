'use client';

import Link from 'next/link';

export default function ViewEditingHistoryPage() {
    return (
        <div>
            <Link href="/admin/history" className="text-blue-400 hover:text-blue-300 mb-6 inline-block">
                ← Back to History
            </Link>

            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">View Editing History</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-gray-400">
                    Browse all editing history records for paintings, categories, and content changes. This feature is coming soon.
                </p>
            </div>
        </div>
    );
}
