'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
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
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Welcome, {user?.displayName} - Dashboard</h1>
            <p className="text-[var(--foreground)] mb-6">
                Select an item to edit from the list below. You can navigate between items from the dashboard (this page) or from the navigation bar.
            </p>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/paintings" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Paintings</h2>
                        <p className="text-[var(--foreground)]">
                            Manage paintings: add new paintings, delete existing ones, edit details, and select new arrivals.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/categories" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Categories</h2>
                        <p className="text-[var(--foreground)]">
                            Manage painting categories: create new categories, edit existing ones, or remove categories.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/content" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Content</h2>
                        <p className="text-[var(--foreground)]">
                            Manage other website content: edit page text, descriptions, home page carousel, etc...
                        </p>
                    </div>
                </Link>

                <Link href="/admin/history" className="hidden">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-gray-700 transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">History</h2>
                        <p className="text-[var(--foreground)]">
                            View editing history: track all changes made to paintings, categories, and content.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
