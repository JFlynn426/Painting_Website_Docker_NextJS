"use client";

import Image from 'next/image';
import Link from 'next/link';
import { paintingsData, formatDimensions } from '@/app/models/paintings';
import styles from '../app/paintings/[category]/page.module.css';

interface PaintingImageProps {
    src: string;
    alt: string;
    priority?: boolean;
    categorySlug: string;
    filename: string;
}

export default function PaintingImage({ src, alt, priority = false, categorySlug, filename }: PaintingImageProps) {
    // Find painting data for hover info and get the slug
    const painting = paintingsData.find(p => {
        const fileSlug = p.imageUrl.split('/').pop()?.replace(/\.[^/.]+$/, "").toLowerCase().replace(/[^a-z0-9]+/g, "-").trim();
        const filenameSlug = filename.replace(/\.[^/.]+$/, "").toLowerCase().replace(/[^a-z0-9]+/g, "-").trim();
        return fileSlug === filenameSlug && p.categorySlug === categorySlug;
    });

    // Use the painting's slug property for navigation
    const slug = painting?.slug || filename.replace(/\.[^/.]+$/, "").toLowerCase().replace(/[^a-z0-9]+/g, "-").trim();
    const detailsUrl = `/paintings/${categorySlug}/${slug}`;

    const title = painting?.title || alt;
    const price = painting?.price ? `$${painting.price.toLocaleString()}` : '';
    const dimensions = painting ? formatDimensions(painting) : '';
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