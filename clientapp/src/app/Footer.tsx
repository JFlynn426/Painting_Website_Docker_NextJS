'use client'

import Link from 'next/link';

export default function Footer() {
    return (
        <footer className="bg-[var(--navbar-footer-bg)] text-white">
            <div className="h-px bg-white w-full"></div>
            <div className="container mx-auto px-4 py-8">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    <div className="flex flex-col">
                        <h3 className="text-xl font-bold mb-4 text-blue-300">Email</h3>
                        <p className="text-lg">gloriagronowicz@gmail.com</p>
                    </div>
                    <div className="flex flex-col">
                        <h3 className="text-xl font-bold mb-4 text-blue-300">Phone</h3>
                        <p className="text-lg">860.670.0799</p>
                    </div>
                </div>
            </div>
        </footer>
    );
}