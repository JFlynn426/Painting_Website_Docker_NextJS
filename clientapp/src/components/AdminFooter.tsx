'use client';

import Link from 'next/link';

const siteName = process.env.NEXT_PUBLIC_SITE_NAME || "Gloria Gronowicz Fine Art";

export default function AdminFooter() {

    return (
        <footer className="bg-[var(--navbar-footer-bg)] text-center text-gray-500 text-sm">
            <div className="h-px bg-[var(--foreground)] w-full"></div>
            <div className="container mx-auto p-4">
                <p>Admin Panel &copy; {new Date().getFullYear()} {siteName}</p>
                <Link href="/" className="text-[var(--foreground)] hover:text-gray-200 transition-colors">
                    Back to Public Site
                </Link>
            </div>
        </footer>
    );
}
