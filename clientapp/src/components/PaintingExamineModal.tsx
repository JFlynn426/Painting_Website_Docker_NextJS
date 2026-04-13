"use client";

import { useState, useRef, useEffect } from "react";
import Image from "next/image";
import { Painting } from "@/types";
import styles from "./PaintingExamineModal.module.css";

interface PaintingExamineModalProps {
    onClose: () => void;
    painting: Painting;
}

/**
 * Formats painting dimensions into a readable string.
 */
function formatDimensions(painting: Painting): string {
    if (!painting.width || !painting.height) return '';
    return `${painting.width}" x ${painting.height}"${painting.depth ? ` x ${painting.depth}"` : ''}`;
}

export default function PaintingExamineModal({ onClose, painting }: PaintingExamineModalProps) {
    const [scale, setScale] = useState(1);
    const [position, setPosition] = useState({ x: 0, y: 0 });
    const [isDragging, setIsDragging] = useState(false);
    const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
    const [imageDimensions, setImageDimensions] = useState({ width: 0, height: 0 });
    const containerRef = useRef<HTMLDivElement>(null);

    // Get image natural dimensions
    useEffect(() => {
        const img = document.createElement('img');
        img.src = painting.imageUrl;
        img.onload = () => {
            setImageDimensions({ width: img.naturalWidth, height: img.naturalHeight });
        };
    }, [painting.imageUrl]);

    // Calculate initial scale to fit image in viewport and reset position
    useEffect(() => {
        if (imageDimensions.width === 0 || imageDimensions.height === 0) return;
        if (!containerRef.current) return;

        const containerWidth = containerRef.current.clientWidth;
        const containerHeight = containerRef.current.clientHeight;

        // Check if we're on mobile (matches CSS breakpoint at 1024px)
        const isMobile = window.innerWidth < 1024;

        // On desktop, account for the details section width (350px)
        // On mobile, the details section is below the image, so use full width
        const detailsSectionWidth = isMobile ? 0 : 350;
        const availableWidth = containerWidth - detailsSectionWidth;

        // Account for controls bar at bottom (~100px height)
        const controlsHeight = 100;
        const availableHeight = containerHeight - controlsHeight;

        // Calculate scale to fit image within available space (contain mode)
        const scaleX = availableWidth / imageDimensions.width;
        const scaleY = availableHeight / imageDimensions.height;
        const fitScale = Math.min(scaleX, scaleY);

        // Increase default scale by 20% for better initial viewing
        setScale(fitScale * 1.2);
        // Reset position to center when scale is recalculated
        setPosition({ x: 0, y: 0 });
    }, [imageDimensions]);

    // Handle body overflow
    useEffect(() => {
        document.body.style.overflow = "hidden";
        return () => {
            document.body.style.overflow = "unset";
        };
    }, []);

    // Handle keyboard events
    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            switch (e.key) {
                case "Escape":
                    onClose();
                    break;
                case "+":
                case "=":
                    e.preventDefault();
                    setScale(prev => Math.min(prev * 1.2, 5));
                    break;
                case "-":
                case "_":
                    e.preventDefault();
                    setScale(prev => Math.max(prev / 1.2, 0.01));
                    break;
            }
        };

        document.addEventListener("keydown", handleKeyDown);
        return () => document.removeEventListener("keydown", handleKeyDown);
    }, [onClose]);

    // Add wheel event listener with passive: false to allow preventDefault
    useEffect(() => {
        const container = containerRef.current;
        if (!container) return;

        const wheelListener = (e: WheelEvent) => {
            e.preventDefault();
            const zoomSensitivity = 0.001;
            const delta = -e.deltaY * zoomSensitivity;
            const newScale = Math.max(0.01, Math.min(scale + delta, 5));
            setScale(newScale);
        };

        container.addEventListener('wheel', wheelListener, { passive: false });
        return () => {
            container.removeEventListener('wheel', wheelListener);
        };
    }, [scale]);

    // Handle drag start
    const handleMouseDown = (e: React.MouseEvent) => {
        e.preventDefault();
        setIsDragging(true);
        setDragStart({ x: e.clientX - position.x, y: e.clientY - position.y });
    };

    // Handle drag move
    const handleMouseMove = (e: React.MouseEvent) => {
        if (!isDragging) return;
        e.preventDefault();
        setPosition({
            x: e.clientX - dragStart.x,
            y: e.clientY - dragStart.y,
        });
    };

    // Handle drag end
    const handleMouseUp = () => {
        setIsDragging(false);
    };

    // Format price in USD
    const formattedPrice = painting.price
        ? new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(painting.price)
        : '';

    // Format dimensions
    const dimensions = formatDimensions(painting);

    return (
        <div className={styles.modalOverlay} onClick={onClose}>
            <div className={styles.modalContent} onClick={(e) => e.stopPropagation()}>
                <button className={styles.closeButton} onClick={onClose}>
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                        <line x1="18" y1="6" x2="6" y2="18"></line>
                        <line x1="6" y1="6" x2="18" y2="18"></line>
                    </svg>
                </button>

                <div className={styles.modalBody}>
                    <div className={styles.imageSection}>
                        <div
                            ref={containerRef}
                            className={styles.imageContainer}
                            onMouseDown={handleMouseDown}
                            onMouseMove={handleMouseMove}
                            onMouseUp={handleMouseUp}
                            onMouseLeave={handleMouseUp}
                        >
                            <div
                                className={styles.imageWrapper}
                                style={{
                                    transform: `scale(${scale}) translate(${position.x}px, ${position.y}px)`,
                                    transformOrigin: 'center center',
                                }}
                            >
                                <Image
                                    src={painting.imageUrl}
                                    alt={painting.title}
                                    width={imageDimensions.width || 800}
                                    height={imageDimensions.height || 600}
                                    className={styles.paintingImage}
                                    priority
                                />
                            </div>
                        </div>

                        <div className={styles.controls}>
                            <button
                                className={styles.zoomButton}
                                onClick={() => setScale(prev => Math.max(prev / 1.2, 0.01))}
                            >
                                -
                            </button>
                            <span className={styles.zoomLevel}>{Math.round(scale * 100)}%</span>
                            <button
                                className={styles.zoomButton}
                                onClick={() => setScale(prev => Math.min(prev * 1.2, 5))}
                            >
                                +
                            </button>
                            <button
                                className={styles.resetButton}
                                onClick={() => {
                                    setScale(1);
                                    setPosition({ x: 0, y: 0 });
                                }}
                            >
                                Reset
                            </button>
                        </div>
                    </div>

                    <div className={styles.detailsSection}>
                        <h2 className={styles.title}>{painting.title}</h2>

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
                                <span className={`${styles.infoValue} ${painting.isAvailable ? styles.available : styles.sold}`}>
                                    {painting.isAvailable ? 'Available' : 'Sold'}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}