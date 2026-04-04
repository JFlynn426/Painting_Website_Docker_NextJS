import Image from 'next/image';
import Link from 'next/link';
import styles from './PaintingImage.module.css';

interface PaintingImageProps {
    src: string;
    thumbnailUrl?: string;
    alt: string;
    priority?: boolean;
    categorySlug: string;
    filename: string;
    orientation?: 'landscape' | 'portrait' | 'square';
    title?: string;
    price?: number;
    width?: number;
    height?: number;
    depth?: number;
    isAvailable?: boolean;
}

export default function PaintingImage({
    src,
    thumbnailUrl,
    alt,
    priority = false,
    categorySlug,
    filename,
    orientation,
    title,
    price,
    width,
    height,
    depth,
    isAvailable = true
}: PaintingImageProps) {
    // The filename prop is already the painting slug from the API
    const slug = filename;
    const detailsUrl = `/paintings/${categorySlug}/${slug}`;

    // Use thumbnailUrl if available, otherwise fall back to src (full image)
    const displaySrc = thumbnailUrl || src;

    // Format dimensions
    const dimensions = width && height
        ? `${width}" x ${height}"${depth ? ` x ${depth}"` : ''}`
        : '';

    // Format price
    const formattedPrice = price ? `$${price.toLocaleString()}` : '';

    // Determine availability
    const availability = isAvailable ? 'Available' : 'Sold';

    // Determine the orientation class
    const orientationClass = orientation ? styles[`paintingImage${orientation.charAt(0).toUpperCase() + orientation.slice(1)}`] : '';

    return (
        <Link href={detailsUrl} className={styles.imageWrapper}>
            <div className={styles.imageContainer}>
                <Image
                    src={displaySrc}
                    alt={alt}
                    width={400}
                    height={400}
                    className={`${styles.paintingImage} ${orientationClass}`}
                    priority={priority}
                    sizes="(max-width: 768px) 100vw, 50vw"
                    quality={60}
                />
                <div className={styles.hoverOverlay}>
                    <div className={styles.hoverContent}>
                        <h3 className={styles.hoverTitle}>{title || alt}</h3>
                        {formattedPrice && <p className={styles.hoverPrice}>{formattedPrice}</p>}
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