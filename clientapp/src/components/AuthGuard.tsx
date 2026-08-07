'use client';

import { useEffect, useState, ReactNode } from 'react';
import { useRouter, usePathname } from 'next/navigation';
import { verifyAuth } from '@/lib/auth';

interface AuthGuardProps {
    children: ReactNode;
}

export default function AuthGuard({ children }: AuthGuardProps) {
    const router = useRouter();
    const pathname = usePathname();
    const isLoginPage = pathname === '/admin/login';

    // Login page is immediately authenticated (skip check), others start as null (loading)
    const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(isLoginPage);

    useEffect(() => {
        // Skip auth check for login page to prevent redirect loop
        if (isLoginPage) {
            return;
        }

        let cancelled = false;

        const checkAuth = async () => {
            const user = await verifyAuth();
            if (!cancelled) {
                if (!user) {
                    router.push('/admin/login');
                } else {
                    setIsAuthenticated(true);
                }
            }
        };

        checkAuth();

        return () => {
            cancelled = true;
        };
    }, [router, pathname, isLoginPage]);

    // Show loading state while checking auth
    if (isAuthenticated === null) {
        return (
            <div className="flex items-center justify-center min-h-[200px]">
                <div className="text-[var(--foreground)]">Loading...</div>
            </div>
        );
    }

    // isAuthenticated is true at this point (false case redirects above)
    return <>{children}</>;
}
