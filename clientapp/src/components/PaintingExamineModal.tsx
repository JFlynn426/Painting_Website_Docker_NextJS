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
    const [position, setPosition] = useState({ x: 0, y: 0 });
    const [isDragging, setIsDragging] = useState(false);
    const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
    // Zoom level as a multiplier (1.0 = 100% contain fit), resolution-independent
    const [zoomLevel, setZoomLevel] = useState(1.0);
    const containerRef = useRef<HTMLDivElement>(null);

    // Check if zoomed away from default contain (100%)
    const isZoomed = zoomLevel !== 1.0;

    // When user zooms, just adjust the resolution-independent zoom level
    const handleZoom = (zoomFactor: number) => {
        setZoomLevel(prev => {
            const newLevel = prev * zoomFactor;
            return Math.max(0.01, Math.min(newLevel, 5));
        });
    };

    // Handle window resize when zoomed - reset to contain mode
    useEffect(() => {
        if (!isZoomed) return;
        const handleResize = () => {
            setZoomLevel(1.0);
            setPosition({ x: 0, y: 0 });
        };
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, [isZoomed]);

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
                    handleZoom(1.2);
                    break;
                case "-":
                case "_":
                    e.preventDefault();
                    handleZoom(1 / 1.2);
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
            const zoomFactor = 1 + delta;
            if (zoomFactor > 1) {
                handleZoom(zoomFactor);
            } else {
                handleZoom(zoomFactor);
            }
        };

        container.addEventListener('wheel', wheelListener, { passive: false });
        return () => {
            container.removeEventListener('wheel', wheelListener);
        };
    }, [isZoomed, zoomLevel]);

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
                                className={`${styles.imageWrapper} ${isZoomed ? '' : styles.imageWrapperContain}`}
                                style={isZoomed ? {
                                    transform: `scale(${zoomLevel}) translate(${position.x / zoomLevel}px, ${position.y / zoomLevel}px)`,
                                    transformOrigin: 'center center',
                                } : undefined}
                            >
                                <Image
                                    src={painting.imageUrl}
                                    alt={painting.title}
                                    fill
                                    style={{ objectFit: 'contain' }}
                                    className={isZoomed ? styles.paintingImage : styles.paintingImageContain}
                                    priority
                                    sizes="100vw"
                                    quality={95}
                                />
                            </div>
                        </div>

                        <div className={styles.controls}>
                            <button
                                className={styles.zoomButton}
                                onClick={() => handleZoom(1 / 1.2)}
                            >
                                -
                            </button>
                            <span className={styles.zoomLevel}>{Math.round(zoomLevel * 100)}%</span>
                            <button
                                className={styles.zoomButton}
                                onClick={() => handleZoom(1.2)}
                            >
                                +
                            </button>
                            <button
                                className={styles.resetButton}
                                onClick={() => {
                                    setZoomLevel(1.0);
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