'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { logout, verifyAuth, AdminUserDto } from '@/lib/auth';

export default function AdminHeader() {
    const pathname = usePathname();
    const [user, setUser] = useState<AdminUserDto | null>(null);

    useEffect(() => {
        verifyAuth().then(setUser);
    }, []);

    const handleLogout = async () => {
        await logout();
    };

    const artworkLabel = process.env.NEXT_PUBLIC_NAVBAR_ARTWORK_LABEL || "Paintings";

    const navLinks = [
        { href: '/admin', label: 'Dashboard' },
        { href: '/admin/paintings', label: artworkLabel },
        { href: '/admin/categories', label: 'Categories' },
        { href: '/admin/content', label: 'Content' },
        { href: '/admin/history', label: 'History' },
    ];

    const isLoginPage = pathname === '/admin/login';

    // Find the longest matching nav link (so /admin/paintings/add matches Paintings, not Dashboard)
    const activeHref = navLinks
        .filter(link => pathname === link.href || pathname.startsWith(link.href + '/'))
        .sort((a, b) => b.href.length - a.href.length)[0]?.href;

    if (isLoginPage) {
        return (
            <header className="bg-[var(--navbar-footer-bg)] shadow-lg text-center">
                <div className="p-6">
                    <h1 className="text-3xl font-bold mb-4" style={{ color: 'var(--title-color)' }}>
                        Only site administrators will be able to log in
                    </h1>
                    <Link
                        href="/"
                        className="inline-block px-6 py-3 border-2 border-[var(--title-color)] text-[var(--title-color)] rounded hover:bg-[var(--title-color)] hover:text-white transition-colors font-medium"
                    >
                        Back to Home Page
                    </Link>
                </div>
                <div className="h-px bg-[var(--foreground)] w-full"></div>
            </header>
        );
    }

    return (
        <nav className="bg-[var(--navbar-footer-bg)] shadow-lg">
            <div className="container mx-auto p-4">
                <div className="flex justify-between items-center mb-4">
                    <Link href="/" className="text-xl font-bold" style={{ color: 'var(--title-color)' }}>
                        Admin Panel
                    </Link>
                    {user && (
                        <div className="flex items-center gap-2">
                            {user.pictureUrl ? (
                                <img
                                    src={user.pictureUrl}
                                    alt={user.displayName}
                                    className="w-8 h-8 rounded-full"
                                    onError={(e) => {
                                        const target = e.currentTarget;
                                        target.style.display = 'none';
                                        const parent = target.parentElement;
                                        if (parent) {
                                            const fallback = document.createElement('div');
                                            fallback.className = 'w-8 h-8 rounded-full bg-[var(--button-color)] flex items-center justify-center text-white text-sm font-bold';
                                            fallback.textContent = user.displayName.charAt(0).toUpperCase();
                                            parent.insertBefore(fallback, target);
                                        }
                                    }}
                                />
                            ) : (
                                <div className="w-8 h-8 rounded-full bg-[var(--button-color)] flex items-center justify-center text-white text-sm font-bold">
                                    {user.displayName.charAt(0).toUpperCase()}
                                </div>
                            )}
                            <span className="text-[var(--foreground)] text-sm whitespace-nowrap">{user.displayName}</span>
                        </div>
                    )}
                </div>
                <div className="flex items-center gap-4 overflow-x-auto">
                    {navLinks.map(link => (
                        <Link
                            key={link.href}
                            href={link.href}
                            className={`px-3 py-2 rounded transition-colors whitespace-nowrap ${link.href === '/admin/history' ? 'hidden' : ''} ${activeHref === link.href
                                ? 'bg-[var(--button-color)] text-white'
                                : 'text-[var(--foreground)] hover:bg-[var(--admin-hover)]'
                                }`}
                        >
                            {link.label}
                        </Link>
                    ))}
                    <button
                        onClick={handleLogout}
                        className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors whitespace-nowrap ml-auto"
                    >
                        Logout
                    </button>
                </div>
            </div>
            <div className="h-px bg-[var(--foreground)] w-full"></div>
        </nav>
    );
}
