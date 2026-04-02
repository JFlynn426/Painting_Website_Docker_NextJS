import PaintingImage from './PaintingImage';
import styles from './PaintingGrid.module.css';
import { buildSmartRows, PaintingRow } from '@/lib/paintingGridHelpers';

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

/**
 * Gets the CSS class for a painting row based on its type.
 */
function getPaintingRowClass(rowType: 'landscape' | 'portrait' | 'mixed'): string {
    const baseClass = styles.paintingRow;

    switch (rowType) {
        case 'landscape':
            return `${baseClass} ${styles.paintingRowLandscape}`;
        case 'portrait':
            return `${baseClass} ${styles.paintingRowPortrait}`;
        case 'mixed':
        default:
            return `${baseClass} ${styles.paintingRowMixed}`;
    }
}

export default function PaintingGrid({ images, categorySlug }: PaintingGridProps) {
    if (images.length === 0) {
        return <p className={styles.noImages}>No images available for this category.</p>;
    }

    // Build smart rows from images using the smart row building algorithm
    const rows = buildSmartRows(images);

    return (
        <div className={styles.paintingGrid}>
            {rows.map((row, rowIndex) => (
                <div key={rowIndex} className={getPaintingRowClass(row.type)}>
                    {row.items.map((image, imageIndex) => (
                        <div key={imageIndex} className={getPaintingItemClass(image.orientation)}>
                            <PaintingImage
                                src={image.src}
                                alt={image.alt}
                                filename={image.filename}
                                categorySlug={categorySlug}
                                priority={rowIndex < 1 && imageIndex < 2}
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
            ))}
        </div>
    );
}