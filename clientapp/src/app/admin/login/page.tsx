'use client';

import { useEffect, useState } from 'react';
import styles from './page.module.css';

interface GoogleAuthResponse {
    token: string;
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
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        // Check if we have a Google auth code in the URL
        const urlParams = new URLSearchParams(window.location.search);
        const code = urlParams.get('code');
        const state = urlParams.get('state');

        if (code && state) {
            handleCallback(code, state);
        }
    }, []);

    const handleGoogleLogin = async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await fetch('/api/auth/google/url');
            if (!response.ok) {
                throw new Error('Failed to get Google authorization URL');
            }

            const data = await response.json();
            window.location.href = data.url;
        } catch (err) {
            setError(err instanceof Error ? err.message : 'An error occurred');
            setLoading(false);
        }
    };

    const handleCallback = async (code: string, state: string) => {
        setLoading(true);
        setError(null);

        try {
            const response = await fetch('/api/auth/google/callback', {
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

            const data: GoogleAuthResponse = await response.json();

            // Store the token in localStorage for subsequent requests
            localStorage.setItem('admin_token', data.token);
            localStorage.setItem('admin_user', JSON.stringify(data.adminUser));

            // Redirect to admin dashboard (or home page for now)
            window.location.href = '/admin';
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Authentication failed');
            setLoading(false);
        }
    };

    return (
        <div className={styles.container}>
            <div className={styles.loginCard}>
                <h1 className={styles.title}>Admin Login</h1>
                <p className={styles.subtitle}>Sign in with Google to access the admin panel</p>

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
            </div>
        </div>
    );
}
