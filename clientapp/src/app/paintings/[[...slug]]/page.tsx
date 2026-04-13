import { getPaintingBySlug, getCategoryData } from "@/lib/api";
import PaintingDetailsClient from "./PaintingDetailsClient";
import Link from "next/link";
import styles from "./page.module.css";

interface PaintingDetailsPageProps {
    params: Promise<{
        slug?: string[];
    }>;
}

export default async function PaintingDetailsPage({ params }: PaintingDetailsPageProps) {
    const slugParts = (await params).slug || [];

    // For catch-all routes: slug[0] is category, slug[1] is painting slug
    const category = slugParts[0] || '';
    const paintingSlug = slugParts[1] || '';

    // If no painting slug provided, show category page or redirect
    if (!paintingSlug) {
        if (category) {
            // Redirect to category page
            return (
                <div className={styles.container}>
                    <div className={styles.errorContainer}>
                        <h1 className={styles.notFoundTitle}>Category Page</h1>
                        <p className={styles.notFoundText}>Please navigate to the category page.</p>
                        <Link
                            href={`/paintings/${category}`}
                            className={styles.errorBackButton}
                        >
                            Go to {category} Category
                        </Link>
                    </div>
                </div>
            );
        }
        // Redirect to paintings list
        return (
            <div className={styles.container}>
                <div className={styles.errorContainer}>
                    <h1 className={styles.notFoundTitle}>No Painting Selected</h1>
                    <p className={styles.notFoundText}>Please select a painting to view.</p>
                    <Link
                        href="/paintings"
                        className={styles.errorBackButton}
                    >
                        Browse Paintings
                    </Link>
                </div>
            </div>
        );
    }

    // Fetch painting data from API
    const painting = await getPaintingBySlug(paintingSlug);

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