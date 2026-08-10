import { NextResponse } from 'next/server';

function getApiBaseUrl(): string {
    return process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';
}

export async function GET() {
    try {
        const apiUrl = `${getApiBaseUrl()}/auth/yahoo/url`;
        const response = await fetch(apiUrl);

        if (!response.ok) {
            return NextResponse.json(
                { error: 'Failed to get Yahoo authorization URL' },
                { status: response.status }
            );
        }

        const data = await response.json();
        return NextResponse.json(data);
    } catch {
        return NextResponse.json(
            { error: 'Internal server error' },
            { status: 500 }
        );
    }
}
