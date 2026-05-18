import { NextResponse } from 'next/server';

function getApiBaseUrl(): string {
    return process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';
}

export async function POST(request: Request) {
    try {
        const body = await request.json();
        const apiUrl = `${getApiBaseUrl()}/auth/google/callback`;

        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(body),
        });

        if (!response.ok) {
            const errorData = await response.json();
            return NextResponse.json(
                { error: errorData.message || 'Authentication failed' },
                { status: response.status }
            );
        }

        const data = await response.json();

        // Create response and set httpOnly cookie
        const nextResponse = NextResponse.json(data);
        nextResponse.cookies.set('admin_token', data.token, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            maxAge: 60 * 60, // 1 hour
            path: '/',
        });

        return nextResponse;
    } catch {
        return NextResponse.json(
            { error: 'Internal server error' },
            { status: 500 }
        );
    }
}
