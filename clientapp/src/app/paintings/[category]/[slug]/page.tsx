"use client";

import { getPaintingBySlug, Painting } from "@/lib/api";
import PaintingDetailImage from "@/components/PaintingDetailImage";
import PaintingExamineModal from "@/components/PaintingExamineModal";
import Link from "next/link";
import { useState, use, useEffect } from "react";
import styles from "./page.module.css";

interface PaintingDetailsPageProps {
    params: Promise<{
        category: string;
        slug: string;
    }>;
}

export default function PaintingDetailsPage({ params }: PaintingDetailsPageProps) {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [painting, setPainting] = useState<Painting | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // Unwrap params using React's use() for client components
    const { category, slug } = use(params);

    // Fetch painting data from API
    useEffect(() => {
        async function fetchPainting() {
            try {
                const data = await getPaintingBySlug(slug);
                setPainting(data);
            } catch (err) {
                setError('Failed to load painting');
                console.error(err);
            } finally {
                setLoading(false);
            }
        }
        fetchPainting();
    }, [slug]);

    if (loading) {
        return (
            <div className={styles.container}>
                <p>Loading...</p>
            </div>
        );
    }

    if (error || !painting) {
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

    // Format price in USD
    const formattedPrice = painting.price ? new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(painting.price) : '';

    // Format dimensions
    const dimensions = painting.width && painting.height
        ? `${painting.width}" x ${painting.height}"${painting.depth ? ` x ${painting.depth}"` : ''}`
        : null;

    return (
        <>
            <div className={styles.container}>
                <div className={styles.contentWrapper}>
                    <div className={styles.imageSection} onClick={() => setIsModalOpen(true)}>
                        <PaintingDetailImage
                            src={painting.imageUrl}
                            alt={painting.title}
                            className={styles.paintingImage}
                        />
                    </div>

                    <div className={styles.detailsSection}>
                        <h1 className={styles.title}>{painting.title}</h1>

                        {painting.description && (
                            <p className={styles.description}>{painting.description}</p>
                        )}

                        <div className={styles.infoGrid}>
                            {dimensions && (
                                <div className={styles.infoItem}>
                                    <span className={styles.infoLabel}>Dimensions:</span>
                                    <span className={styles.infoValue}>{dimensions}</span>
                                </div>
                            )}

                            {painting.year && (
                                <div className={styles.infoItem}>
                                    <span className={styles.infoLabel}>Year:</span>
                                    <span className={styles.infoValue}>{painting.year}</span>
                                </div>
                            )}

                            {painting.price && (
                                <div className={styles.infoItem}>
                                    <span className={styles.infoLabel}>Price:</span>
                                    <span className={styles.price}>{formattedPrice}</span>
                                </div>
                            )}

                            <div className={styles.infoItem}>
                                <span className={styles.infoLabel}>Availability:</span>
                                <span className={`${styles.infoValue} ${painting.isAvailable ? styles.available : styles.unavailable}`}>
                                    {painting.isAvailable ? 'Available' : 'Sold'}
                                </span>
                            </div>
                        </div>

                        <div className={styles.buttonGroup}>
                            <Link
                                href={`/paintings/${category}`}
                                className={styles.backButton}
                            >
                                ← Back to Category
                            </Link>

                            {painting.isAvailable && painting.price && (
                                <button className={styles.inquireButton}>
                                    Inquire About This Piece
                                </button>
                            )}

                            <button
                                className={styles.examineButton}
                                onClick={() => setIsModalOpen(true)}
                            >
                                <svg className={styles.magnifyingGlass} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <circle cx="11" cy="11" r="8"></circle>
                                    <path d="m21 21-4.35-4.35"></path>
                                </svg>
                                Examine This Painting
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {isModalOpen && painting && (
                <PaintingExamineModal
                    key={painting.title}
                    onClose={() => setIsModalOpen(false)}
                    painting={painting}
                />
            )}
        </>
    );
}
