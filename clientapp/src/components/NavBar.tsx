import { getAllPaintingCategories } from '@/lib/api';
import PaintingsDropdown from './PaintingsDropdown';
import Link from 'next/link';

export default async function NavBar() {
    // Fetch painting categories from API on the server
    const categories = await getAllPaintingCategories();

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
                    <PaintingsDropdown categories={categories} />
                    <Link
                        href="/paintings/new-paintings"
                        className="pr-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        New Paintings
                    </Link>
                    <Link
                        href="/galleries"
                        className="px-3 py-2 rounded transition duration-200 ease-in-out hover:text-blue-400 text-center w-full md:w-auto"
                    >
                        Galleries
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