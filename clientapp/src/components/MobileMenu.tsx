"use client";

import Link from 'next/link';
import PaintingsDropdown from './PaintingsDropdown';
import { PaintingCategory } from '@/types';

interface MobileMenuProps {
    categories: PaintingCategory[];
    isOpen: boolean;
    onClose: () => void;
}

export default function MobileMenu({ categories, isOpen, onClose }: MobileMenuProps) {
    if (!isOpen) return null;

    return (
        <div className="flex flex-col space-y-2 w-full items-center">
            <Link
                href="/"
                className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full"
                onClick={onClose}
            >
                Home
            </Link>
            <Link
                href="/about"
                className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full"
                onClick={onClose}
            >
                About
            </Link>
            <PaintingsDropdown categories={categories} />
            <Link
                href="/paintings/new-paintings"
                className="pr-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full"
                onClick={onClose}
            >
                New Paintings
            </Link>
            <Link
                href="/galleries"
                className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full"
                onClick={onClose}
            >
                Galleries
            </Link>
            <Link
                href="/contact"
                className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full"
                onClick={onClose}
            >
                Contact
            </Link>
        </div>
    );
}