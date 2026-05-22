import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

/**
 * Next.js Proxy for protecting admin routes.
 *
 * This proxy runs at the Edge and checks for the presence of the
 * `admin_token` cookie before allowing access to /admin routes.
 *
 * Note: This only checks cookie presence, not validity. The actual token
 * validation happens via the API call to /api/auth/me in the admin page.
 */
export function proxy(request: NextRequest) {
    const isAdminRoute = request.nextUrl.pathname.startsWith('/admin');
    const isLoginRoute = request.nextUrl.pathname === '/admin/login';

    // Only protect non-login admin routes
    if (isAdminRoute && !isLoginRoute) {
        const token = request.cookies.get('admin_token');

        if (!token) {
            // Redirect to login page if no cookie present
            return NextResponse.redirect(new URL('/admin/login', request.url));
        }
    }

    return NextResponse.next();
}

// Configure proxy to run only on /admin routes
export const config = {
    matcher: ['/admin/:path*'],
};
