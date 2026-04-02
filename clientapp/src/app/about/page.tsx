import { getPageContent } from "../../lib/api";
import { sanitizeHtml } from "../../lib/sanitization";

export default async function About() {
    // Get about page content from API
    const aboutContent = await getPageContent('about');

    return (
        <div className="flex flex-col items-center justify-center p-4 sm:p-2 md:p-1 text-[var(--foreground)]">
            {/* Title from page content */}
            {aboutContent?.title && (
                <h1 className="text-3xl mb-8 text-center" style={{ color: 'var(--title-color)' }}>
                    {aboutContent.title}
                </h1>
            )}

            {/* Content from page content */}
            {aboutContent && (
                <div className="max-w-4xl mx-auto mb-8" dangerouslySetInnerHTML={{ __html: sanitizeHtml(aboutContent.content) }} />
            )}
        </div>
    );
}