import { getCategoryData } from "@/lib/api";
import PaintingGrid, { PaintingImageItem } from "@/components/PaintingGrid";
import styles from "./page.module.css";

interface CategoryPageProps {
    params: Promise<{
        category: string;
    }>;
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

    // Convert API paintings to PaintingImageItem format
    // The PaintingGrid component will use smart row building to group paintings by orientation
    // IMPORTANT: Use isLandscape from the API (based on JPEG pixel dimensions) NOT physical dimensions
    const images: PaintingImageItem[] = categoryData.paintings.map(painting => ({
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
            <h1 className={styles.categoryTitle}>{categoryData.name}</h1>
            {categoryData.description && (
                <p className={styles.description}>{categoryData.description}</p>
            )}

            <PaintingGrid images={images} categorySlug={category} />
        </div>
    );
}