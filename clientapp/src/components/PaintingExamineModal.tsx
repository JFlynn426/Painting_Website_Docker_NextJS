"use client";

import { useState, useRef, useEffect, useCallback } from "react";
import Image from "next/image";
import { Painting, formatDimensions } from "@/app/models/paintings";
import styles from "./PaintingExamineModal.module.css";

interface PaintingExamineModalProps {
    onClose: () => void;
    painting: Painting;
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

    // Calculate initial scale to fit image in viewport
    useEffect(() => {
        if (imageDimensions.width === 0 || imageDimensions.height === 0) return;
        if (!containerRef.current) return;

        const containerWidth = containerRef.current.clientWidth;
        const containerHeight = containerRef.current.clientHeight;

        // Calculate scale to fit image within available space (contain mode)
        const scaleX = containerWidth / imageDimensions.width;
        const scaleY = containerHeight / imageDimensions.height;
        const fitScale = Math.min(scaleX, scaleY);

        setScale(fitScale);
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

    // Handle zoom with mouse wheel
    const handleWheel = useCallback((e: React.WheelEvent) => {
        e.preventDefault();
        const zoomSensitivity = 0.001;
        const delta = -e.deltaY * zoomSensitivity;
        const newScale = Math.max(0.01, Math.min(scale + delta, 5));
        setScale(newScale);
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

    // Handle drag end - add to document to catch releases outside the container
    const handleMouseUp = useCallback(() => {
        setIsDragging(false);
    }, []);

    // Add global mouse up listener when dragging starts
    useEffect(() => {
        if (isDragging) {
            document.addEventListener('mouseup', handleMouseUp);
            return () => {
                document.removeEventListener('mouseup', handleMouseUp);
            };
        }
    }, [isDragging, handleMouseUp]);

    // Handle zoom in/out buttons
    const handleZoomIn = () => {
        setScale(prev => Math.min(prev * 1.2, 5));
    };

    const handleZoomOut = () => {
        setScale(prev => Math.max(prev / 1.2, 0.01));
    };

    const handleReset = () => {
        if (imageDimensions.width === 0 || imageDimensions.height === 0) return;
        if (!containerRef.current) return;

        const containerWidth = containerRef.current.clientWidth;
        const containerHeight = containerRef.current.clientHeight;

        // Calculate scale to fit image within available space (contain mode)
        const scaleX = containerWidth / imageDimensions.width;
        const scaleY = containerHeight / imageDimensions.height;
        const fitScale = Math.min(scaleX, scaleY);

        setScale(fitScale);
        setPosition({ x: 0, y: 0 });
    };

    const formattedPrice = painting.price ? new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(painting.price) : '';

    const dimensions = formatDimensions(painting);

    return (
        <div className={styles.modalOverlay} onClick={onClose}>
            <div className={styles.modalContent} onClick={e => e.stopPropagation()}>
                {/* Close button */}
                <button className={styles.closeButton} onClick={onClose}>
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path d="M18 6L6 18M6 6l12 12" />
                    </svg>
                </button>

                {/* Instructions */}
                <div className={styles.instructions}>
                    <p>Scroll to zoom • Drag to pan • Escape to close</p>
                </div>

                {/* Zoom controls */}
                <div className={styles.zoomControls}>
                    <button className={styles.zoomButton} onClick={handleZoomOut} title="Zoom Out (-)">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                            <path d="M20 12H4" />
                        </svg>
                    </button>
                    <span className={styles.zoomLevel}>{Math.round(scale * 100)}%</span>
                    <button className={styles.zoomButton} onClick={handleZoomIn} title="Zoom In (+)">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                            <path d="M12 4v16M20 12H4" />
                        </svg>
                    </button>
                    <button className={styles.zoomButton} onClick={handleReset} title="Reset View">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                            <path d="M3 12a9 9 0 1 0 9-9 9.75 9.75 0 0 0-6.74 2.74L3 8" />
                            <path d="M3 3v5h5" />
                        </svg>
                    </button>
                </div>

                {/* Image container */}
                <div
                    ref={containerRef}
                    className={styles.imageContainer}
                    onWheel={handleWheel}
                    onMouseDown={handleMouseDown}
                    onMouseMove={handleMouseMove}
                    style={{ cursor: scale > 1 ? (isDragging ? 'grabbing' : 'grab') : 'default' }}
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
                            width={imageDimensions.width || 2000}
                            height={imageDimensions.height || 2000}
                            className={styles.examineImage}
                            priority
                        />
                    </div>
                </div>

                {/* Painting info */}
                <div className={styles.infoPanel}>
                    <h2 className={styles.infoTitle}>{painting.title}</h2>
                    {painting.price && <p className={styles.infoPrice}>{formattedPrice}</p>}
                    {dimensions && <p className={styles.infoDimensions}>{dimensions}</p>}
                    <p className={`${styles.infoAvailability} ${painting.isAvailable ? styles.available : styles.sold}`}>
                        {painting.isAvailable ? 'Available' : 'Sold'}
                    </p>
                    {painting.description && <p className={styles.infoDescription}>{painting.description}</p>}
                </div>
            </div>
        </div>
    );
}