'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';

interface AdminUser {
    id: string;
    email: string;
    displayName: string;
    pictureUrl?: string;
    lastLoginAt: string;
    createdAt: string;
    isActive: boolean;
}

export default function AdminDashboardPage() {
    const router = useRouter();
    const [user, setUser] = useState<AdminUser | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('admin_token');
        const userData = localStorage.getItem('admin_user');

        if (!token || !userData) {
            router.push('/admin/login');
            return;
        }

        try {
            setUser(JSON.parse(userData));
        } catch {
            router.push('/admin/login');
        }

        setLoading(false);
    }, [router]);

    const handleLogout = async () => {
        try {
            await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include',
            });
        } catch {
            // Ignore logout errors
        } finally {
            localStorage.removeItem('admin_token');
            localStorage.removeItem('admin_user');
            router.push('/admin/login');
        }
    };

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: 'var(--background)' }}>
                <div className="text-white">Loading...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen" style={{ backgroundColor: 'var(--background)', color: 'var(--foreground)' }}>
            <nav className="bg-[var(--navbar-footer-bg)] p-4">
                <div className="container mx-auto flex justify-between items-center">
                    <h1 className="text-xl font-bold" style={{ color: 'var(--title-color)' }}>Admin Panel</h1>
                    <div className="flex items-center gap-4">
                        {user && (
                            <span className="text-sm">{user.displayName}</span>
                        )}
                        <button
                            onClick={handleLogout}
                            className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors"
                        >
                            Logout
                        </button>
                    </div>
                </div>
            </nav>

            <main className="container mx-auto p-6">
                <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                    <h2 className="text-2xl font-bold mb-4">Welcome, {user?.displayName}</h2>
                    <p className="text-gray-400">Admin dashboard coming soon...</p>
                </div>
            </main>
        </div>
    );
}
