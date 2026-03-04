"use client";

import Image from "next/image";
import { useEffect, useState, useMemo } from "react";

interface PaintingDetailImageProps {
    src: string;
    alt: string;
    className?: string;
}

export default function PaintingDetailImage({ src, alt, className }: PaintingDetailImageProps) {
    const [aspectRatio, setAspectRatio] = useState<number | null>(null);

    useEffect(() => {
        // Create a new image to get natural dimensions
        const img = document.createElement("img");
        img.src = src;
        img.onload = () => {
            const ratio = img.naturalWidth / img.naturalHeight;
            setAspectRatio(ratio);
        };
    }, [src]);

    const imageStyle = useMemo(() => {
        if (aspectRatio === null) return {};

        const viewportHeight = window.innerHeight;
        const viewportWidth = window.innerWidth;

        // Calculate dimensions at 80vh height
        const height80vh = viewportHeight * 0.8;
        const widthAt80vh = height80vh * aspectRatio;

        // Check if width at 80vh exceeds 75vw
        const max75vw = viewportWidth * 0.75;

        if (widthAt80vh <= max75vw) {
            // Use 80vh height, width auto to maintain aspect ratio
            return { height: `${height80vh}px`, width: 'auto' };
        } else {
            // Use 75vw width, height auto to maintain aspect ratio
            return { width: `${max75vw}px`, height: 'auto' };
        }
    }, [aspectRatio]);

    return (
        <Image
            src={src}
            alt={alt}
            width={1200}
            height={1200}
            style={imageStyle}
            className={className}
            priority
            sizes="100vw"
        />
    );
}