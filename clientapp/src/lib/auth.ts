// ============================================================================
// Auth Verification Utilities
// ============================================================================
// Provides functions for verifying authentication state and handling logout.
// Relies on httpOnly cookie (admin_token) for actual auth, not localStorage.
// ============================================================================

export interface AdminUserDto {
    id: string;
    email: string;
    displayName: string;
    pictureUrl?: string;
    lastLoginAt: string;
    createdAt: string;
    isActive: boolean;
}

/**
 * Check if user is authenticated by calling /api/auth/me
 * Returns the user info if authenticated, null if not.
 */
export async function verifyAuth(): Promise<AdminUserDto | null> {
    try {
        const response = await fetch('/api/auth/me', {
            credentials: 'include',
            cache: 'no-store'
        });

        if (response.status === 401) {
            return null;
        }

        if (!response.ok) {
            throw new Error('Auth verification failed');
        }

        return await response.json() as AdminUserDto;
    } catch {
        return null;
    }
}

/**
 * Logout by clearing the httpOnly cookie on the server.
 * Redirects to /admin/login after clearing.
 */
export async function logout(): Promise<void> {
    try {
        await fetch('/api/auth/logout', {
            method: 'POST',
            credentials: 'include'
        });
    } catch {
        // Ignore logout errors - still redirect to login
    } finally {
        // Clear any cached user display data (non-sensitive)
        if (typeof window !== 'undefined') {
            localStorage.removeItem('admin_user');
            window.location.href = '/admin/login';
        }
    }
}
