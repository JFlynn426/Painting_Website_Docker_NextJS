import Link from 'next/link';

export default function CarouselContentPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Change Carousel Images</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-gray-400">
                    Manage the images displayed in the homepage carousel. Add, remove, or reorder carousel images.
                </p>
            </div>

            <div className="mt-6">
                <Link href="/admin/content" className="block text-[var(--title-color)] hover:underline">
                    &larr; Back to Content Management
                </Link>
            </div>
        </div>
    );
}
