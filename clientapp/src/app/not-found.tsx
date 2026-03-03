import Link from 'next/link';

export default function NotFound() {
    return (
        <div style={{ textAlign: 'center', padding: '2rem' }}>
            <h1 style={{ color: 'var(--title-color)', fontSize: '2.5rem', marginBottom: '1rem' }}>
                Page Not Found - 404
            </h1>
            <p style={{ fontSize: '1.1rem', marginBottom: '2rem' }}>
                Sorry, the page you are looking for does not exist.
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
    )
}