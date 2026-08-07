import Link from 'next/link';

export default function ContentAdminPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-2 text-[var(--title-color)]">Content Management</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Link href="/admin/content/edit" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Edit Content</h2>
                        <p className="text-[var(--foreground)]">
                            Edit page text, descriptions, and about page photo. Update the home page, about page, galleries and contact page.
                        </p>
                    </div>
                </Link>

                <Link href="/admin/content/carousel" className="block">
                    <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6 hover:bg-[var(--admin-hover)] transition-colors cursor-pointer h-[156px] flex flex-col justify-center">
                        <h2 className="text-xl font-bold mb-2 text-[var(--title-color)]">Change Carousel</h2>
                        <p className="text-[var(--foreground)]">
                            Manage the images displayed in the homepage carousel. Add, remove, or reorder the carousel images shown on the front page.
                        </p>
                    </div>
                </Link>
            </div>
        </div>
    );
}
