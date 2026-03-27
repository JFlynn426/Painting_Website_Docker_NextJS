import PaintingImage from './PaintingImage';
import styles from '../app/paintings/[category]/page.module.css';

export interface PaintingImageItem {
    src: string;
    alt: string;
    filename: string;
}

interface PaintingGridProps {
    images: PaintingImageItem[];
    categorySlug: string;
}

export default function PaintingGrid({ images, categorySlug }: PaintingGridProps) {
    if (images.length === 0) {
        return <p className={styles.noImages}>No images available for this category.</p>;
    }

    return (
        <div className={styles.masonryGrid}>
            {images.map((image, index) => (
                <PaintingImage
                    key={index}
                    src={image.src}
                    alt={image.alt}
                    filename={image.filename}
                    categorySlug={categorySlug}
                    priority={index < 3}
                />
            ))}
        </div>
    );
}