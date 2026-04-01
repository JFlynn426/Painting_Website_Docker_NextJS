import PaintingImage from './PaintingImage';
import styles from './PaintingGrid.module.css';

export interface PaintingImageItem {
    src: string;
    alt: string;
    filename: string;
    width?: number;
    height?: number;
    orientation?: 'landscape' | 'portrait' | 'square';
    title?: string;
    price?: number;
    depth?: number;
    isAvailable?: boolean;
}

interface PaintingGridProps {
    images: PaintingImageItem[];
    categorySlug: string;
}

/**
 * Gets the appropriate CSS class for the painting item wrapper based on orientation.
 * Landscape paintings span 3 columns (50% width), portrait and square span 2 columns (33% width).
 */
function getPaintingItemClass(orientation?: 'landscape' | 'portrait' | 'square'): string {
    const baseClass = styles.paintingItem;

    // Debug: log orientation for troubleshooting
    console.log('Painting orientation:', orientation);

    switch (orientation) {
        case 'landscape':
            return `${baseClass} ${styles.paintingItemLandscape}`;
        case 'portrait':
            return `${baseClass} ${styles.paintingItemPortrait}`;
        case 'square':
        default:
            return `${baseClass} ${styles.paintingItemSquare}`;
    }
}

export default function PaintingGrid({ images, categorySlug }: PaintingGridProps) {
    if (images.length === 0) {
        return <p className={styles.noImages}>No images available for this category.</p>;
    }

    // Debug: log all orientations
    console.log('All painting orientations:', images.map(img => img.orientation));

    return (
        <div className={styles.paintingGrid}>
            {images.map((image, index) => (
                <div key={index} className={getPaintingItemClass(image.orientation)}>
                    <PaintingImage
                        src={image.src}
                        alt={image.alt}
                        filename={image.filename}
                        categorySlug={categorySlug}
                        priority={index < 3}
                        orientation={image.orientation}
                        title={image.title}
                        price={image.price}
                        width={image.width}
                        height={image.height}
                        depth={image.depth}
                        isAvailable={image.isAvailable}
                    />
                </div>
            ))}
        </div>
    );
}