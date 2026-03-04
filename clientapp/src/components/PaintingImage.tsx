"use client";

import Image from 'next/image';
import Link from 'next/link';
import { paintingsData } from '@/app/models/paintings';
import styles from '../app/paintings/[category]/page.module.css';

interface PaintingImageProps {
    src: string;
    alt: string;
    priority?: boolean;
    category: string;
    filename: string;
}

export default function PaintingImage({ src, alt, priority = false, category, filename }: PaintingImageProps) {
    // Generate slug from filename (e.g., "Aspens.jpg" -> "aspens")
    const slug = filename.replace(/\.[^/.]+$/, "").toLowerCase().replace(/[^a-z0-9]+/g, "-").trim();
    const detailsUrl = `/paintings/${category}/${slug}`;

    // Find painting data for hover info
    const painting = paintingsData.find(p => {
        const fileSlug = p.imageUrl.split('/').pop()?.replace(/\.[^/.]+$/, "").toLowerCase().replace(/[^a-z0-9]+/g, "-").trim();
        return fileSlug === slug && p.category === category;
    });

    const title = painting?.title || alt;
    const price = painting?.price ? `$${painting.price.toLocaleString()}` : '';
    const dimensions = painting?.dimensions || '';
    const availability = painting?.isAvailable !== false ? 'Available' : 'Sold';

    return (
        <Link href={detailsUrl} className={styles.imageWrapper}>
            <div className={styles.imageContainer}>
                <Image
                    src={src}
                    alt={alt}
                    width={400}
                    height={400}
                    className={styles.paintingImage}
                    priority={priority}
                />
                <div className={styles.hoverOverlay}>
                    <div className={styles.hoverContent}>
                        <h3 className={styles.hoverTitle}>{title}</h3>
                        {price && <p className={styles.hoverPrice}>{price}</p>}
                        {dimensions && <p className={styles.hoverDimensions}>{dimensions}</p>}
                        <p className={`${styles.hoverAvailability} ${availability === 'Available' ? styles.available : styles.sold}`}>
                            {availability}
                        </p>
                    </div>
                </div>
            </div>
        </Link>
    );
}