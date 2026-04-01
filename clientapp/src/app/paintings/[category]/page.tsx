import { getCategoryData } from "@/lib/api";
import PaintingGrid, { PaintingImageItem } from "@/components/PaintingGrid";
import styles from "./page.module.css";

interface CategoryPageProps {
    params: Promise<{
        category: string;
    }>;
}

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

export default async function CategoryPage({ params }: CategoryPageProps) {
    const { category } = await params;

    // Fetch category data with paintings from API
    const categoryData = await getCategoryData(category);

    if (!categoryData) {
        return (
            <div className="container">
                <h1>Category Not Found</h1>
            </div>
        );
    }

    // Debug: log first painting to see what values we're getting
    if (categoryData.paintings.length > 0) {
        const firstPainting = categoryData.paintings[0];
        console.log('First painting from API:', {
            title: firstPainting.title,
            width: firstPainting.width,
            height: firstPainting.height,
            depth: firstPainting.depth
        });
    }

    // Convert API paintings to PaintingImageItem format
    const images: PaintingImageItem[] = categoryData.paintings.map(painting => {
        const orientation = getPaintingOrientation(painting.width, painting.height);
        console.log(`Painting: ${painting.title}, width: ${painting.width}, height: ${painting.height}, orientation: ${orientation}`);

        return {
            src: painting.imageUrl,
            alt: painting.title,
            filename: painting.slug,
            title: painting.title,
            price: painting.price,
            width: painting.width,
            height: painting.height,
            depth: painting.depth,
            isAvailable: painting.isAvailable,
            orientation
        };
    });

    return (
        <div className={styles.container}>
            <h1 className={styles.categoryTitle}>{categoryData.name}</h1>
            {categoryData.description && (
                <p className={styles.description}>{categoryData.description}</p>
            )}

            <PaintingGrid images={images} categorySlug={category} />
        </div>
    );
}