import Link from 'next/link';

export default function EditContentPage() {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-6 text-[var(--title-color)]">Edit Content</h1>

            <div className="bg-[var(--navbar-footer-bg)] rounded-lg p-6">
                <p className="text-gray-400">
                    Edit page text, descriptions, and other website content. This form will allow you to update the content displayed on various pages of the website.
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
