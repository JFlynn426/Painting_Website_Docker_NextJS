"use client";

import { useState, useEffect } from 'react';
import { getAllPaintingCategories } from '@/lib/api';
import HamburgerMenu from './HamburgerMenu';
import MobileMenu from './MobileMenu';
import PaintingsDropdown from './PaintingsDropdown';
import Link from 'next/link';
import { PaintingCategory } from '@/types';

export default function NavBar() {
    const [categories, setCategories] = useState<PaintingCategory[]>([]);
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [isLoaded, setIsLoaded] = useState(false);

    useEffect(() => {
        const fetchCategories = async () => {
            const data = await getAllPaintingCategories();
            setCategories(data);
            setIsLoaded(true);
        };
        fetchCategories();
    }, []);

    // Close menu when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            const navBar = document.querySelector('nav');
            if (navBar && !navBar.contains(event.target as Node)) {
                setIsMenuOpen(false);
            }
        };

        if (isMenuOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isMenuOpen]);

    if (!isLoaded) {
        return (
            <nav className="bg-[var(--navbar-footer-bg)] text-white sticky-top">
                <div className="container mx-auto px-4 py-3">
                    <div className="flex items-center justify-center mb-3">
                        <Link href="/" className="text-4xl md:text-5xl font-bold" style={{ color: 'var(--title-color)' }}>
                            Gloria Gronowicz Fine Art
                        </Link>
                    </div>
                </div>
                <div className="h-px bg-white w-full"></div>
            </nav>
        );
    }

    return (
        <nav className="bg-[var(--navbar-footer-bg)] text-white sticky-top">
            <div className="container mx-auto px-4 py-3">
                {/* Header Row - Centered Logo with Hamburger Button on Right */}
                <div className="relative flex items-center justify-center mb-3">
                    <Link href="/" className="text-4xl md:text-5xl font-bold" style={{ color: 'var(--title-color)' }}>
                        Gloria Gronowicz Fine Art
                    </Link>

                    {/* Hamburger Menu Button - Visible only on screens smaller than 770px, positioned on right */}
                    <div className="absolute right-0 lg:hidden">
                        <HamburgerMenu isMenuOpen={isMenuOpen} onToggle={setIsMenuOpen} />
                    </div>
                </div>

                {/* Mobile Menu - Rendered outside the absolute container */}
                <MobileMenu categories={categories} isOpen={isMenuOpen} onClose={() => setIsMenuOpen(false)} />

                {/* Desktop Navigation - Visible only on screens 770px and larger */}
                <div className="hidden lg:flex lg:space-x-4 justify-center">
                    <Link
                        href="/"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400"
                    >
                        Home
                    </Link>
                    <Link
                        href="/about"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400"
                    >
                        About
                    </Link>
                    <PaintingsDropdown categories={categories} />
                    <Link
                        href="/paintings/new-paintings"
                        className="pr-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400"
                    >
                        New Paintings
                    </Link>
                    <Link
                        href="/galleries"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400"
                    >
                        Galleries
                    </Link>
                    <Link
                        href="/contact"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400"
                    >
                        Contact
                    </Link>
                </div>
            </div>
            <div className="h-px bg-white w-full"></div>
        </nav>
    );
}