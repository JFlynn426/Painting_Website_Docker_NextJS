"use client"

import Link from 'next/link';

interface ErrorProps {
    error: Error;
    reset: () => void;
}

export default function Error({ error, reset }: ErrorProps) {
    return (
        <div style={{ textAlign: 'center', padding: '2rem' }}>
            <h1 style={{ color: 'var(--title-color)', fontSize: '2.5rem', marginBottom: '1rem' }}>
                An Unexpected Error Occured
            </h1>
            <p style={{ fontSize: '1.1rem', marginBottom: '2rem' }}>
                The website administrator has been notified
            </p>
            <Link
                href="/"
                style={{
                    display: 'inline-flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    padding: '12px 24px',
                    backgroundColor: 'var(--button-color)',
                    color: 'white',
                    textDecoration: 'none',
                    borderRadius: '5px',
                    fontSize: '1rem',
                    border: '1px solid white'
                }}
            >
                Return to Home Page
            </Link>
        </div>
    );
}