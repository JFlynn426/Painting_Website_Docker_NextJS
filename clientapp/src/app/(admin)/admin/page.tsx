'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { verifyAuth, AdminUserDto } from '@/lib/auth';

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

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[200px]">
                <div className="text-white">Loading...</div>
            </div>
        );
    }

    return (
        <div>
            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <h2 className="text-2xl font-bold mb-4">Welcome, {user?.displayName}</h2>
                <p className="text-gray-400">Admin dashboard coming soon...</p>
            </div>
        </div>
    );
}
