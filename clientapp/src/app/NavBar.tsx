'use client'

import { useState } from 'react';
import Link from 'next/link';
import { paintingCategories } from './models/paintingCategories';

export default function NavBar() {
    const [isPaintingsOpen, setIsPaintingsOpen] = useState(false);

    const closePaintingsDropdown = () => {
        setIsPaintingsOpen(false);
    };

    return (
        <nav className="bg-[var(--navbar-footer-bg)] text-white sticky-top">
            <div className="container mx-auto px-4 py-3">
                <div className="flex flex-col items-center m-3">
                    <Link href="/" className="text-4xl md:text-5xl font-bold" style={{ color: 'var(--title-color)' }}>
                        Gloria Gronowicz Fine Art
                    </Link>
                </div>
                <div className="flex flex-col md:flex-row space-y-2 md:space-y-0 md:space-x-4 w-full md:w-auto justify-center">
                    <Link
                        href="/"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        Home
                    </Link>
                    <Link
                        href="/about"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        About
                    </Link>
                    <div className="relative">
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
                            <div className="relative w-full md:absolute left-0 mt-2 bg-[#3a3a3a] rounded-md shadow-lg py-1 z-10">
                                {paintingCategories.map((category) => (
                                    <Link
                                        key={category.id}
                                        href={`/paintings/${category.slug}`}
                                        onClick={closePaintingsDropdown}
                                        className="block px-4 py-2 transition duration-200 ease-in-out hover:bg-[#1e3a8a] text-center w-full"
                                    >
                                        {category.name}
                                    </Link>
                                ))}
                            </div>
                        )}
                    </div>
                    <Link
                        href="/new_paintings"
                        className="pr-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        New Paintings
                    </Link>
                    <Link
                        href="/gallery"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        Gallery
                    </Link>
                    <Link
                        href="/contact"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        Contact
                    </Link>
                </div>
            </div>
            <div className="h-px bg-white w-full"></div>
        </nav>
    );
}