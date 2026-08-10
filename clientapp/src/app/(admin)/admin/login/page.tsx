'use client';

import { useCallback, useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { verifyAuth } from '@/lib/auth';
import styles from './page.module.css';

interface AuthResponse {
    adminUser: {
        id: string;
        email: string;
        displayName: string;
        pictureUrl?: string;
        lastLoginAt: string;
        createdAt: string;
        isActive: boolean;
    };
}

export default function AdminLoginPage() {
    const router = useRouter();
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    const [checkingAuth, setCheckingAuth] = useState(true);

    // Check if user is already authenticated - redirect to admin dashboard if so
    // This uses the server-side /api/auth/me endpoint which validates the httpOnly cookie
    useEffect(() => {
        verifyAuth().then(user => {
            if (user) {
                router.replace('/admin');
            }
            setCheckingAuth(false);
        });
    }, [router]);

    const handleCallback = useCallback(async (code: string, state: string, provider: string) => {
        setLoading(true);
        setError(null);

        try {
            const callbackEndpoint = provider === 'yahoo'
                ? '/api/auth/yahoo/callback'
                : '/api/auth/google/callback';

            const response = await fetch(callbackEndpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify({ code, state }),
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Authentication failed');
            }

            const data: AuthResponse = await response.json();

            // Store only non-sensitive display data in localStorage
            // The actual auth token is stored as httpOnly cookie by the backend
            localStorage.setItem('admin_user', JSON.stringify({
                displayName: data.adminUser.displayName,
                pictureUrl: data.adminUser.pictureUrl
            }));

            // Redirect to admin dashboard
            router.push('/admin');
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Authentication failed');
            setLoading(false);
        }
    }, [router]);

    useEffect(() => {
        // Check if we have an auth code in the URL
        const urlParams = new URLSearchParams(window.location.search);
        const code = urlParams.get('code');
        const state = urlParams.get('state');
        const provider = sessionStorage.getItem('oauth_provider');

        if (code && state && provider) {
            sessionStorage.removeItem('oauth_provider');
            handleCallback(code, state, provider);
        }
    }, [handleCallback]);

    const handleGoogleLogin = async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await fetch('/api/auth/google/url');
            if (!response.ok) {
                throw new Error('Failed to get Google authorization URL');
            }

            const data = await response.json();
            sessionStorage.setItem('oauth_provider', 'google');
            window.location.href = data.url;
        } catch (err) {
            setError(err instanceof Error ? err.message : 'An error occurred');
            setLoading(false);
        }
    };

    const handleYahooLogin = async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await fetch('/api/auth/yahoo/url');
            if (!response.ok) {
                throw new Error('Failed to get Yahoo authorization URL');
            }

            const data = await response.json();
            sessionStorage.setItem('oauth_provider', 'yahoo');
            window.location.href = data.url;
        } catch (err) {
            setError(err instanceof Error ? err.message : 'An error occurred');
            setLoading(false);
        }
    };

    return (
        <div className={styles.container}>
            {checkingAuth && (
                <div className="text-white">Checking authentication...</div>
            )}
            <div className={styles.loginCard}>
                <h1 className={styles.title}>Admin Login</h1>
                <p className={styles.subtitle}>Sign in to access the admin panel</p>

                {error && (
                    <div className={styles.error}>
                        {error}
                    </div>
                )}

                <button
                    onClick={handleGoogleLogin}
                    disabled={loading}
                    className={styles.googleButton}
                >
                    {loading ? (
                        'Signing in...'
                    ) : (
                        <>
                            <svg className={styles.googleIcon} viewBox="0 0 24 24">
                                <path
                                    fill="#4285F4"
                                    d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                                />
                                <path
                                    fill="#34A853"
                                    d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                                />
                                <path
                                    fill="#FBBC05"
                                    d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                                />
                                <path
                                    fill="#EA4335"
                                    d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                                />
                            </svg>
                            Sign in with Google
                        </>
                    )}
                </button>

                <button
                    onClick={handleYahooLogin}
                    disabled={loading}
                    className={styles.yahooButton}
                >
                    {loading ? (
                        'Signing in...'
                    ) : (
                        <>
                            <svg className={styles.yahooIcon} viewBox="0 0 24 24">
                                <path
                                    fill="#ffffff"
                                    d="M12.473 2.594l-4.14 5.228h2.567l-.293 2.754H2.594v2.754h8.177l-1.186 11.122h2.886l1.17-11.122h2.346l-2.514-2.754h-2.567l.293-2.754h2.567l-2.003-5.228z"
                                />
                            </svg>
                            Sign in with Yahoo
                        </>
                    )}
                </button>
            </div>
        </div>
    );
}
