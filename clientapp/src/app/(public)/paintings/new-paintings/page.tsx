import { getNewPaintings } from "@/lib/api";
import PaintingGrid, { PaintingImageItem } from "@/components/PaintingGrid";
import styles from "./page.module.css";

// Prevent build-time prerendering since API is only available at runtime (Docker)
export const dynamic = "force-dynamic";

export default async function NewPaintingsPage() {
    // Fetch new paintings from API
    const paintings = await getNewPaintings();

    // Convert API paintings to PaintingImageItem format
    // The PaintingGrid component will use smart row building to group paintings by orientation
    // IMPORTANT: Use isLandscape from the API (based on JPEG pixel dimensions) NOT physical dimensions
    const images: PaintingImageItem[] = paintings.map(painting => ({
        src: painting.imageUrl,
        thumbnailUrl: painting.thumbnailUrl,
        alt: painting.title,
        filename: painting.slug,
        title: painting.title,
        price: painting.price,
        width: painting.width,
        height: painting.height,
        depth: painting.depth,
        isAvailable: painting.isAvailable,
        orientation: painting.isLandscape ? 'landscape' : 'portrait'
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