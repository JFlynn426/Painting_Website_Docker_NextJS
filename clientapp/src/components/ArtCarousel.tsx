'use client'

import { useState, useEffect } from 'react';
import Image from 'next/image';

export default function ArtCarousel() {
    const [currentIndex, setCurrentIndex] = useState(0);

    // Image data
    const images = [
        {
            src: "/Carousel-Paintings/Wind_and_Water-Carousel.jpg",
            alt: "Wind and Water: Brilliant Sailboat on a Windy Day"
        },
        {
            src: "/Carousel-Paintings/Manatees-Carousel.jpg",
            alt: "Buddies: Manatee Buddies Swimming Together"
        },
        {
            src: "/Carousel-Paintings/Solitude-Carousel.jpg",
            alt: "A Lone Rowboat on a Foggy Lake"
        },
        {
            src: "/Carousel-Paintings/Aspens-Carousel.jpg",
            alt: "Aspens in Autumn"
        },

        {
            src: "/Carousel-Paintings/Leatherback-Carousel.jpg",
            alt: "Leatherback: Leatherback Turtle Swimming"
        },
        {
            src: "/Carousel-Paintings/Bird_of_Paradise-Carousel.jpg",
            alt: "Bird of Paradise Flower"
        }
    ];

    // Auto-advance carousel
    useEffect(() => {
        const interval = setInterval(() => {
            setCurrentIndex((prevIndex) => (prevIndex + 1) % images.length);
        }, 5000); // Change image every 5 seconds

        return () => clearInterval(interval);
    }, [images.length]);

    const goToSlide = (index: number) => {
        setCurrentIndex(index);
    };

    const goToPrev = () => {
        setCurrentIndex((prevIndex) =>
            prevIndex === 0 ? images.length - 1 : prevIndex - 1
        );
    };

    const goToNext = () => {
        setCurrentIndex((prevIndex) =>
            prevIndex === images.length - 1 ? 0 : prevIndex + 1
        );
    };

    return (
        <div className="relative w-full max-w-[50rem] h-[50vh] md:h-[75vh] overflow-hidden rounded-lg mx-auto bg-[var(--background)]">
            {/* Images container */}
            <div className="relative w-full h-full">
                {images.map((image, index) => (
                    <div
                        key={index}
                        className={`absolute inset-0 transition-opacity duration-500 ease-in-out ${index === currentIndex ? 'opacity-100' : 'opacity-0'
                            }`}
                    >
                        <div className="flex items-center justify-center h-full">
                            <Image
                                src={image.src}
                                alt={image.alt}
                                width={1000}
                                height={750}
                                className="object-contain h-full"
                                style={{ width: 'auto' }}
                                priority={index === 0}
                                quality={75}
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

            {/* Indicators */}
            <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 flex space-x-2">
                {images.map((_, index) => (
                    <button
                        key={index}
                        onClick={() => goToSlide(index)}
                        className={`w-3 h-3 rounded-full transition ${index === currentIndex ? 'bg-white' : 'bg-white bg-opacity-50'
                            }`}
                        aria-label={`Go to slide ${index + 1}`}
                    />
                ))}
            </div>
        </div>
    );
}