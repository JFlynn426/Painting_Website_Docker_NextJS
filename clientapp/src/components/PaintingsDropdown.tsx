"use client";

import { useState, useRef, useEffect } from 'react';
import Link from 'next/link';
import { PaintingCategory } from '@/types';

interface PaintingsDropdownProps {
    categories: PaintingCategory[];
}

export default function PaintingsDropdown({ categories }: PaintingsDropdownProps) {
    const [isPaintingsOpen, setIsPaintingsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const closePaintingsDropdown = () => {
        setIsPaintingsOpen(false);
    };

    // Close dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsPaintingsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <div
            className="relative"
            ref={dropdownRef}
        >
            <button
                onClick={() => setIsPaintingsOpen(!isPaintingsOpen)}
                className="pl-3 py-2 rounded flex items-center hover:text-blue-400 justify-center transition duration-200 ease-in-out w-full"
            >
                Paintings
                <span style={{
                    display: 'inline-block',
                    transform: isPaintingsOpen ? 'rotate(0deg) scale(0.6)' : 'rotate(90deg) scale(0.6)',
                    marginLeft: '0.25rem',
                    transition: 'transform 0.2s ease',
                    transformOrigin: 'center',
                    position: 'relative',
                    top: '-2px',
                    width: '3rem',
                    right: '1rem'
                }}>
                    ▼
                </span>
            </button>
            {isPaintingsOpen && (
                <div className="absolute left-0 top-full mt-1 bg-[#3a3a3a] rounded-md shadow-lg py-1 z-50 min-w-[200px]">
                    {categories
                        .filter(category => category.slug !== 'new-paintings')
                        .map((category) => (
                            <Link
                                key={category.id}
                                href={`/paintings/${category.slug}`}
                                onClick={closePaintingsDropdown}
                                className="block px-4 py-2 transition duration-200 ease-in-out hover:bg-[#1e3a8a] text-center"
                            >
                                {category.name}
                            </Link>
                        ))}
                </div>
            )}
        </div>
    );
}