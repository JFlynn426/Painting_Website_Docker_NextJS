"use client"

interface ErrorProps {
    error: Error;
    reset: () => void;
}

export default function Error({ error, reset }: ErrorProps) {
    return (
        <div className="flex flex-col items-center justify-center min-vh-100 text-center p-4 text-[var(--foreground)]">
            <h1 className="text-3xl font-bold mb-4">Something went wrong!</h1>
            <p className="text-lg mb-4">{error.message}</p>
            <button
                className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                onClick={() => reset()}
            >
                Try again
            </button>
        </div>
    );
}