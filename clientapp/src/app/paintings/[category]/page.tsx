import { getCategoryData, Painting, PaintingCategory } from "@/lib/api";
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
    const images: PaintingImageItem[] = categoryData.paintings.map(painting => ({
        src: painting.imageUrl,
        alt: painting.title,
        filename: painting.slug,
        href: `/paintings/${category}/${painting.slug}`
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