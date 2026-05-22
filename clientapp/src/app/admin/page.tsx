'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { verifyAuth, logout, AdminUserDto } from '@/lib/auth';

export default function AdminDashboardPage() {
    const router = useRouter();
    const [user, setUser] = useState<AdminUserDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        verifyAuth().then(result => {
            if (!result) {
                router.push('/admin/login');
            } else {
                setUser(result);
                // Store non-sensitive display data for UI caching
                localStorage.setItem('admin_user', JSON.stringify({
                    displayName: result.displayName,
                    pictureUrl: result.pictureUrl
                }));
            }
            setLoading(false);
        }).catch(() => {
            router.push('/admin/login');
            setLoading(false);
        });
    }, [router]);

    const handleLogout = async () => {
        await logout();
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
