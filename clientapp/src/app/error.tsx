"use client"

import Button from "react-bootstrap/esm/Button";

interface ErrorProps {
    error: Error;
    reset: () => void;
}

export default function Error({ error, reset }: ErrorProps) {
    return (
    <div>
        <h1
        >Something went wrong!</h1>
        <p
        >An unexpected error has occurred. Please try again later.</p>
        <Button onClick={reset}>Try Again</Button>
    </div>
    ) 
}