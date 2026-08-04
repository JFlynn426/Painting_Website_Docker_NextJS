'use client';

import Link from 'next/link';

export default function AdminFooter() {

    return (
        <footer className="bg-[var(--navbar-footer-bg)] text-center text-gray-500 text-sm">
            <div className="h-px bg-[var(--foreground)] w-full"></div>
            <div className="container mx-auto p-4">
                <p>Admin Panel &copy; {new Date().getFullYear()} Gloria Gronowicz Fine Art</p>
                <Link href="/" className="text-white hover:text-gray-200 transition-colors">
                    Back to Public Site
                </Link>
            </div>
        </footer>
    );
}
