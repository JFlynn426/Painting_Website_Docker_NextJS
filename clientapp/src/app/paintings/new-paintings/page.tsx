import { getNewPaintings } from "@/lib/api";
import PaintingGrid, { PaintingImageItem } from "@/components/PaintingGrid";
import styles from "./page.module.css";

/**
 * Determines if a painting is landscape or portrait based on its dimensions.
 * @param width - The width of the painting
 * @param height - The height of the painting
 * @returns 'landscape' if width > height, 'portrait' if height > width, 'square' if equal
 */
function getPaintingOrientation(width: number | undefined, height: number | undefined): 'landscape' | 'portrait' | 'square' {
    // Default to square if dimensions are not available
    if (width === undefined || height === undefined) {
        return 'square';
    }

    if (width > height) {
        return 'landscape';
    } else if (height > width) {
        return 'portrait';
    } else {
        return 'square';
    }
}

export default async function NewPaintingsPage() {
    // Fetch new paintings from API
    const paintings = await getNewPaintings();

    // Convert API paintings to PaintingImageItem format
    // The PaintingGrid component will use smart row building to group paintings by orientation
    const images: PaintingImageItem[] = paintings.map(painting => ({
        src: painting.imageUrl,
        alt: painting.title,
        filename: painting.slug,
        title: painting.title,
        price: painting.price,
        width: painting.width,
        height: painting.height,
        depth: painting.depth,
        isAvailable: painting.isAvailable,
        orientation: getPaintingOrientation(painting.width, painting.height)
    }));

    return (
        <div className={styles.container}>
            <h1 className={styles.categoryTitle}>New Paintings</h1>
            <p className={styles.description}>Discover our latest additions to the collection.</p>

            {images.length > 0 ? (
                <PaintingGrid images={images} categorySlug="new-paintings" />
            ) : (
                <p className={styles.noPaintings}>No new paintings available at this time.</p>
            )}
        </div>
    );
}