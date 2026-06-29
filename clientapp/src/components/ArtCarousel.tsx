'use client'

import { useState, useEffect } from 'react';
import Image from 'next/image';
import { getPageContent } from '@/lib/api';

export default function ArtCarousel() {
    const [currentIndex, setCurrentIndex] = useState(0);
    const [photoUrls, setPhotoUrls] = useState<string[]>([]);
    const [loading, setLoading] = useState(true);

    // Fetch carousel images from home page content photoUrls
    useEffect(() => {
        async function loadCarouselImages() {
            try {
                const homeContent = await getPageContent('home');
                if (homeContent?.photoUrls && homeContent.photoUrls.length > 0) {
                    setPhotoUrls(homeContent.photoUrls);
                }
            } catch (error) {
                console.error('Failed to load carousel images:', error);
            } finally {
                setLoading(false);
            }
        }
        loadCarouselImages();
    }, []);

    // Auto-advance carousel
    useEffect(() => {
        if (photoUrls.length === 0) return;
        const interval = setInterval(() => {
            setCurrentIndex((prevIndex) => (prevIndex + 1) % photoUrls.length);
        }, 5000); // Change image every 5 seconds

        return () => clearInterval(interval);
    }, [photoUrls.length]);

    const goToSlide = (index: number) => {
        setCurrentIndex(index);
    };

    const goToPrev = () => {
        setCurrentIndex((prevIndex) =>
            prevIndex === 0 ? photoUrls.length - 1 : prevIndex - 1
        );
    };

    const goToNext = () => {
        setCurrentIndex((prevIndex) =>
            prevIndex === photoUrls.length - 1 ? 0 : prevIndex + 1
        );
    };

    if (loading || photoUrls.length === 0) {
        return null;
    }

    return (
        <div className="relative w-full max-w-[50rem] aspect-[3/2] overflow-hidden rounded-lg mx-auto bg-[var(--navbar-footer-bg)]">
            {/* Images container */}
            <div className="relative w-full h-full">
                {photoUrls.map((url, index) => (
                    <div
                        key={index}
                        className={`absolute inset-0 transition-opacity duration-500 ease-in-out ${index === currentIndex ? 'opacity-100' : 'opacity-0'
                            }`}
                    >
                        <div className="flex items-center justify-center h-full">
                            <Image
                                src={url}
                                alt={`Carousel image ${index + 1}`}
                                width={1000}
                                height={750}
                                className="object-contain h-full"
                                style={{ width: 'auto' }}
                                priority={index === 0}
                                quality={50}
                                unoptimized={url.startsWith('/images/')}
                            />
                        </div>
                    </div>
                ))}
            </div>

            {/* Navigation buttons - constrained to close to image edges */}
            <button
                onClick={goToPrev}
                className="absolute left-2 top-1/2 transform -translate-y-1/2 bg-black bg-opacity-50 text-white p-2 rounded-full hover:bg-opacity-75 transition"
                aria-label="Previous slide"
            >
                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                </svg>
            </button>
            <button
                onClick={goToNext}
                className="absolute right-2 top-1/2 transform -translate-y-1/2 bg-black bg-opacity-50 text-white p-2 rounded-full hover:bg-opacity-75 transition"
                aria-label="Next slide"
            >
                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                </svg>
            </button>

            {/* Dots indicator */}
            <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 flex gap-2">
                {photoUrls.map((_, index) => (
                    <button
                        key={index}
                        onClick={() => goToSlide(index)}
                        className={`w-3 h-3 rounded-full transition-all ${index === currentIndex
                            ? 'bg-white scale-110'
                            : 'bg-white bg-opacity-50 hover:bg-opacity-75'
                            }`}
                        aria-label={`Go to slide ${index + 1}`}
                    />
                ))}
            </div>
        </div>
    );
}
