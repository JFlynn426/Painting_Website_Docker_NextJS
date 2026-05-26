'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { logout } from '@/lib/auth';

export default function AdminHeader() {
    const pathname = usePathname();

    const handleLogout = async () => {
        await logout();
    };

    const navLinks = [
        { href: '/admin', label: 'Dashboard' },
        { href: '/admin/paintings', label: 'Paintings' },
        { href: '/admin/categories', label: 'Categories' },
        { href: '/admin/content', label: 'Content' },
        { href: '/admin/settings', label: 'Settings' },
    ];

    const isLoginPage = pathname === '/admin/login';

    if (isLoginPage) {
        return (
            <header className="bg-[var(--navbar-footer-bg)] p-6 shadow-lg text-center">
                <h1 className="text-3xl font-bold mb-4" style={{ color: 'var(--title-color)' }}>
                    Only site administrators will be able to log in
                </h1>
                <Link
                    href="/"
                    className="inline-block px-6 py-3 border-2 border-blue-400 text-blue-400 rounded hover:bg-blue-400 hover:text-gray-900 transition-colors font-medium"
                >
                    Back to Home Page
                </Link>
            </header>
        );
    }

    return (
        <nav className="bg-[var(--navbar-footer-bg)] p-4 shadow-lg">
            <div className="container mx-auto">
                <div className="flex justify-between items-center mb-4">
                    <Link href="/" className="text-xl font-bold" style={{ color: 'var(--title-color)' }}>
                        Admin Panel
                    </Link>
                    <button
                        onClick={handleLogout}
                        className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors"
                    >
                        Logout
                    </button>
                </div>
                <div className="flex gap-4 overflow-x-auto">
                    {navLinks.map(link => (
                        <Link
                            key={link.href}
                            href={link.href}
                            className={`px-3 py-2 rounded transition-colors whitespace-nowrap ${pathname === link.href
                                ? 'bg-blue-600 text-white'
                                : 'text-gray-300 hover:bg-gray-700'
                                }`}
                        >
                            {link.label}
                        </Link>
                    ))}
                </div>
            </div>
        </nav>
    );
}
