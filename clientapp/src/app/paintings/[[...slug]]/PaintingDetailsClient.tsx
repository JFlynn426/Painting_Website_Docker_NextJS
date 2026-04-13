"use client";

import { Painting } from "@/types";
import PaintingDetailImage from "@/components/PaintingDetailImage";
import PaintingExamineModal from "@/components/PaintingExamineModal";
import Link from "next/link";
import { useState } from "react";
import styles from "./page.module.css";

interface PaintingDetailsClientProps {
    painting: Painting;
    category: string;
}

export default function PaintingDetailsClient({ painting, category }: PaintingDetailsClientProps) {
    const [isModalOpen, setIsModalOpen] = useState(false);

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
                        <Link href="/contact" className={styles.inquireButton}>
                            Inquire About This Piece
                        </Link>
                    )}

                    {!painting.isAvailable && (
                        <Link href="/contact" className={styles.inquireButton}>
                            Inquire About Prints
                        </Link>
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