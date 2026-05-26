'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';

export default function AdminFooter() {
    const pathname = usePathname();

    if (pathname === '/admin/login') {
        return null;
    }

    return (
        <footer className="bg-[var(--navbar-footer-bg)] p-4 text-center text-gray-500 text-sm">
            <div className="container mx-auto">
                <p>Admin Panel &copy; {new Date().getFullYear()} Gloria Gronowicz Fine Art</p>
                <Link href="/" className="text-gray-400 hover:text-gray-200 transition-colors">
                    Back to Public Site
                </Link>
            </div>
        </footer>
    );
}
