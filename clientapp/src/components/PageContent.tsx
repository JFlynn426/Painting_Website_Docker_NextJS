import { getPageContent } from "@/lib/api";
import { sanitizeHtml } from "@/lib/sanitization";

interface PageContentProps {
    address: string;
    titleClassName?: string;
    contentClassName?: string;
}

/**
 * Reusable server component for rendering page content from the API.
 * Uses Next.js data fetching with caching for optimal performance.
 * 
 * @param address - The page content address (e.g., 'about', 'galleries', 'contact')
 * @param titleClassName - Optional custom CSS class for the title
 * @param contentClassName - Optional custom CSS class for the content container
 */
export default async function PageContent({
    address,
    titleClassName = "text-3xl mb-8 text-center",
    contentClassName = "max-w-4xl mx-auto mb-8"
}: PageContentProps) {
    // Fetch page content from API with 24-hour cache
    const pageContent = await getPageContent(address);

    if (!pageContent) {
        return null;
    }

    return (
        <div className="flex flex-col items-center justify-center p-4 sm:p-2 md:p-1 text-[var(--foreground)]">
            {/* Title from page content */}
            {pageContent.title && (
                <h1 className={titleClassName} style={{ color: 'var(--title-color)' }}>
                    {pageContent.title}
                </h1>
            )}

            {/* Content from page content */}
            <div className={contentClassName} dangerouslySetInnerHTML={{ __html: sanitizeHtml(pageContent.content) }} />
        </div>
    );
}