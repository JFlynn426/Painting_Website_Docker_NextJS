import { NextResponse } from 'next/server';

function getApiBaseUrl(): string {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL;
    if (!apiUrl) {
        throw new Error('NEXT_PUBLIC_API_URL is not set');
    }
    return apiUrl;
}

export async function POST(request: Request) {
    try {
        const body = await request.json();
        const apiUrl = `${getApiBaseUrl()}/auth/yahoo/callback`;

        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify(body),
        });

        if (!response.ok) {
            return NextResponse.json(
                { error: 'Authentication failed' },
                { status: response.status }
            );
        }

        const data = await response.json();
        const res = NextResponse.json(data);

        // Node.js fetch strips Set-Cookie headers, so we set the cookie directly
        // from the token in the response body
        if (data.token) {
            res.cookies.set('admin_token', data.token, {
                httpOnly: true,
                secure: process.env.NODE_ENV === 'production',
                sameSite: 'strict',
                maxAge: 60 * 60, // 1 hour
                path: '/',
            });
        }

        return res;
    } catch {
        return NextResponse.json(
            { error: 'Internal server error' },
            { status: 500 }
        );
    }
}
