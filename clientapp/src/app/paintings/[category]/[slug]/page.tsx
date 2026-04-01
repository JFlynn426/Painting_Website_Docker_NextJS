import { getPaintingBySlug } from "@/lib/api";
import PaintingDetailsClient from "./PaintingDetailsClient";
import Link from "next/link";
import styles from "./page.module.css";

interface PaintingDetailsPageProps {
    params: Promise<{
        category: string;
        slug: string;
    }>;
}

export default async function PaintingDetailsPage({ params }: PaintingDetailsPageProps) {
    const { category, slug } = await params;

    // Fetch painting data from API
    const painting = await getPaintingBySlug(slug);

    if (!painting) {
        return (
            <div className={styles.container}>
                <div className={styles.errorContainer}>
                    <h1 className={styles.notFoundTitle}>Painting Not Found</h1>
                    <p className={styles.notFoundText}>Sorry, the painting you are looking for does not exist.</p>
                    <Link
                        href={`/paintings/${category}`}
                        className={styles.errorBackButton}
                    >
                        Back to Category
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div className={styles.container}>
            <div className={styles.contentWrapper}>
                <PaintingDetailsClient painting={painting} category={category} />
            </div>
        </div>
    );
}
