"use client";

interface HamburgerMenuProps {
    isMenuOpen: boolean;
    onToggle: (isOpen: boolean) => void;
}

export default function HamburgerMenu({ isMenuOpen, onToggle }: HamburgerMenuProps) {
    const toggleMenu = () => {
        const newState = !isMenuOpen;
        onToggle(newState);
    };

    return (
        <button
            className="flex flex-col justify-center items-center space-y-1.5 p-2 focus:outline-none"
            onClick={toggleMenu}
            aria-label={isMenuOpen ? "Close menu" : "Open menu"}
            aria-expanded={isMenuOpen}
        >
            <span className={`w-6 h-0.5 bg-white transition-all duration-300 ${isMenuOpen ? 'rotate-45 translate-y-2' : ''}`}></span>
            <span className={`w-6 h-0.5 bg-white transition-all duration-300 ${isMenuOpen ? 'opacity-0' : ''}`}></span>
            <span className={`w-6 h-0.5 bg-white transition-all duration-300 ${isMenuOpen ? '-rotate-45 -translate-y-2' : ''}`}></span>
        </button>
    );
}